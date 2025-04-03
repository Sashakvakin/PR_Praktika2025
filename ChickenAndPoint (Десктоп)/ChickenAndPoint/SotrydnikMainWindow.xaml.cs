using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ChickenAndPoint.Models;
using static Postgrest.Constants;

namespace ChickenAndPoint
{

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
    }

    public class OrderItemDisplayViewModel
    {
        public string НазваниеБлюда { get; set; }
        public int Количество { get; set; }
        public decimal ЦенаНаМоментЗаказа { get; set; }
        public string СсылкаНаИзображение { get; set; }
        public decimal СуммаПозиции => Количество * ЦенаНаМоментЗаказа;
    }

    public class CurrentOrderViewModel
    {
        public Guid Id { get; set; }
        public string НомерЗаказа { get; set; }
        public List<OrderItemDisplayViewModel> Items { get; set; }
    }

    public class ImageUrlToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imageUrl && !string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imageUrl, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class SotrydnikMainWindow : Window
    {
        private Пользователь _loggedInUser;
        private ObservableCollection<CurrentOrderViewModel> _currentOrders = new ObservableCollection<CurrentOrderViewModel>();

        private Dictionary<Guid, string> _clientNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _statusNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _typeNamesDictionary = new Dictionary<Guid, string>();
        private List<OrderDisplayViewModel> _allLoadedOrders = new List<OrderDisplayViewModel>();
        private const string AllItemsFilterKey = "[Все]";

        public ObservableCollection<CurrentOrderViewModel> CurrentOrders
        {
            get { return _currentOrders; }
            set { _currentOrders = value; }
        }

        public SotrydnikMainWindow(Пользователь user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DataContext = this;
            ShowProfile();
        }

        private void ShowProfile()
        {
            HideAllPanels();
            ProfilePanel.Visibility = Visibility.Visible;
            LoadProfileData();
        }

        private async void ShowCurrentOrders()
        {
            HideAllPanels();
            CurrentOrdersPanel.Visibility = Visibility.Visible;
            await LoadCurrentOrdersAsync();
        }

        private async void ShowOrdersHistory()
        {
            HideAllPanels();
            OrdersHistoryPanel.Visibility = Visibility.Visible;
            await LoadOrdersHistoryWithDetailsAsync();
        }

        private async Task LoadCurrentOrdersAsync()
        {
            CurrentOrders.Clear();

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Ошибка: Клиент Supabase не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получение ID статусов из конфигурации
                string acceptedStatusUUID = ConfigurationManager.AppSettings["AcceptedStatusUUID"];
                string preparingStatusUUID = ConfigurationManager.AppSettings["PreparingStatusUUID"];
                List<Guid> statusUUIDs = new List<Guid>();

                if (Guid.TryParse(acceptedStatusUUID, out Guid acceptedGuid)) statusUUIDs.Add(acceptedGuid);
                else MessageBox.Show($"Неверный формат GUID для AcceptedStatusUUID: {acceptedStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);

                if (Guid.TryParse(preparingStatusUUID, out Guid preparingGuid)) statusUUIDs.Add(preparingGuid);
                else MessageBox.Show($"Неверный формат GUID для PreparingStatusUUID: {preparingStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);

                if (statusUUIDs.Count == 0)
                {
                    MessageBox.Show($"Не удалось определить UUID статусов 'Принят' или 'Готовится' из конфигурации.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var ordersResponse = await App.SupabaseClient
                    .From<Заказы>()
                    .Select("*")
                    .Get();

                if (ordersResponse?.Models != null)
                {
                    var filteredOrders = ordersResponse.Models
                        .Where(order => statusUUIDs.Contains(order.IdСтатуса))
                        .OrderByDescending(o => o.ВремяСоздания)
                        .ToList();

                    if (!filteredOrders.Any())
                    {
                        return;
                    }

                    var dishesResponse = await App.SupabaseClient.From<Блюда>().Select("*").Get();
                    var allDishesDictionary = dishesResponse?.Models?.ToDictionary(d => d.Id, d => d);

                    var allOrderItemsResponse = await App.SupabaseClient
                        .From<СоставЗаказа>()
                        .Select("*")
                        .Get();

                    Dictionary<Guid, List<СоставЗаказа>> orderItemsByOrderId = new Dictionary<Guid, List<СоставЗаказа>>();
                    if (allOrderItemsResponse?.Models != null)
                    {
                        orderItemsByOrderId = allOrderItemsResponse.Models
                            .GroupBy(item => item.IdЗаказа)
                            .ToDictionary(group => group.Key, group => group.ToList());
                    }

                    foreach (var order in filteredOrders)
                    {
                        List<OrderItemDisplayViewModel> orderItems = new List<OrderItemDisplayViewModel>();

                        if (orderItemsByOrderId.TryGetValue(order.Id, out List<СоставЗаказа> orderItemsList))
                        {
                            foreach (var item in orderItemsList)
                            {
                                string dishName = "Блюдо (?)";
                                string imageUrl = null;
                                if (allDishesDictionary != null && allDishesDictionary.TryGetValue(item.IdБлюда, out Блюда dish))
                                {
                                    dishName = dish.НазваниеБлюда ?? dishName;
                                    imageUrl = dish.СсылкаНаИзображение;
                                }

                                orderItems.Add(new OrderItemDisplayViewModel
                                {
                                    НазваниеБлюда = dishName,
                                    Количество = item.Количество,
                                    ЦенаНаМоментЗаказа = item.ЦенаНаМоментЗаказа, 
                                    СсылкаНаИзображение = imageUrl
                                });
                            }
                        }

                        if (orderItems.Any())
                        {
                            CurrentOrders.Add(new CurrentOrderViewModel
                            {
                                Id = order.Id,
                                НомерЗаказа = order.НомерЗаказа ?? "Нет номера",
                                Items = orderItems
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки текущих заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProfileData()
        {
            if (_loggedInUser != null)
            {
                FullNameTextBlock.Text = _loggedInUser.ПолноеИмя ?? "-";
                PhoneTextBlock.Text = _loggedInUser.НомерТелефона ?? "-";
            }
            else
            {
                FullNameTextBlock.Text = "Ошибка загрузки";
                PhoneTextBlock.Text = "Ошибка загрузки";
            }
        }

        private async Task LoadOrdersHistoryWithDetailsAsync()
    {
        OrdersStatusText.Text = "Загрузка данных истории...";
        OrdersStatusText.Visibility = Visibility.Visible;
        OrdersDataGrid.ItemsSource = null;
        _allLoadedOrders.Clear();
        _clientNamesDictionary.Clear();
        _statusNamesDictionary.Clear();
        _typeNamesDictionary.Clear();

        StatusFilterComboBox.ItemsSource = null;
        TypeFilterComboBox.ItemsSource = null;

        try
        {
            if (App.SupabaseClient == null)
            {
                OrdersStatusText.Text = "Ошибка: Клиент Supabase не инициализирован.";
                return;
            }

            var statusesTask = App.SupabaseClient.From<СтатусЗаказа>().Select("*").Get();
            var typesTask = App.SupabaseClient.From<ТипЗаказа>().Select("*").Get();
            await Task.WhenAll(statusesTask, typesTask);

            var statusesResponse = statusesTask.Result;
            if (statusesResponse?.Models != null)
            {
                _statusNamesDictionary = statusesResponse.Models.ToDictionary(s => s.Id, s => s.НазваниеСтатуса ?? "?");
                PopulateFilterComboBox(StatusFilterComboBox, _statusNamesDictionary, "статусы");
            }
            else { OrdersStatusText.Text = "Ошибка: Не удалось загрузить статусы."; }

            var typesResponse = typesTask.Result;
            if (typesResponse?.Models != null)
            {
                _typeNamesDictionary = typesResponse.Models.ToDictionary(t => t.Id, t => t.НазваниеТипа ?? "?");
                PopulateFilterComboBox(TypeFilterComboBox, _typeNamesDictionary, "типы");
            }
            else { OrdersStatusText.Text = "Ошибка: Не удалось загрузить типы."; }

            var ordersResponse = await App.SupabaseClient.From<Заказы>().Select("*").Get();

            if (ordersResponse?.Models == null || !ordersResponse.Models.Any())
            {
                OrdersStatusText.Text = "История заказов пуста.";
                _allLoadedOrders = new List<OrderDisplayViewModel>();
                ApplyFilters();
                OrdersStatusText.Visibility = Visibility.Collapsed;
                return;
            }

            var orders = ordersResponse.Models;
            var clientIds = orders.Select(o => o.IdКлиента.ToString()).Distinct().ToList();

            if (clientIds.Any())
            {
                var usersResponse = await App.SupabaseClient.From<Пользователь>().Select("*").Filter("id", Operator.In, clientIds).Get();
                if (usersResponse?.Models != null)
                    _clientNamesDictionary = usersResponse.Models.ToDictionary(u => u.Id, u => u.ПолноеИмя ?? "Имя не указано");
            }

            var viewModels = orders.Select(order => new OrderDisplayViewModel
            {
                Id = order.Id,
                НомерЗаказа = order.НомерЗаказа ?? "-",
                ИмяКлиента = _clientNamesDictionary.TryGetValue(order.IdКлиента, out string clientName) ? clientName : "Клиент (?)",
                НазваниеСтатуса = _statusNamesDictionary.TryGetValue(order.IdСтатуса, out string statusName) ? statusName : "Статус (?)",
                НазваниеТипа = _typeNamesDictionary.TryGetValue(order.IdТипа, out string typeName) ? typeName : "Тип (?)",
                АдресДоставки = order.АдресДоставки ?? "---",
                ИтоговаяСумма = order.ИтоговаяСумма ,
                ВремяСоздания = order.ВремяСоздания,
                ВремяОбновления = order.ВремяОбновления,
                IdСтатуса = order.IdСтатуса,
                IdТипа = order.IdТипа
            }).ToList();

            var sortedViewModels = viewModels.OrderByDescending(vm => vm.ВремяСоздания).ToList();
            _allLoadedOrders = sortedViewModels;
            ApplyFilters();
            OrdersStatusText.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            OrdersStatusText.Text = $"Ошибка загрузки истории: {ex.Message}";
            MessageBox.Show($"Ошибка загрузки истории заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            _allLoadedOrders = new List<OrderDisplayViewModel>();
            ApplyFilters();
        }
    }

        private void PopulateFilterComboBox(ComboBox comboBox, Dictionary<Guid, string> dataSource, string typeName)
        {
            var items = new List<KeyValuePair<Guid, string>> {
                new KeyValuePair<Guid, string>(Guid.Empty, $"[Все {typeName}]")
            };
            items.AddRange(dataSource.OrderBy(kvp => kvp.Value).ToList());
            comboBox.ItemsSource = items;
            comboBox.SelectedIndex = 0;
        }

        private void HideAllPanels()
        {
            ProfilePanel.Visibility = Visibility.Collapsed;
            CurrentOrdersPanel.Visibility = Visibility.Collapsed;
            OrdersHistoryPanel.Visibility = Visibility.Collapsed;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string section)
            {
                switch (section)
                {
                    case "Профиль":
                        ShowProfile();
                        break;
                    case "ТекущиеЗаказы":
                        ShowCurrentOrders();
                        break;
                    case "ИсторияЗаказов":
                        ShowOrdersHistory();
                        break;
                }
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.SupabaseClient != null) await App.SupabaseClient.Auth.SignOut();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded && OrdersHistoryPanel.IsVisible)
            {
                ApplyFilters();
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded && OrdersHistoryPanel.IsVisible && e.AddedItems.Count > 0)
            {
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (_allLoadedOrders == null) return;

            IEnumerable<OrderDisplayViewModel> filteredList = _allLoadedOrders;

            if (StatusFilterComboBox.SelectedValue is Guid selectedStatusId && selectedStatusId != Guid.Empty)
            {
                filteredList = filteredList.Where(vm => vm.IdСтатуса == selectedStatusId);
            }
            if (TypeFilterComboBox.SelectedValue is Guid selectedTypeId && selectedTypeId != Guid.Empty)
            {
                filteredList = filteredList.Where(vm => vm.IdТипа == selectedTypeId);
            }

            string searchText = SearchTextBox.Text.Trim().ToLowerInvariant();
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

            OrdersDataGrid.ItemsSource = filteredList.ToList();

            if (!filteredList.Any() && _allLoadedOrders.Any())
            {
                OrdersStatusText.Text = "Заказы, соответствующие фильтрам, не найдены.";
                OrdersStatusText.Visibility = Visibility.Visible;
            }
            else if (!_allLoadedOrders.Any() && OrdersStatusText.Text == "История заказов пуста.")
            {
                OrdersStatusText.Visibility = Visibility.Visible;
            }
            else if (OrdersStatusText.Visibility == Visibility.Visible && OrdersStatusText.Text != "Загрузка данных истории..." && !OrdersStatusText.Text.StartsWith("Ошибка"))
            {
                OrdersStatusText.Visibility = Visibility.Collapsed;
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            bool changed = false;
            if (StatusFilterComboBox != null && StatusFilterComboBox.SelectedIndex != 0) { StatusFilterComboBox.SelectedIndex = 0; changed = true; }
            if (TypeFilterComboBox != null && TypeFilterComboBox.SelectedIndex != 0) { TypeFilterComboBox.SelectedIndex = 0; changed = true; }
            if (SearchTextBox != null && !string.IsNullOrEmpty(SearchTextBox.Text)) { SearchTextBox.Text = string.Empty; changed = true; }
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox != null && !string.IsNullOrEmpty(SearchTextBox.Text))
            {
                SearchTextBox.Text = string.Empty; // TextChanged вызовет ApplyFilters
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid orderId)
            {
                OrderDetailsWindow detailsWindow = new OrderDetailsWindow(orderId);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
        }
    }
}