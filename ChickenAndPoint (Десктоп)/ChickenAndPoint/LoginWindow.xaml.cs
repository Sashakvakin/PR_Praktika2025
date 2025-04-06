using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Supabase;
using ChickenAndPoint.Models;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using Postgrest.Exceptions;
using static Postgrest.Constants;
using ChickenAndPoint.Admin;

namespace ChickenAndPoint
{
    public partial class LoginWindow : Window
    {
        private Пользователь _loggedInUserData = null;

        public LoginWindow()
        {
            InitializeComponent();
            EmailTextBox.Text = "admin@gmail.com";
            PasswordTextBox.Password = "111111";
        }

        private async Task LoginUser(string email, string password)
        {
            Роль userRoleData = null;
            _loggedInUserData = null;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var session = await App.SupabaseClient.Auth.SignIn(email, password);

                if (session?.User == null)
                {
                    MessageBox.Show("Неверный email или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var userId = Guid.Parse(session.User.Id);

                var userResponse = await App.SupabaseClient
                    .From<Пользователь>()
                    .Select("*")
                    .Filter("id", Operator.Equals, userId.ToString())
                    .Get();

                _loggedInUserData = userResponse?.Models?.FirstOrDefault();

                if (_loggedInUserData == null)
                {
                    await App.SupabaseClient.Auth.SignOut();
                    MessageBox.Show("Профиль пользователя не найден в базе данных 'Пользователи' или доступ запрещен RLS.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_loggedInUserData.IdРоли == Guid.Empty)
                {
                    await App.SupabaseClient.Auth.SignOut();
                    MessageBox.Show("У пользователя не задана роль. Обратитесь к администратору.", "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                    _loggedInUserData = null;
                    return;
                }

                var roleResponse = await App.SupabaseClient
                    .From<Роль>()
                    .Select("*")
                    .Filter("id", Operator.Equals, _loggedInUserData.IdРоли.ToString())
                    .Get();

                userRoleData = roleResponse?.Models?.FirstOrDefault();

                if (userRoleData == null)
                {
                    await App.SupabaseClient.Auth.SignOut();
                    MessageBox.Show("Роль пользователя не найдена в базе данных 'Роли' или доступ запрещен RLS.", "Ошибка конфигурации роли", MessageBoxButton.OK, MessageBoxImage.Error);
                    _loggedInUserData = null;
                    return;
                }

                string userRoleName = userRoleData.НазваниеРоли;
                OpenMainWindowBasedOnRole(userRoleName);
                this.Close();

            }
            catch (GotrueException authEx)
            {
                MessageBox.Show($"Ошибка аутентификации: {authEx.Message}", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                _loggedInUserData = null;
            }
            catch (PostgrestException pgEx)
            {
                MessageBox.Show($"Ошибка при запросе данных из базы: {pgEx.Message}. Проверьте RLS или обратитесь к администратору.", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                _loggedInUserData = null;
                if (App.SupabaseClient?.Auth?.CurrentUser != null)
                {
                    await App.SupabaseClient.Auth.SignOut().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _loggedInUserData = null;
                if (App.SupabaseClient?.Auth?.CurrentUser != null)
                {
                    await App.SupabaseClient.Auth.SignOut().ConfigureAwait(false);
                }
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите email и пароль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoginButton.IsEnabled = false;
            await LoginUser(email, password);
            if (this.IsVisible)
            {
                LoginButton.IsEnabled = true;
            }
        }

        private void OpenMainWindowBasedOnRole(string roleName)
        {
            Window mainWindow = null;
            Пользователь userToPass = _loggedInUserData;

            switch (roleName?.ToLower())
            {
                case "администратор":
                    if (userToPass != null)
                    {
                        mainWindow = new AdminMainWindow(userToPass);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось получить данные пользователя для открытия окна администратора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "сотрудник":
                    if (userToPass != null)
                    {
                        mainWindow = new SotrydnikMainWindow(userToPass);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось получить данные пользователя для открытия окна сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show($"Роль '{roleName}' не имеет доступа к этому приложению.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
            }

            if (mainWindow != null)
            {
                mainWindow.Show();
                this.Close();
            }
        }
    }
}