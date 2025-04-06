using ChickenAndPoint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Postgrest.Constants;
using Supabase.Gotrue;
using Postgrest.Models;
using Postgrest.Attributes;
using System.Collections.ObjectModel;


namespace ChickenAndPoint.Admin
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string ПолноеИмя { get; set; }
        public string НомерТелефона { get; set; }
        public string НазваниеРоли { get; set; }
        public Guid IdРоли { get; set; }
        public string Почта { get; set; }
    }

    public class DishAdminViewModel
    {
        public Guid Id { get; set; }
        public Guid IdКатегории { get; set; }
        public string НазваниеБлюда { get; set; }
        public string Описание { get; set; }
        public decimal Цена { get; set; }
        public string СсылкаНаИзображение { get; set; }
        public bool Доступно { get; set; }
        public string НазваниеКатегории { get; set; }
    }


    public partial class AdminMainWindow : Window
    {
        private Пользователь _loggedInUser;
        private Dictionary<Guid, string> _roleNamesCache = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _categoryNamesCache = new Dictionary<Guid, string>();
        private ObservableCollection<DishAdminViewModel> _dishViewModels = new ObservableCollection<DishAdminViewModel>();

        public AdminMainWindow(Пользователь user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DishesItemsControl.ItemsSource = _dishViewModels;
            ShowSection("Профиль");
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string section)
            {
                ShowSection(section);
            }
        }

        private async void ShowSection(string sectionName)
        {
            ProfilePanel.Visibility = Visibility.Collapsed;
            UsersPanel.Visibility = Visibility.Collapsed;
            DishesPanel.Visibility = Visibility.Collapsed;
            OrdersPanel.Visibility = Visibility.Collapsed;
            OrderTypesPanel.Visibility = Visibility.Collapsed;
            OrderStatusesPanel.Visibility = Visibility.Collapsed;

            EditUserButton.IsEnabled = false;
            DeleteUserButton.IsEnabled = false;

            switch (sectionName)
            {
                case "Профиль":
                    ProfilePanel.Visibility = Visibility.Visible;
                    LoadProfileData();
                    break;
                case "Пользователи":
                    UsersPanel.Visibility = Visibility.Visible;
                    await LoadUsersAsync();
                    break;
                case "Блюда":
                    DishesPanel.Visibility = Visibility.Visible;
                    await LoadDishesAsync();
                    break;
                case "Заказы":
                    OrdersPanel.Visibility = Visibility.Visible;
                    // await LoadOrdersAsync();
                    break;
                case "ТипыЗаказов":
                    OrderTypesPanel.Visibility = Visibility.Visible;
                    // await LoadOrderTypesAsync();
                    break;
                case "СтатусыЗаказов":
                    OrderStatusesPanel.Visibility = Visibility.Visible;
                    // await LoadOrderStatusesAsync();
                    break;
                default:
                    ProfilePanel.Visibility = Visibility.Visible;
                    LoadProfileData();
                    break;
            }
        }

        private void LoadProfileData()
        {
            if (_loggedInUser != null)
            {
                AdminFullNameTextBlock.Text = _loggedInUser.ПолноеИмя ?? "-";
                AdminPhoneTextBlock.Text = _loggedInUser.НомерТелефона ?? "-";
            }
            else
            {
                AdminFullNameTextBlock.Text = "Ошибка загрузки данных";
                AdminPhoneTextBlock.Text = "Ошибка загрузки данных";
                MessageBox.Show("Не удалось загрузить данные профиля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task LoadUsersAsync()
        {
            UsersLoadingStatus.Text = "Загрузка пользователей...";
            UsersLoadingStatus.Visibility = Visibility.Visible;
            UsersDataGrid.ItemsSource = null;
            UsersDataGrid.Visibility = Visibility.Collapsed;
            _roleNamesCache.Clear();
            EditUserButton.IsEnabled = false;
            DeleteUserButton.IsEnabled = false;

            try
            {
                if (App.SupabaseClient == null)
                {
                    UsersLoadingStatus.Text = "Клиент Supabase не инициализирован.";
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var rolesResponse = await App.SupabaseClient.From<Роль>().Select("*").Get();
                if (rolesResponse?.Models != null)
                {
                    _roleNamesCache = rolesResponse.Models.ToDictionary(r => r.Id, r => r.НазваниеРоли ?? "Без роли");
                }
                else
                {
                    UsersLoadingStatus.Text = "Не удалось загрузить роли.";
                    MessageBox.Show("Не удалось загрузить справочник ролей.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var usersResponse = await App.SupabaseClient.From<Пользователь>().Select("*").Get();


                if (usersResponse?.Models != null)
                {
                    if (!usersResponse.Models.Any())
                    {
                        UsersLoadingStatus.Text = "Список пользователей пуст.";
                        UsersLoadingStatus.Visibility = Visibility.Visible;
                        UsersDataGrid.ItemsSource = new List<UserViewModel>();
                        UsersDataGrid.Visibility = Visibility.Visible;
                        return;
                    }

                    var userViewModels = usersResponse.Models
                        .OrderBy(u => u.ПолноеИмя)
                        .Select(u => new UserViewModel
                        {
                            Id = u.Id,
                            ПолноеИмя = u.ПолноеИмя ?? "Имя не указано",
                            НомерТелефона = u.НомерТелефона ?? "-",
                            IdРоли = u.IdРоли,
                            НазваниеРоли = _roleNamesCache.TryGetValue(u.IdРоли, out string roleName) ? roleName : "Роль не найдена",
                            Почта = u.Почта ?? "Нет почты"
                        }).ToList();

                    UsersDataGrid.ItemsSource = userViewModels;
                    UsersDataGrid.Visibility = Visibility.Visible;
                    UsersLoadingStatus.Visibility = Visibility.Collapsed;
                }
                else
                {
                    UsersLoadingStatus.Text = "Не удалось загрузить пользователей.";
                    UsersLoadingStatus.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                UsersLoadingStatus.Text = $"Ошибка загрузки: {ex.Message}";
                UsersLoadingStatus.Visibility = Visibility.Visible;
                MessageBox.Show($"Произошла ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = UsersDataGrid.SelectedItem != null;
            EditUserButton.IsEnabled = isSelected;
            DeleteUserButton.IsEnabled = isSelected;
        }

        private async void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddNewUser();
            addUserWindow.Owner = this;
            bool? result = addUserWindow.ShowDialog();

            if (result == true)
            {
                await LoadUsersAsync();
            }
        }

        private async void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is UserViewModel selectedUserVm)
            {
                var editWindow = new EditUserInfo(selectedUserVm);
                editWindow.Owner = this;

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    await LoadUsersAsync();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(UsersDataGrid.SelectedItem is UserViewModel selectedUserVm))
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult confirmResult = MessageBox.Show(
                $"Вы уверены, что хотите удалить пользователя \n\n {selectedUserVm.ПолноеИмя} ({selectedUserVm.Почта})?",
                "Подтверждение удаления профиля",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult != MessageBoxResult.Yes)
            {
                return;
            }

            AddUserButton.IsEnabled = false;
            EditUserButton.IsEnabled = false;
            DeleteUserButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            bool profileDeleted = false;

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    await App.SupabaseClient.From<Пользователь>()
                        .Where(u => u.Id == selectedUserVm.Id)
                        .Delete();

                    profileDeleted = true;
                    System.Diagnostics.Debug.WriteLine($"Профиль пользователя {selectedUserVm.Id} успешно удален.");
                }
                catch (Exception profileEx)
                {
                    MessageBox.Show($"Произошла ошибка при удалении профиля: {profileEx.Message}", "Ошибка удаления профиля", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if (profileDeleted)
                {
                    MessageBox.Show($"Профиль пользователя '{selectedUserVm.ПолноеИмя}' успешно удален.", "Удаление завершено", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadUsersAsync();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла критическая ошибка при удалении профиля: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (this.IsVisible && !profileDeleted)
                {
                    AddUserButton.IsEnabled = true;
                    UsersDataGrid_SelectionChanged(null, null);
                }
                this.Cursor = Cursors.Arrow;
            }
        }


        private async Task LoadDishesAsync()
        {
            DishesLoadingStatus.Text = "Загрузка блюд...";
            DishesLoadingStatus.Visibility = Visibility.Visible;
            _dishViewModels.Clear();
            DishesItemsControl.Visibility = Visibility.Collapsed;

            try
            {
                if (App.SupabaseClient == null)
                {
                    DishesLoadingStatus.Text = "Клиент Supabase не инициализирован.";
                    MessageBox.Show(DishesLoadingStatus.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_categoryNamesCache.Count == 0)
                {

                    var categoriesResponse = await App.SupabaseClient.From<Категории>().Select("*").Get();

                    if (categoriesResponse?.Models != null)
                    {
                        _categoryNamesCache = categoriesResponse.Models.ToDictionary(c => c.Id, c => c.НазваниеКатегории ?? "?");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить справочник категорий.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                var dishesResponse = await App.SupabaseClient.From<Блюда>().Select("*").Get();

                if (dishesResponse?.Models != null)
                {
                    if (!dishesResponse.Models.Any())
                    {
                        DishesLoadingStatus.Text = "Список блюд пуст.";
                        DishesLoadingStatus.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        foreach (var dish in dishesResponse.Models.OrderBy(d => d.НазваниеБлюда))
                        {
                            _dishViewModels.Add(new DishAdminViewModel
                            {
                                Id = dish.Id,
                                IdКатегории = dish.IdКатегории,
                                НазваниеБлюда = dish.НазваниеБлюда,
                                Описание = dish.Описание,
                                Цена = dish.Цена,
                                СсылкаНаИзображение = dish.СсылкаНаИзображение,
                                Доступно = dish.Доступно,
                                НазваниеКатегории = _categoryNamesCache.TryGetValue(dish.IdКатегории, out string catName) ? catName : "???"
                            });
                        }
                        DishesLoadingStatus.Visibility = Visibility.Collapsed;
                        DishesItemsControl.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    DishesLoadingStatus.Text = "Не удалось загрузить блюда.";
                    DishesLoadingStatus.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                DishesLoadingStatus.Text = $"Ошибка загрузки блюд: {ex.Message}";
                DishesLoadingStatus.Visibility = Visibility.Visible;
                MessageBox.Show(DishesLoadingStatus.Text, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.SupabaseClient != null)
                {
                    await App.SupabaseClient.Auth.SignOut();
                }
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void EditDishButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is DishAdminViewModel selectedDishVm)
            {
                var editWindow = new EditFoodWindow(selectedDishVm);
                editWindow.Owner = this;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    await LoadDishesAsync();
                }
            }
            else
            {
                MessageBox.Show("Не удалось получить данные блюда для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AddDishButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddFoodWindow();
            addWindow.Owner = this;

            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                await LoadDishesAsync();
            }
        }
    }
}