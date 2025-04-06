using ChickenAndPoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Supabase.Gotrue;

namespace ChickenAndPoint
{

    public partial class AddNewUser : Window
    {
        private List<Роль> _availableRoles = new List<Роль>();

        public AddNewUser()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            RoleComboBox.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            try
            {
                if (App.SupabaseClient == null) { MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                var rolesResponse = await App.SupabaseClient.From<Роль>().Select("*").Get();
                if (rolesResponse?.Models != null)
                {
                    _availableRoles = rolesResponse.Models.OrderBy(r => r.НазваниеРоли).ToList();
                    RoleComboBox.ItemsSource = _availableRoles;
                    RoleComboBox.IsEnabled = true;
                }
                else { MessageBox.Show("Не удалось загрузить список доступных ролей.", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Warning); }
            }
            catch (Exception ex) { MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally
            {
                this.Cursor = Cursors.Arrow;
                if (!RoleComboBox.IsEnabled && RoleComboBox.Items.Count == 0) { SaveButton.IsEnabled = false; MessageBox.Show("Не удалось загрузить роли. Добавление пользователя невозможно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text)) { MessageBox.Show("Пожалуйста, введите полное имя.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning); FullNameTextBox.Focus(); return; }
            if (string.IsNullOrWhiteSpace(PochtaTextBox.Text) || !PochtaTextBox.Text.Contains("@")) { MessageBox.Show("Пожалуйста, введите корректный Email.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning); PochtaTextBox.Focus(); return; }
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password) || PasswordTextBox.Password.Length < 6) { MessageBox.Show("Пароль должен содержать минимум 6 символов.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning); PasswordTextBox.Focus(); return; }
            if (RoleComboBox.SelectedValue == null) { MessageBox.Show("Пожалуйста, выберите роль.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            string newFullName = FullNameTextBox.Text.Trim();
            string newEmail = PochtaTextBox.Text.Trim().ToLowerInvariant();
            string newPassword = PasswordTextBox.Password;
            string newPhoneNumber = PhoneTextBox.Text.Trim();
            Guid newRoleId = (Guid)RoleComboBox.SelectedValue;

            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            Session authResponse = null;
            Guid userGuid = Guid.Empty;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    authResponse = await App.SupabaseClient.Auth.SignUp(newEmail, newPassword);
                    if (authResponse?.User == null || string.IsNullOrEmpty(authResponse.User.Id))
                    {
                        throw new Exception("Supabase.Auth.SignUp не вернул данные пользователя.");
                    }
                    if (!Guid.TryParse(authResponse.User.Id, out userGuid) || userGuid == Guid.Empty)
                    {
                        throw new Exception("Не удалось получить корректный ID пользователя из Auth.");
                    }
                }
                catch (Exception authEx)
                {
                    MessageBox.Show($"Ошибка регистрации в Auth: {authEx.Message}", "Ошибка Auth", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    var updateResponse = await App.SupabaseClient.From<Пользователь>()
                       .Where(p => p.Id == userGuid)
                       .Set(p => p.ПолноеИмя, newFullName)
                       .Set(p => p.НомерТелефона, string.IsNullOrWhiteSpace(newPhoneNumber) ? null : newPhoneNumber)
                       .Set(p => p.IdРоли, newRoleId)
                       .Update();

                    if (updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Пользователь '{newFullName}' успешно добавлен и профиль обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        string errorDetails = updateResponse?.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка";
                        int statusCode = (int)(updateResponse?.ResponseMessage?.StatusCode ?? 0);
                        string errorMsg = $"Пользователь в Auth создан (ID: {userGuid}), но произошла ошибка при ОБНОВЛЕНИИ профиля: {errorDetails} ({statusCode})";
                        MessageBox.Show($"{errorMsg}\n\nПрофиль пользователя может быть неполным. Попробуйте отредактировать его вручную.", "Ошибка обновления профиля", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception updateEx)
                {
                    MessageBox.Show($"Произошла ошибка при обновлении профиля: {updateEx.Message}", "Критическая ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла критическая ошибка: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible)
                {
                    SaveButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}