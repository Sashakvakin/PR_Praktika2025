using System;
using System.Collections.Generic;
//using System.ComponentModel; // Больше не нужен
using System.Globalization;
using System.Linq;
//using System.Runtime.CompilerServices; // Больше не нужен
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChickenAndPoint.Models;
using static Postgrest.Constants;

namespace ChickenAndPoint
{
    public class OrderDisplayViewModel
    {
        public Guid Id { get; set; }
        public string ИмяКлиента { get; set; }
        public string НазваниеСтатуса { get; set; }
        public string НазваниеТипа { get; set; }
        public string АдресДоставки { get; set; }
        public decimal? ИтоговаяСумма { get; set; }
        public DateTimeOffset ВремяСоздания { get; set; }
        public DateTimeOffset ВремяОбновления { get; set; }
    }

    public partial class SotrydnikMainWindow : Window // Убрано: INotifyPropertyChanged
    {
        private Пользователь _loggedInUser;
        private Dictionary<Guid, string> _clientNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _statusNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _typeNamesDictionary = new Dictionary<Guid, string>();
        private List<OrderDisplayViewModel> _allLoadedOrders = new List<OrderDisplayViewModel>();

        public SotrydnikMainWindow(Пользователь user)
        {
            InitializeComponent();
            _loggedInUser = user;
            ShowProfile();
        }

        private void ShowProfile()
        {
            HideAllPanels();
            ProfilePanel.Visibility = Visibility.Visible;
            LoadProfileData();
        }

        private async void ShowOrders()
        {
            HideAllPanels();
            OrdersPanel.Visibility = Visibility.Visible;
            await LoadOrdersWithDetailsAsync();
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

        private async Task LoadOrdersWithDetailsAsync()
        {
            OrdersStatusText.Text = "Загрузка данных...";
            OrdersStatusText.Visibility = Visibility.Visible;
            OrdersDataGrid.ItemsSource = null; // Очищаем перед загрузкой
            _allLoadedOrders.Clear();
            _clientNamesDictionary.Clear();
            _statusNamesDictionary.Clear();
            _typeNamesDictionary.Clear();

            try
            {
                if (App.SupabaseClient == null)
                {
                    OrdersStatusText.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    return;
                }

                var statusesResponse = await App.SupabaseClient.From<СтатусЗаказа>().Select("*").Get();
                if (statusesResponse?.Models != null)
                    _statusNamesDictionary = statusesResponse.Models.ToDictionary(s => s.Id, s => s.НазваниеСтатуса ?? "?");
                else OrdersStatusText.Text = "Ошибка: Не удалось загрузить статусы.";

                var typesResponse = await App.SupabaseClient.From<ТипЗаказа>().Select("*").Get();
                if (typesResponse?.Models != null)
                    _typeNamesDictionary = typesResponse.Models.ToDictionary(t => t.Id, t => t.НазваниеТипа ?? "?");
                else OrdersStatusText.Text = "Ошибка: Не удалось загрузить типы.";

                var ordersResponse = await App.SupabaseClient.From<Заказы>().Select("*").Get();

                if (ordersResponse?.Models == null || !ordersResponse.Models.Any())
                {
                    OrdersStatusText.Text = "Заказов нет.";
                    _allLoadedOrders = new List<OrderDisplayViewModel>();
                    OrdersDataGrid.ItemsSource = _allLoadedOrders; // Показываем пустую таблицу
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
                    ИмяКлиента = _clientNamesDictionary.TryGetValue(order.IdКлиента, out string clientName) ? clientName : "Клиент (?)",
                    НазваниеСтатуса = _statusNamesDictionary.TryGetValue(order.IdСтатуса, out string statusName) ? statusName : "Статус (?)",
                    НазваниеТипа = _typeNamesDictionary.TryGetValue(order.IdТипа, out string typeName) ? typeName : "Тип (?)",
                    АдресДоставки = order.АдресДоставки,
                    ИтоговаяСумма = order.ИтоговаяСумма,
                    ВремяСоздания = order.ВремяСоздания,
                    ВремяОбновления = order.ВремяОбновления
                }).ToList();

                var sortedViewModels = viewModels.OrderByDescending(vm => vm.ВремяСоздания).ToList();
                _allLoadedOrders = sortedViewModels; // Сохраняем полный список
                OrdersDataGrid.ItemsSource = _allLoadedOrders; // Устанавливаем ItemsSource напрямую
                OrdersStatusText.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                OrdersStatusText.Text = $"Ошибка загрузки данных: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HideAllPanels()
        {
            ProfilePanel.Visibility = Visibility.Collapsed;
            OrdersPanel.Visibility = Visibility.Collapsed;
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
                    case "Заказы":
                        ShowOrders();
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
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            if (_allLoadedOrders == null) return;

            string searchText = SearchTextBox.Text.Trim().ToLowerInvariant();
            List<OrderDisplayViewModel> filteredList;

            if (string.IsNullOrEmpty(searchText))
            {
                filteredList = _allLoadedOrders;
            }
            else
            {
                filteredList = _allLoadedOrders.Where(vm =>
                    (vm.ИмяКлиента?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.НазваниеСтатуса?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.НазваниеТипа?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.АдресДоставки?.ToLowerInvariant().Contains(searchText) ?? false) ||
                    (vm.ИтоговаяСумма?.ToString("F2", CultureInfo.InvariantCulture).Contains(searchText) ?? false) ||
                    (vm.ВремяСоздания.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture).Contains(searchText)) ||
                    (vm.ВремяОбновления.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture).Contains(searchText))
                ).ToList();
            }

            // Устанавливаем ItemsSource напрямую
            OrdersDataGrid.ItemsSource = filteredList;
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }
        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid orderId)
            {
                OrderDetailsWindow detailsWindow = new OrderDetailsWindow(orderId);
                detailsWindow.Owner = this; // Делаем главное окно владельцем
                detailsWindow.ShowDialog(); // Открываем как модальное окно
            }
        }
    }
}