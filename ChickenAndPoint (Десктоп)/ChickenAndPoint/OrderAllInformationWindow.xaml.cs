using ChickenAndPoint.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Postgrest.Constants;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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
        private Guid _currentOrderStatusId;


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
            CancelOrderButton.IsEnabled = false;

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

                _currentOrderStatusId = orderResponse.IdСтатуса;

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

                Guid cancelledId = Guid.Empty;
                Guid completedId = Guid.Empty;
                bool configReadOk = true;

                try
                {
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["CancelledStatusUUID"], out cancelledId))
                    {
                        MessageBox.Show("Не удалось прочитать CancelledStatusUUID из App.config.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Warning);
                        configReadOk = false;
                    }
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["CompletedStatusUUID"], out completedId))
                    {
                        MessageBox.Show("Не удалось прочитать CompletedStatusUUID из App.config.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception configEx)
                {
                    MessageBox.Show($"Ошибка чтения конфигурации статусов: {configEx.Message}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                    configReadOk = false;
                }

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

                if (configReadOk && cancelledId != Guid.Empty && (_currentOrderStatusId == cancelledId || (completedId != Guid.Empty && _currentOrderStatusId == completedId)))
                {
                    CancelOrderButton.IsEnabled = false;
                }
                else if (configReadOk && cancelledId != Guid.Empty)
                {
                    CancelOrderButton.IsEnabled = true;
                }
                else
                {
                    CancelOrderButton.IsEnabled = false;
                }

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


        private async void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            Guid targetStatusId = Guid.Empty;
            try
            {
                if (!Guid.TryParse(ConfigurationManager.AppSettings["CancelledStatusUUID"], out targetStatusId) || targetStatusId == Guid.Empty)
                {
                    MessageBox.Show("Не удалось прочитать ID статуса 'Отменен' из App.config. Отмена невозможна.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception configEx)
            {
                MessageBox.Show($"Ошибка чтения конфигурации статусов: {configEx.Message}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Guid completedId = Guid.Empty;
            Guid.TryParse(ConfigurationManager.AppSettings["CompletedStatusUUID"], out completedId);

            if (_currentOrderStatusId == targetStatusId || (completedId != Guid.Empty && _currentOrderStatusId == completedId))
            {
                MessageBox.Show("Заказ уже отменен или выполнен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                CancelOrderButton.IsEnabled = false;
                return;
            }


            MessageBoxResult confirm = MessageBox.Show($"Вы уверены, что хотите отменить заказ № {TitleTextBlock.Text.Split('№').LastOrDefault()?.Trim()}?",
                                                    "Подтверждение отмены", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            CancelOrderButton.IsEnabled = false;
            CloseButton.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                if (App.SupabaseClient == null) throw new InvalidOperationException("Клиент Supabase не инициализирован.");

                await App.SupabaseClient.From<Заказы>()
                          .Where(o => o.Id == _orderId)
                          .Set(o => o.IdСтатуса, targetStatusId)
                          .Update();

                MessageBox.Show("Заказ успешно отменен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                var newStatus = await App.SupabaseClient.From<СтатусЗаказа>().Select("название_статуса").Filter("id", Operator.Equals, targetStatusId.ToString()).Single();
                StatusTextBlock.Text = newStatus?.НазваниеСтатуса ?? "Отменен";
                _currentOrderStatusId = targetStatusId;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                if (_currentOrderStatusId != targetStatusId && !(completedId != Guid.Empty && _currentOrderStatusId == completedId))
                {
                    CancelOrderButton.IsEnabled = true;
                }
            }
            finally
            {
                CloseButton.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
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
}