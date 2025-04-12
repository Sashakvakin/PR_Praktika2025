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
using System.Globalization;
using ChickenAndPoint;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Net.Http;
using System.Configuration;
using Postgrest.Exceptions;

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
    public class OrderDisplayViewModel
    {
        public Guid Id { get; set; }
        public string НомерЗаказа { get; set; }
        public string ИмяКлиента { get; set; }
        public string НазваниеСтатуса { get; set; }
        public string НазваниеТипа { get; set; }
        public string АдресДоставки { get; set; } 
        public decimal? ИтоговаяСумма { get; set; }
        public DateTimeOffset ВремяСоздания { get; set; }
        public DateTimeOffset ВремяОбновления { get; set; }
        public Guid IdСтатуса { get; set; }
        public Guid IdТипа { get; set; }
        public Guid IdКлиента { get; set; }
    }

    public class DishAdminViewModel : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, BitmapImage> ImageCache = new Dictionary<string, BitmapImage>();
        private static readonly object CacheLock = new object();
        private static readonly HashSet<string> FailedUrls = new HashSet<string>();

        private static readonly HttpClient httpClient = new HttpClient();

        public Guid Id { get; set; }
        public Guid IdКатегории { get; set; }
        public string НазваниеБлюда { get; set; }
        public string Описание { get; set; }
        public decimal Цена { get; set; }
        private string _ссылкаНаИзображение;
        public string СсылкаНаИзображение
        {
            get => _ссылкаНаИзображение;
            set => SetProperty(ref _ссылкаНаИзображение, value);
        }
        public bool Доступно { get; set; }
        public string НазваниеКатегории { get; set; }

        private BitmapImage _dishImageSource;
        public BitmapImage DishImageSource
        {
            get => _dishImageSource;
            private set => SetProperty(ref _dishImageSource, value);
        }

        private bool _isLoadingImage;
        public bool IsLoadingImage
        {
            get => _isLoadingImage;
            private set => SetProperty(ref _isLoadingImage, value);
        }

        public async Task LoadImageAsync()
        {
            if (string.IsNullOrEmpty(this.СсылкаНаИзображение) || DishImageSource != null || IsLoadingImage)
            {
                return;
            }

            IsLoadingImage = true;

            BitmapImage image = null;
            bool foundInCache = false;

            lock (CacheLock)
            {
                foundInCache = ImageCache.TryGetValue(this.СсылкаНаИзображение, out image);
            }

            if (foundInCache && image != null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => DishImageSource = image);
            }
            else
            {
                byte[] imageData = await Task.Run(() => DownloadImageDataAsync(this.СсылкаНаИзображение));

                if (imageData != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            BitmapImage bitmap = new BitmapImage();
                            using (var stream = new System.IO.MemoryStream(imageData))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                            }

                            bitmap.Freeze();

                            lock (CacheLock)
                            {
                                if (!ImageCache.ContainsKey(this.СсылкаНаИзображение))
                                {
                                    ImageCache.Add(this.СсылкаНаИзображение, bitmap);
                                }
                                else
                                {
                                    bitmap = ImageCache[this.СсылкаНаИзображение];
                                }
                            }
                            DishImageSource = bitmap;
                        }
                        catch
                        {
                        }
                    });
                }
            }

            IsLoadingImage = false;
        }

        private async Task<byte[]> DownloadImageDataAsync(string url)
        {
            try
            {
                byte[] imageData = await httpClient.GetByteArrayAsync(url);
                return imageData;
            }
            catch
            {
                 Application.Current?.Dispatcher.InvokeAsync(() => {
                      if (!FailedUrls.Contains(url))
                      {
                          FailedUrls.Add(url);
                      }
                 });
                return null;
            }
        }

        public static void ClearImageCache()
        {
            lock (CacheLock)
            {
                ImageCache.Clear();
                FailedUrls.Clear();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public partial class AdminMainWindow : Window
    {
        private Пользователь _loggedInUser;
        private Dictionary<Guid, string> _roleNamesCache = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _categoryNamesCache = new Dictionary<Guid, string>();
        private ObservableCollection<DishAdminViewModel> _dishViewModels = new ObservableCollection<DishAdminViewModel>();
        private List<UserViewModel> _allLoadedUsers = new List<UserViewModel>();
        private List<DishAdminViewModel> _allLoadedDishes = new List<DishAdminViewModel>();
        private List<KeyValuePair<Guid?, string>> _categoryFilterSource = new List<KeyValuePair<Guid?, string>>();

        private List<OrderDisplayViewModel> _allAdminOrders = new List<OrderDisplayViewModel>();
        private Dictionary<Guid, string> _adminStatusNames = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _adminTypeNames = new Dictionary<Guid, string>();
        private List<KeyValuePair<Guid?, string>> _adminOrderStatusFilterSource = new List<KeyValuePair<Guid?, string>>();
        private List<KeyValuePair<Guid?, string>> _adminOrderTypeFilterSource = new List<KeyValuePair<Guid?, string>>();
        private Dictionary<Guid, string> _clientNamesCache = new Dictionary<Guid, string>();

        public static readonly Guid DeliveryTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["DeliveryTypeUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid PickupTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["PickupTypeUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid PackagingPickupTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["PackagingPickupTypeUUID"] ?? Guid.Empty.ToString());

        private void UpdateDatePickersEnabledState()
        {
            bool enablePickers = !(AdminShowAllTimeCheckBox.IsChecked ?? false);
            AdminStartDatePicker.IsEnabled = enablePickers;
            AdminEndDatePicker.IsEnabled = enablePickers;
        }
        private void AdminShowAllTimeCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateDatePickersEnabledState();
            if (this.IsLoaded && OrdersPanel.IsVisible)
            {
                ApplyAdminOrderFilters();
            }
        }
        public AdminMainWindow(Пользователь user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DishesItemsControl.ItemsSource = _dishViewModels;

            AdminStartDatePicker.SelectedDate = DateTime.Today;
            AdminEndDatePicker.SelectedDate = DateTime.Today;
            AdminShowAllTimeCheckBox.IsChecked = false;
            UpdateDatePickersEnabledState();

            ShowSection("Профиль");
        }


        private void AdminDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded && OrdersPanel.IsVisible &&
                AdminStartDatePicker.SelectedDate.HasValue && AdminEndDatePicker.SelectedDate.HasValue)
            {
                if (AdminStartDatePicker.SelectedDate.Value > AdminEndDatePicker.SelectedDate.Value)
                {
                    return;
                }
                ApplyAdminOrderFilters();
            }
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
            CategoriesPanel.Visibility = Visibility.Collapsed;

            EditUserButton.IsEnabled = false;
            DeleteUserButton.IsEnabled = false;
            EditCategoryButton.IsEnabled = false;
            DeleteCategoryButton.IsEnabled = false;

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
                    await LoadAdminOrdersAsync();
                    break;
                case "Категории":
                    CategoriesPanel.Visibility = Visibility.Visible;
                    await LoadCategoriesForManagementAsync();
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
            _allLoadedUsers.Clear();
            _clientNamesCache.Clear();

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
                    _clientNamesCache = usersResponse.Models
                        .Where(u => u != null)
                        .ToDictionary(u => u.Id, u => u.ПолноеИмя ?? "Имя не указано");

                    if (!usersResponse.Models.Any())
                    {
                        UsersLoadingStatus.Text = "Список пользователей пуст.";
                        UsersLoadingStatus.Visibility = Visibility.Visible;
                        _allLoadedUsers = new List<UserViewModel>();
                        ApplyUserFilter();
                        UsersDataGrid.Visibility = Visibility.Visible;
                        return;
                    }

                    var userViewModels = usersResponse.Models
                        .Where(u => u != null)
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

                    _allLoadedUsers = userViewModels;
                    ApplyUserFilter();
                    UsersDataGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    UsersLoadingStatus.Text = "Не удалось загрузить пользователей.";
                    _allLoadedUsers = new List<UserViewModel>();
                    ApplyUserFilter();
                    UsersLoadingStatus.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                UsersLoadingStatus.Text = $"Ошибка загрузки: {ex.Message}";
                UsersLoadingStatus.Visibility = Visibility.Visible;
                _allLoadedUsers = new List<UserViewModel>();
                _clientNamesCache.Clear();
                ApplyUserFilter();
                MessageBox.Show($"Произошла ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyUserFilter()
        {
            if (_allLoadedUsers == null) return;

            string searchText = UserSearchTextBox.Text.Trim().ToLowerInvariant();
            IEnumerable<UserViewModel> filteredList = _allLoadedUsers;

            if (!string.IsNullOrEmpty(searchText))
            {
                filteredList = _allLoadedUsers.Where(user =>
                    (user.ПолноеИмя?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (user.Почта?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (user.НомерТелефона?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (user.НазваниеРоли?.ToLowerInvariant().Contains(searchText) ?? false)
                );
            }

            UsersDataGrid.ItemsSource = filteredList.ToList();

            if (!_allLoadedUsers.Any())
            {
                UsersLoadingStatus.Text = "Список пользователей пуст.";
                UsersLoadingStatus.Visibility = Visibility.Visible;
            }
            else if (!filteredList.Any() && !string.IsNullOrEmpty(searchText))
            {
                UsersLoadingStatus.Text = "Пользователи не найдены по вашему запросу.";
                UsersLoadingStatus.Visibility = Visibility.Visible;
            }
            else
            {
                UsersLoadingStatus.Visibility = Visibility.Collapsed;
            }
        }

        private void UserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded && UsersPanel.IsVisible)
            {
                ApplyUserFilter();
            }
        }

        private void ResetUserSearchButton_Click(object sender, RoutedEventArgs e)
        {
            UserSearchTextBox.Text = string.Empty;
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
            DishAdminViewModel.ClearImageCache();

            if (_allLoadedDishes.Any() && _categoryNamesCache.Any())
            {
                System.Diagnostics.Debug.WriteLine("[LoadDishesAsync] Using cached dishes and category names.");
                foreach (var vm in _allLoadedDishes) { _ = vm.LoadImageAsync(); }
                ApplyDishFilter();
                return;
            }

            DishesLoadingStatus.Text = "Загрузка блюд...";
            DishesLoadingStatus.Visibility = Visibility.Visible;
            _dishViewModels.Clear();
            _allLoadedDishes.Clear();
            DishesItemsControl.Visibility = Visibility.Collapsed;

            bool categoriesLoaded = await LoadAndPopulateCategoriesFilterAsync();

            try
            {
                if (App.SupabaseClient == null)
                {
                    DishesLoadingStatus.Text = "Клиент Supabase не инициализирован.";
                    MessageBox.Show(DishesLoadingStatus.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ApplyDishFilter();
                    return;
                }

                if (_categoryNamesCache.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[LoadDishesAsync] Category name cache is empty, loading...");
                    var categoriesResponse = await App.SupabaseClient.From<Категории>().Select("*").Get();
                    if (categoriesResponse?.Models != null)
                    {
                        _categoryNamesCache = categoriesResponse.Models.ToDictionary(c => c.Id, c => c.НазваниеКатегории ?? "?");
                        System.Diagnostics.Debug.WriteLine($"[LoadDishesAsync] Category name cache loaded with {_categoryNamesCache.Count} items.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось загрузить справочник категорий при обновлении блюд.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                System.Diagnostics.Debug.WriteLine("[LoadDishesAsync] Loading dishes from DB...");
                var dishesResponse = await App.SupabaseClient.From<Блюда>().Select("*").Get();

                if (dishesResponse?.Models != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadDishesAsync] Loaded {dishesResponse.Models.Count} dishes from DB.");
                    _allLoadedDishes = dishesResponse.Models
                       .OrderBy(d => d.НазваниеБлюда)
                       .Select(dish => new DishAdminViewModel
                       {
                           Id = dish.Id,
                           IdКатегории = dish.IdКатегории,
                           НазваниеБлюда = dish.НазваниеБлюда,
                           Описание = dish.Описание,
                           Цена = dish.Цена,
                           СсылкаНаИзображение = dish.СсылкаНаИзображение,
                           Доступно = dish.Доступно,
                           НазваниеКатегории = _categoryNamesCache.TryGetValue(dish.IdКатегории, out string catName) ? catName : "???"
                       }).ToList();

                    foreach (var vm in _allLoadedDishes)
                    {
                        _ = vm.LoadImageAsync();
                    }
                }
                else
                {
                    DishesLoadingStatus.Text = "Не удалось загрузить блюда.";
                    System.Diagnostics.Debug.WriteLine("[LoadDishesAsync] Failed to load dishes from DB.");
                }
            }
            catch (Exception ex)
            {
                DishesLoadingStatus.Text = $"Ошибка загрузки блюд: {ex.Message}";
                MessageBox.Show(DishesLoadingStatus.Text, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ApplyDishFilter();
            }
        }

        private async Task<bool> LoadAndPopulateCategoriesFilterAsync()
        {
            if (_categoryFilterSource.Any()) return true;

            _categoryFilterSource.Clear();

            try
            {
                if (App.SupabaseClient == null) return false;

                var categoriesResponse = await App.SupabaseClient.From<Категории>().Select("*").Get();

                _categoryFilterSource.Add(new KeyValuePair<Guid?, string>(null, "[ Все категории ]"));

                if (categoriesResponse?.Models != null && categoriesResponse.Models.Any())
                {
                    var sortedCategories = categoriesResponse.Models.OrderBy(c => c.НазваниеКатегории).ToList();

                    foreach (var category in sortedCategories)
                    {
                        if (!_categoryNamesCache.ContainsKey(category.Id))
                        {
                            _categoryNamesCache[category.Id] = category.НазваниеКатегории ?? "???";
                        }
                        _categoryFilterSource.Add(new KeyValuePair<Guid?, string>(category.Id, category.НазваниеКатегории ?? "???"));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Не удалось загрузить категории или список пуст для фильтра.");
                }

                DishCategoryFilterComboBox.ItemsSource = null;
                DishCategoryFilterComboBox.ItemsSource = _categoryFilterSource;
                DishCategoryFilterComboBox.SelectedIndex = 0;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий для фильтра: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                if (!_categoryFilterSource.Any(kvp => kvp.Key == null)) { _categoryFilterSource.Insert(0, new KeyValuePair<Guid?, string>(null, "[ Все категории ]")); }
                DishCategoryFilterComboBox.ItemsSource = null; DishCategoryFilterComboBox.ItemsSource = _categoryFilterSource; DishCategoryFilterComboBox.SelectedIndex = 0;
                return false;
            }
        }

        private void ApplyDishFilter()
        {
            if (_allLoadedDishes == null) _allLoadedDishes = new List<DishAdminViewModel>();
            if (_dishViewModels == null) _dishViewModels = new ObservableCollection<DishAdminViewModel>();

            IEnumerable<DishAdminViewModel> filteredList = _allLoadedDishes;

            Guid? selectedFilterComboBoxCategoryId = null;
            if (DishCategoryFilterComboBox.SelectedValue is Guid guidValue) selectedFilterComboBoxCategoryId = guidValue;
            if (selectedFilterComboBoxCategoryId.HasValue)
            {
                filteredList = filteredList.Where(dish => dish.IdКатегории == selectedFilterComboBoxCategoryId.Value);
            }

            string searchText = DishSearchTextBox.Text.Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredList = filteredList.Where(dish =>
                   (dish.НазваниеБлюда?.ToLowerInvariant().Contains(searchText) ?? false) ||
                   (dish.Описание?.ToLowerInvariant().Contains(searchText) ?? false) ||
                   (dish.НазваниеКатегории?.ToLowerInvariant().Contains(searchText) ?? false) ||
                   (dish.Цена.ToString(CultureInfo.InvariantCulture).Contains(searchText))
               );
            }

            _dishViewModels.Clear();
            foreach (var dish in filteredList)
            {
                _dishViewModels.Add(dish);
            }

            UpdateDishesLoadingStatus(filteredList);
        }

        private void UpdateDishesLoadingStatus(IEnumerable<DishAdminViewModel> filteredList)
        {
            if (!_allLoadedDishes.Any())
            {
                if (DishesLoadingStatus.Text == "Загрузка блюд...") DishesLoadingStatus.Text = "Список блюд пуст.";
                DishesLoadingStatus.Visibility = Visibility.Visible;
                DishesItemsControl.Visibility = Visibility.Collapsed;
            }
            else if (!filteredList.Any())
            {
                DishesLoadingStatus.Text = "Блюда, соответствующие фильтрам, не найдены.";
                DishesLoadingStatus.Visibility = Visibility.Visible;
                DishesItemsControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                DishesLoadingStatus.Visibility = Visibility.Collapsed;
                DishesItemsControl.Visibility = Visibility.Visible;
            }
        }
        private void DishSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded && DishesPanel.IsVisible)
            {
                ApplyDishFilter();
            }
        }

        private void DishCategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded && DishesPanel.IsVisible && e.AddedItems.Count > 0)
            {
                ApplyDishFilter();
            }
        }

        private void ResetDishSearchButton_Click(object sender, RoutedEventArgs e)
        {
            bool textChanged = false;
            bool categoryComboBoxChanged = false;

            if (DishSearchTextBox.Text != string.Empty)
            {
                DishSearchTextBox.Text = string.Empty;
                textChanged = true;
            }
            if (DishCategoryFilterComboBox != null && DishCategoryFilterComboBox.SelectedIndex != 0)
            {
                DishCategoryFilterComboBox.SelectedIndex = 0;
                categoryComboBoxChanged = true;
            }

            if (!textChanged && !categoryComboBoxChanged)
            {
                ApplyDishFilter();
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

        private async Task LoadAdminOrdersAsync()
        {
            AdminOrdersStatusText.Text = "Загрузка заказов...";
            AdminOrdersStatusText.Visibility = Visibility.Visible;
            AdminOrdersDataGrid.ItemsSource = null;
            _allAdminOrders.Clear();
            _adminStatusNames.Clear();
            _adminTypeNames.Clear();
            _adminOrderStatusFilterSource.Clear();
            _adminOrderTypeFilterSource.Clear();

            AdminStatusFilterComboBox.ItemsSource = null;
            AdminTypeFilterComboBox.ItemsSource = null;

            try
            {
                if (App.SupabaseClient == null)
                {
                    AdminOrdersStatusText.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    return;
                }

                var statusesTask = App.SupabaseClient.From<СтатусЗаказа>().Select("*").Get();
                var typesTask = App.SupabaseClient.From<ТипЗаказа>().Select("*").Get();

                await Task.WhenAll(statusesTask, typesTask);

                var statusesResponse = statusesTask.Result;
                if (statusesResponse?.Models != null)
                {
                    _adminStatusNames = statusesResponse.Models.ToDictionary(s => s.Id, s => s.НазваниеСтатуса ?? "?");
                    PopulateAdminFilterComboBox(AdminStatusFilterComboBox, _adminStatusNames, "статусы", ref _adminOrderStatusFilterSource);
                }
                else { AdminOrdersStatusText.Text = "Ошибка: Не удалось загрузить статусы."; }

                var typesResponse = typesTask.Result;
                if (typesResponse?.Models != null)
                {
                    _adminTypeNames = typesResponse.Models.ToDictionary(t => t.Id, t => t.НазваниеТипа ?? "?");
                    PopulateAdminFilterComboBox(AdminTypeFilterComboBox, _adminTypeNames, "типы", ref _adminOrderTypeFilterSource);
                }
                else { AdminOrdersStatusText.Text = "Ошибка: Не удалось загрузить типы."; }

                var ordersResponse = await App.SupabaseClient.From<Заказы>().Select("*").Get();

                if (ordersResponse?.Models == null || !ordersResponse.Models.Any())
                {
                    AdminOrdersStatusText.Text = "Заказы не найдены.";
                    _allAdminOrders = new List<OrderDisplayViewModel>();
                    ApplyAdminOrderFilters();
                    AdminOrdersStatusText.Visibility = Visibility.Visible;
                    return;
                }

                var orders = ordersResponse.Models;
                var clientIds = orders.Select(o => o.IdКлиента).Distinct().ToList();

                bool needToLoadClients = false;
                if (_clientNamesCache == null || !_clientNamesCache.Any()) needToLoadClients = true;
                else
                {
                    foreach (var id in clientIds)
                    {
                        if (!_clientNamesCache.ContainsKey(id))
                        {
                            needToLoadClients = true;
                            break;
                        }
                    }
                }

                if (needToLoadClients && clientIds.Any())
                {
                    var clientIdsStr = clientIds.Select(id => id.ToString()).ToList();
                    Debug.WriteLine($"Loading clients for admin orders: {string.Join(",", clientIdsStr)}");
                    var usersResponse = await App.SupabaseClient.From<Пользователь>()
                                           .Select("*")
                                           .Filter("id", Operator.In, clientIdsStr)
                                           .Get();
                    if (usersResponse?.Models != null)
                    {
                        foreach (var user in usersResponse.Models)
                        {
                            _clientNamesCache[user.Id] = user.ПолноеИмя ?? "Имя не указано";
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Failed to load client names for admin orders.");
                    }
                }

                var viewModels = orders.Select(order => new OrderDisplayViewModel
                {
                    Id = order.Id,
                    НомерЗаказа = order.НомерЗаказа ?? "-",
                    ИмяКлиента = _clientNamesCache.TryGetValue(order.IdКлиента, out string clientName) ? clientName : "Клиент (?)",
                    НазваниеСтатуса = _adminStatusNames.TryGetValue(order.IdСтатуса, out string statusName) ? statusName : "Статус (?)",
                    НазваниеТипа = _adminTypeNames.TryGetValue(order.IdТипа, out string typeName) ? typeName : "Тип (?)",
                    АдресДоставки = string.IsNullOrWhiteSpace(order.АдресДоставки) ?
                                    (_adminTypeNames.TryGetValue(order.IdТипа, out string tName) ? tName : "Тип (?)")
                                    : order.АдресДоставки,
                    ИтоговаяСумма = order.ИтоговаяСумма,
                    ВремяСоздания = order.ВремяСоздания,
                    ВремяОбновления = order.ВремяОбновления,
                    IdСтатуса = order.IdСтатуса,
                    IdТипа = order.IdТипа,
                    IdКлиента = order.IdКлиента
                }).ToList();

                var sortedViewModels = viewModels.OrderByDescending(vm => vm.ВремяСоздания).ToList();
                _allAdminOrders = sortedViewModels;
                ApplyAdminOrderFilters();
                AdminOrdersStatusText.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                AdminOrdersStatusText.Text = $"Ошибка загрузки заказов: {ex.Message}";
                AdminOrdersStatusText.Visibility = Visibility.Visible;
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _allAdminOrders = new List<OrderDisplayViewModel>();
                ApplyAdminOrderFilters();
            }
        }

        private void PopulateAdminFilterComboBox(ComboBox comboBox, Dictionary<Guid, string> dataSource, string typeName, ref List<KeyValuePair<Guid?, string>> filterSourceList)
        {
            filterSourceList.Clear();
            filterSourceList.Add(new KeyValuePair<Guid?, string>(null, $"[Все {typeName}]"));
            filterSourceList.AddRange(dataSource.OrderBy(kvp => kvp.Value)
                                                .Select(kvp => new KeyValuePair<Guid?, string>(kvp.Key, kvp.Value)));
            comboBox.ItemsSource = filterSourceList;
            comboBox.SelectedIndex = 0;
        }
        private void ApplyAdminOrderFilters()
        {
            if (_allAdminOrders == null) return;

            IEnumerable<OrderDisplayViewModel> filteredList = _allAdminOrders;

            Guid? selectedStatusId = null;
            if (AdminStatusFilterComboBox.SelectedValue is Guid statusGuid) selectedStatusId = statusGuid;
            if (selectedStatusId.HasValue)
            {
                filteredList = filteredList.Where(vm => vm.IdСтатуса == selectedStatusId.Value);
            }

            Guid? selectedTypeId = null;
            if (AdminTypeFilterComboBox.SelectedValue is Guid typeGuid) selectedTypeId = typeGuid;
            if (selectedTypeId.HasValue)
            {
                filteredList = filteredList.Where(vm => vm.IdТипа == selectedTypeId.Value);
            }

            bool showAllTime = AdminShowAllTimeCheckBox.IsChecked ?? false;
            if (!showAllTime && AdminStartDatePicker.SelectedDate.HasValue && AdminEndDatePicker.SelectedDate.HasValue)
            {
                DateTime startDate = AdminStartDatePicker.SelectedDate.Value.Date;
                DateTime endDateExclusive = AdminEndDatePicker.SelectedDate.Value.Date.AddDays(1);
                DateTimeOffset startOffset = new DateTimeOffset(startDate);
                DateTimeOffset endOffsetExclusive = new DateTimeOffset(endDateExclusive);

                filteredList = filteredList.Where(vm =>
                    vm.ВремяСоздания >= startOffset && vm.ВремяСоздания < endOffsetExclusive
                );
            }

            string searchText = AdminOrderSearchTextBox.Text.Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(searchText))
            {
                filteredList = filteredList.Where(vm =>
                    (vm.НомерЗаказа?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.ИмяКлиента?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.НазваниеСтатуса?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.НазваниеТипа?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.АдресДоставки?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.ИтоговаяСумма?.ToString("F2", CultureInfo.InvariantCulture).Contains(searchText) ?? false) ||
                    (vm.ВремяСоздания.ToString("dd.MM HH:mm", CultureInfo.InvariantCulture).Contains(searchText))
                );
            }

            AdminOrdersDataGrid.ItemsSource = filteredList.ToList();

            if (!_allAdminOrders.Any())
            {
                AdminOrdersStatusText.Text = "Заказы не найдены.";
                AdminOrdersStatusText.Visibility = Visibility.Visible;
            }
            else if (!filteredList.Any())
            {
                AdminOrdersStatusText.Text = "Заказы, соответствующие фильтрам, не найдены.";
                AdminOrdersStatusText.Visibility = Visibility.Visible;
            }
            else if (AdminOrdersStatusText.Visibility == Visibility.Visible)
            {
                AdminOrdersStatusText.Visibility = Visibility.Collapsed;
            }
        }
        private void AdminFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded && OrdersPanel.IsVisible && e.AddedItems.Count > 0)
            {
                ApplyAdminOrderFilters();
            }
        }

        private void AdminOrderSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded && OrdersPanel.IsVisible)
            {
                ApplyAdminOrderFilters();
            }
        }

        private void AdminResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            bool changed = false;

            if (AdminStatusFilterComboBox != null && AdminStatusFilterComboBox.SelectedIndex != 0)
            {
                AdminStatusFilterComboBox.SelectedIndex = 0;
                changed = true;
            }
            if (AdminTypeFilterComboBox != null && AdminTypeFilterComboBox.SelectedIndex != 0)
            {
                AdminTypeFilterComboBox.SelectedIndex = 0;
                changed = true;
            }

            bool dateChanged = false;
            if (AdminStartDatePicker != null && AdminStartDatePicker.SelectedDate != DateTime.Today)
            {
                AdminStartDatePicker.SelectedDate = DateTime.Today;
                dateChanged = true;
            }
            if (AdminEndDatePicker != null && AdminEndDatePicker.SelectedDate != DateTime.Today)
            {
                AdminEndDatePicker.SelectedDate = DateTime.Today;
                dateChanged = true;
            }

            bool checkBoxChanged = false;
            if (AdminShowAllTimeCheckBox != null && AdminShowAllTimeCheckBox.IsChecked == true)
            {
                AdminShowAllTimeCheckBox.IsChecked = false;
                checkBoxChanged = true;
                changed = true;
            }

            if (!changed || (dateChanged && !checkBoxChanged))
            {
                ApplyAdminOrderFilters();
            }
        }



        private void AdminOrderDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid orderId)
            {
                OrderAllInformationWindow detailsWindow = new OrderAllInformationWindow(orderId);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
        }
        private void AdminResetSearchButton_Click(object sender, RoutedEventArgs e)
        {

            if (AdminOrderSearchTextBox != null)
            {
                AdminOrderSearchTextBox.Text = string.Empty;
            }
        }
        private async Task LoadCategoriesForManagementAsync()
        {
            CategoriesLoadingStatus.Text = "Загрузка категорий...";
            CategoriesLoadingStatus.Visibility = Visibility.Visible;
            CategoriesDataGrid.ItemsSource = null;
            CategoriesDataGrid.Visibility = Visibility.Collapsed;
            EditCategoryButton.IsEnabled = false;
            DeleteCategoryButton.IsEnabled = false;

            try
            {
                if (App.SupabaseClient == null)
                {
                    CategoriesLoadingStatus.Text = "Клиент Supabase не инициализирован.";
                    MessageBox.Show(CategoriesLoadingStatus.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var categoriesResponse = await App.SupabaseClient.From<Категории>().Select("*").Get();

                if (categoriesResponse?.Models != null)
                {
                    if (!categoriesResponse.Models.Any())
                    {
                        CategoriesLoadingStatus.Text = "Список категорий пуст.";
                        CategoriesDataGrid.ItemsSource = new List<Категории>();
                    }
                    else
                    {
                        var sortedCategories = categoriesResponse.Models.OrderBy(c => c.НазваниеКатегории).ToList();
                        CategoriesDataGrid.ItemsSource = sortedCategories;
                        CategoriesLoadingStatus.Visibility = Visibility.Collapsed;
                        CategoriesDataGrid.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    CategoriesLoadingStatus.Text = "Не удалось загрузить категории.";
                }
            }
            catch (Exception ex)
            {
                CategoriesLoadingStatus.Text = $"Ошибка загрузки категорий: {ex.Message}";
                MessageBox.Show(CategoriesLoadingStatus.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                CategoriesDataGrid.ItemsSource = new List<Категории>();
            }
            finally
            {
                if (CategoriesDataGrid.ItemsSource != null && (CategoriesDataGrid.ItemsSource as System.Collections.IList)?.Count > 0)
                {
                    CategoriesLoadingStatus.Visibility = Visibility.Collapsed;
                    CategoriesDataGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    CategoriesDataGrid.Visibility = Visibility.Collapsed;
                    CategoriesLoadingStatus.Visibility = Visibility.Visible;
                }
            }
        }

        private void CategoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isSelected = CategoriesDataGrid.SelectedItem != null;
            EditCategoryButton.IsEnabled = isSelected;
            DeleteCategoryButton.IsEnabled = isSelected;
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategoryFoodWindow();
            addCategoryWindow.Owner = this;
            bool? result = addCategoryWindow.ShowDialog();
            if (result == true)
            {
                await LoadCategoriesForManagementAsync();
            }
        }

        private async void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is Категории selectedCategory)
            {
                var editWindow = new EditCategoryFoodWindow(selectedCategory);
                editWindow.Owner = this;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    _categoryNamesCache.Clear();
                    _categoryFilterSource.Clear();
                    _allLoadedDishes.Clear();
                    await LoadCategoriesForManagementAsync();
                    await LoadDishesAsync();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите категорию для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is Категории selectedCategory)
            {
                MessageBoxResult confirmResult = MessageBox.Show(
                   $"Вы уверены, что хотите удалить категорию '{selectedCategory.НазваниеКатегории}'?\n\nВНИМАНИЕ: Удаление категории невозможно, если к ней привязаны блюда!",
                   "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmResult != MessageBoxResult.Yes) return;

                EditCategoryButton.IsEnabled = false;
                DeleteCategoryButton.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                try
                {
                    if (App.SupabaseClient == null)
                    {
                        MessageBox.Show("Клиент Supabase не инициализирован.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    await App.SupabaseClient.From<Категории>()
                               .Where(c => c.Id == selectedCategory.Id)
                               .Delete();

                    MessageBox.Show($"Категория '{selectedCategory.НазваниеКатегории}' успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadCategoriesForManagementAsync();

                }
                catch (PostgrestException pgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"PostgrestException during category delete: {pgEx.Message}");
                    if (pgEx.Message.Contains("violates foreign key constraint") && pgEx.Message.Contains("Блюда_id_категории_fkey"))
                    {
                        MessageBox.Show($"Невозможно удалить категорию '{selectedCategory.НазваниеКатегории}', так как к ней привязаны существующие блюда.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Не удалось удалить категорию: {pgEx.Message}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении категории: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (this.IsVisible)
                    {
                        CategoriesDataGrid_SelectionChanged(null, null);
                        this.Cursor = Cursors.Arrow;
                    }
                }
            }
        }
    }
}