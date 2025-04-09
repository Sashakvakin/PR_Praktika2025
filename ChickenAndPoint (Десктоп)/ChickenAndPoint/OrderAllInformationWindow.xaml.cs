using ChickenAndPoint.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Postgrest.Constants;

namespace ChickenAndPoint 
{
    public class OrderDisplayItemViewModel
    {
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal TotalItemPrice => Quantity * PricePerItem;
    }

    public partial class OrderAllInformationWindow : Window
    {
        private readonly Guid _orderId;

        public OrderAllInformationWindow(Guid orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            TitleTextBlock.Text = $"Заказ №... (ID: {_orderId})";
            Loaded += OrderAllInformationWindow_Loaded;
        }

        private async void OrderAllInformationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrderDataAsync();
        }

        private async Task LoadOrderDataAsync()
        {
            LoadingStatusText.Visibility = Visibility.Visible;
            OrderItemsList.ItemsSource = null;
            StatusTextBlock.Text = "-";
            CreationTimeRun.Text = "-";
            UpdateTimeRun.Text = "-";
            ClientNameTextBlock.Text = "-";
            ClientPhoneTextBlock.Text = "-";
            ClientEmailTextBlock.Text = "-";
            OrderTypeTextBlock.Text = "-";
            DeliveryAddressPanel.Visibility = Visibility.Collapsed;
            DeliveryAddressTextBlock.Text = "-";
            TotalSumTextBlock.Text = "- ₽";
            TitleTextBlock.Text = $"Заказ №... (ID: {_orderId})";

            try
            {
                if (App.SupabaseClient == null)
                {
                    LoadingStatusText.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    MessageBox.Show(LoadingStatusText.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var orderResponse = await App.SupabaseClient.From<Заказы>()
                                          .Select("*")
                                          .Filter("id", Operator.Equals, _orderId.ToString())
                                          .Single();

                if (orderResponse == null)
                {
                    LoadingStatusText.Text = "Ошибка: Заказ не найден.";
                    MessageBox.Show(LoadingStatusText.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var statusTask = App.SupabaseClient.From<СтатусЗаказа>().Select("*").Filter("id", Operator.Equals, orderResponse.IdСтатуса.ToString()).Single();
                var typeTask = App.SupabaseClient.From<ТипЗаказа>().Select("*").Filter("id", Operator.Equals, orderResponse.IdТипа.ToString()).Single();
                var clientTask = App.SupabaseClient.From<Пользователь>().Select("*").Filter("id", Operator.Equals, orderResponse.IdКлиента.ToString()).Single();
                var allItemsTask = App.SupabaseClient.From<СоставЗаказа>()
                                    .Select("*")
                                    .Get();

                await Task.WhenAll(statusTask, typeTask, clientTask, allItemsTask);

                var status = statusTask.Result;
                var type = typeTask.Result;
                var client = clientTask.Result;
                var allItemsResponse = allItemsTask.Result;

                TitleTextBlock.Text = $"Заказ № {orderResponse.НомерЗаказа ?? _orderId.ToString("N").Substring(0, 6)}";
                StatusTextBlock.Text = status?.НазваниеСтатуса ?? "Неизвестен";
                CreationTimeRun.Text = orderResponse.ВремяСоздания.ToString("dd.MM.yyyy HH:mm");
                UpdateTimeRun.Text = orderResponse.ВремяОбновления.ToString("dd.MM.yyyy HH:mm");
                ClientNameTextBlock.Text = client?.ПолноеИмя ?? "Имя не указано";
                ClientPhoneTextBlock.Text = client?.НомерТелефона ?? "-";
                ClientEmailTextBlock.Text = client?.Почта ?? "-";
                OrderTypeTextBlock.Text = type?.НазваниеТипа ?? "Неизвестен";

                if (!string.IsNullOrWhiteSpace(orderResponse.АдресДоставки))
                {
                    DeliveryAddressTextBlock.Text = orderResponse.АдресДоставки;
                    DeliveryAddressPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    DeliveryAddressPanel.Visibility = Visibility.Collapsed;
                }

                TotalSumTextBlock.Text = orderResponse.ИтоговаяСумма.HasValue
                                        ? orderResponse.ИтоговаяСумма.Value.ToString("N2", CultureInfo.InvariantCulture) + " ₽"
                                        : "- ₽";

                List<СоставЗаказа> relevantItems = new List<СоставЗаказа>();
                if (allItemsResponse?.Models != null)
                {
                    relevantItems = allItemsResponse.Models
                                       .Where(item => item.IdЗаказа == _orderId)
                                       .ToList();
                }

                Dictionary<Guid, Блюда> dishesDict = new Dictionary<Guid, Блюда>();
                if (relevantItems.Any())
                {
                    var dishIds = relevantItems.Select(i => i.IdБлюда).Distinct().ToList();
                    if (dishIds.Any())
                    {
                        var dishesResponse = await App.SupabaseClient.From<Блюда>()
                                                   .Select("*")
                                                   .Filter("id", Operator.In, dishIds.Select(id => id.ToString()).ToList())
                                                   .Get();
                        if (dishesResponse?.Models != null)
                        {
                            dishesDict = dishesResponse.Models.ToDictionary(d => d.Id, d => d);
                        }
                    }
                }

                List<OrderDisplayItemViewModel> itemVMs = new List<OrderDisplayItemViewModel>();
                if (relevantItems.Any())
                {
                    foreach (var item in relevantItems.OrderBy(i => dishesDict.TryGetValue(i.IdБлюда, out var d) ? d.НазваниеБлюда : "ЯЯЯ"))
                    {
                        itemVMs.Add(new OrderDisplayItemViewModel
                        {
                            DishName = dishesDict.TryGetValue(item.IdБлюда, out var dish) ? dish.НазваниеБлюда : "Блюдо (?)",
                            Quantity = item.Количество,
                            PricePerItem = item.ЦенаНаМоментЗаказа
                        });
                    }
                }

                OrderItemsList.ItemsSource = itemVMs;

                if (!itemVMs.Any())
                {
                    LoadingStatusText.Text = "Состав заказа пуст.";
                    LoadingStatusText.Visibility = Visibility.Visible;
                }
                else
                {
                    LoadingStatusText.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                LoadingStatusText.Text = $"Ошибка загрузки данных: {ex.Message}";
                LoadingStatusText.Visibility = Visibility.Visible;
                MessageBox.Show($"Произошла ошибка при загрузке данных заказа: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
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