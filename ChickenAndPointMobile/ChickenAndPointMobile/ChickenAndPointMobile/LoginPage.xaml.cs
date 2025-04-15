using Supabase.Gotrue.Exceptions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ChickenAndPointMobile.Models;
using System.Linq;
using static Postgrest.Constants;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            EmailEntry.Text = "client@gmail.com";
            PasswordEntry.Text = "111111";
        }

        private void EmailEntry_Completed(object sender, EventArgs e)
        {
            PasswordEntry.Focus();
        }

        private void PasswordEntry_Completed(object sender, EventArgs e)
        {
            PerformLogin();
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            PerformLogin();
        }

        private async void PerformLogin()
        {
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка", "Введите email и пароль", "OK");
                return;
            }

            LoginButton.IsEnabled = false;
            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = true;
            App.LoggedInUserProfile = null;

            try
            {
                var session = await App.SupabaseClient.Auth.SignIn(email, password);

                if (session?.User != null)
                {
                    if (Guid.TryParse(session.User.Id, out Guid userId))
                    {
                        var userProfile = await GetUserProfile(userId);
                        if (userProfile != null)
                        {
                            App.LoggedInUserProfile = userProfile;
                            var userRole = await GetUserRole(userProfile.IdРоли);
                            if (userRole != null)
                            {
                                NavigateBasedOnRole(userRole.НазваниеРоли);
                            }
                            else
                            {
                                await HandleLoginError("Не удалось определить роль пользователя.");
                            }
                        }
                        else
                        {
                            await HandleLoginError("Не удалось загрузить профиль пользователя.");
                        }
                    }
                    else
                    {
                        await HandleLoginError("Некорректный ID пользователя.");
                    }
                }
            }
            catch (GotrueException)
            {
                await DisplayAlert("Ошибка входа", $"Неверный email или пароль.", "OK");
            }
            catch (Postgrest.Exceptions.PostgrestException postgrestEx)
            {
                await HandleLoginError($"Ошибка получения данных: {postgrestEx.Message}");
            }
            catch (Exception ex)
            {
                await HandleLoginError($"Произошла неизвестная ошибка: {ex.Message}");
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoginButton.IsEnabled = true;
                    ActivityIndicator.IsRunning = false;
                    ActivityIndicator.IsVisible = false;
                });
            }
        }

        public async Task<Пользователь> GetUserProfile(Guid userId)
        {
            try
            {
                var response = await App.SupabaseClient
                    .From<Пользователь>()
                    .Filter("id", Operator.Equals, userId.ToString())
                    .Single();
                return response;
            }
            catch (Postgrest.Exceptions.PostgrestException pgEx)
            {
                Console.WriteLine($"Ошибка получения профиля: {pgEx}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка получения профиля: {ex}");
                return null;
            }
        }

        public async Task<Роль> GetUserRole(Guid roleId)
        {
            if (roleId == Guid.Empty) return null;

            try
            {
                var response = await App.SupabaseClient
                    .From<Роль>()
                    .Filter("id", Operator.Equals, roleId.ToString())
                    .Single();
                return response;
            }
            catch (Postgrest.Exceptions.PostgrestException pgEx)
            {
                Console.WriteLine($"Ошибка получения роли: {pgEx}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка получения роли: {ex}");
                return null;
            }
        }

        private void NavigateBasedOnRole(string roleName)
        {
            if ("Клиент".Equals(roleName, StringComparison.OrdinalIgnoreCase))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new ClientMainWindow();
                });
            }
            else if ("Курьер".Equals(roleName, StringComparison.OrdinalIgnoreCase))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Вход успешен", "Окно курьера еще не создано", "OK");
                    await HandleLoginError(null);
                });
            }
            else if ("Сотрудник".Equals(roleName, StringComparison.OrdinalIgnoreCase))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Доступ запрещен", "Роль 'Сотрудник' не поддерживается в мобильном приложении.", "OK");
                    await HandleLoginError(null);
                });
            }
            else if ("Администратор".Equals(roleName, StringComparison.OrdinalIgnoreCase))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Доступ запрещен", "Роль 'Администратор' не поддерживается в мобильном приложении.", "OK");
                    await HandleLoginError(null);
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Ошибка", $"Неизвестная роль пользователя: {roleName}", "OK");
                    await HandleLoginError(null);
                });
            }
        }

        private async Task HandleLoginError(string message)
        {
            App.LoggedInUserProfile = null;
            if (!string.IsNullOrEmpty(message))
            {
                await DisplayAlert("Ошибка входа", message, "OK");
            }
            if (App.SupabaseClient.Auth.CurrentSession != null)
            {
                try
                {
                    await App.SupabaseClient.Auth.SignOut();
                }
                catch (Exception signOutEx)
                {
                    Console.WriteLine($"Ошибка при выходе из системы: {signOutEx}");
                }
            }
            if (!(Application.Current.MainPage is LoginPage))
            {
                Application.Current.MainPage = new LoginPage();
            }
        }
    }
}