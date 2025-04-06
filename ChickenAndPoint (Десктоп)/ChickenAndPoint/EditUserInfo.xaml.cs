using ChickenAndPoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChickenAndPoint.Admin
{
    public partial class EditUserInfo : Window
    {
        private UserViewModel _userToEdit;
        private List<Роль> _availableRoles = new List<Роль>();

        public EditUserInfo(UserViewModel userViewModel)
        {
            InitializeComponent();
            _userToEdit = userViewModel ?? throw new ArgumentNullException(nameof(userViewModel));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FullNameTextBox.Text = _userToEdit.ПолноеИмя;
            PhoneTextBox.Text = _userToEdit.НомерТелефона;
            PochtaTextBox.Text = _userToEdit.Почта;
            Title = $"Редактирование: {_userToEdit.ПолноеИмя}";
            await LoadRolesAsync();
        }

        private async Task LoadRolesAsync()
        {
            RoleComboBox.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var rolesResponse = await App.SupabaseClient.From<Роль>().Select("*").Get();
                if (rolesResponse?.Models != null)
                {
                    _availableRoles = rolesResponse.Models.OrderBy(r => r.НазваниеРоли).ToList();
                    RoleComboBox.ItemsSource = _availableRoles;
                    RoleComboBox.SelectedValue = _userToEdit.IdРоли;
                    RoleComboBox.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить список доступных ролей.", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке ролей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                if (!RoleComboBox.IsEnabled && RoleComboBox.Items.Count == 0)
                {
                    SaveButton.IsEnabled = false;
                    MessageBox.Show("Не удалось загрузить роли. Редактирование роли невозможно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите роль пользователя.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string newFullName = FullNameTextBox.Text.Trim();
            string newPhoneNumber = PhoneTextBox.Text.Trim();
            string newPochta = PochtaTextBox.Text.Trim();
            Guid newRoleId = (Guid)RoleComboBox.SelectedValue;

            if (!string.IsNullOrEmpty(newPochta) && !newPochta.Contains("@"))
            {
                MessageBox.Show("Пожалуйста, введите корректный адрес почты.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newFullName == _userToEdit.ПолноеИмя &&
                newPhoneNumber == _userToEdit.НомерТелефона &&
                newPochta == (_userToEdit.Почта ?? string.Empty) && 
                newRoleId == _userToEdit.IdРоли)
            {
                this.DialogResult = false;
                this.Close();
                return;
            }

            SaveButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    SaveButton.IsEnabled = true; CancelButton.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                    return;
                }

                var updateResponse = await App.SupabaseClient.From<Пользователь>()
                    .Where(u => u.Id == _userToEdit.Id)
                    .Set(u => u.ПолноеИмя, newFullName)
                    .Set(u => u.НомерТелефона, newPhoneNumber)
                    .Set(u => u.Почта, newPochta)
                    .Set(u => u.IdРоли, newRoleId)
                    .Update();

                if (updateResponse.ResponseMessage.IsSuccessStatusCode)
                {
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorMsg = $"Ошибка при обновлении данных пользователя: {updateResponse.ResponseMessage.ReasonPhrase} ({(int)updateResponse.ResponseMessage.StatusCode})";
                    MessageBox.Show(errorMsg, "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                    SaveButton.IsEnabled = true; CancelButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла критическая ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                SaveButton.IsEnabled = true; CancelButton.IsEnabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
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