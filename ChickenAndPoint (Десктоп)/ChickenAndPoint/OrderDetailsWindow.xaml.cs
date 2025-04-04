using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChickenAndPoint.Models;
using static Postgrest.Constants;

namespace ChickenAndPoint
{
    public partial class OrderDetailsWindow : Window
    {
        private Guid _orderId;
        private Dictionary<Guid, Блюда> _allDishesDictionary = new Dictionary<Guid, Блюда>();
        string orderNumber = "???";

        public OrderDetailsWindow(Guid orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            WindowTitleTextBlock.Text = "Состав заказа";
            Loaded += OrderDetailsWindow_Loaded;
        }

        private async void OrderDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrderItemsAsync();
        }

        private async Task LoadOrderItemsAsync()
        {
            StatusTextBlock.Text = "Загрузка данных...";
            StatusTextBlock.Visibility = Visibility.Visible;
            OrderItemsItemsControl.ItemsSource = null;
            TotalSumTextBlock.Visibility = Visibility.Collapsed;
            List<OrderItemDisplayViewModel> viewModels = new List<OrderItemDisplayViewModel>();
            _allDishesDictionary.Clear();

            try
            {
                if (App.SupabaseClient == null)
                {
                    StatusTextBlock.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                    return;
                }

                try
                {
                    var orderHeaderResponse = await App.SupabaseClient
                            .From<Заказы>()
                            .Select("НомерЗаказа")
                            .Filter("id", Operator.Equals, _orderId.ToString())
                            .Single();
                    if (orderHeaderResponse != null && !string.IsNullOrEmpty(orderHeaderResponse.НомерЗаказа))
                    {
                        orderNumber = orderHeaderResponse.НомерЗаказа;
                    }
                }
                catch (Exception orderNumEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Не удалось получить номер заказа: {orderNumEx.Message}");
                }

                WindowTitleTextBlock.Text = $"Состав заказа ({orderNumber})";

                var dishesResponse = await App.SupabaseClient
                    .From<Блюда>()
                    .Select("*")
                    .Get();

                if (dishesResponse?.Models != null)
                {
                    _allDishesDictionary = dishesResponse.Models.ToDictionary(d => d.Id, d => d);
                }
                else
                {
                    StatusTextBlock.Text = "Предупреждение: Не удалось загрузить справочник блюд.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                }

                var allOrderItemsResponse = await App.SupabaseClient
                    .From<СоставЗаказа>()
                    .Select("*")
                    .Get();

                if (allOrderItemsResponse?.Models == null)
                {
                    StatusTextBlock.Text = "Не удалось загрузить позиции заказов.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                    OrderItemsItemsControl.ItemsSource = viewModels;
                    return;
                }

                var relevantOrderItems = allOrderItemsResponse.Models
                                            .Where(item => item.IdЗаказа == _orderId)
                                            .ToList();

                if (!relevantOrderItems.Any())
                {
                    StatusTextBlock.Text = "В этом заказе нет позиций.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                    OrderItemsItemsControl.ItemsSource = viewModels;
                    return;
                }

                foreach (var item in relevantOrderItems)
                {
                    string dishName = "Блюдо (?)";
                    if (_allDishesDictionary.TryGetValue(item.IdБлюда, out Блюда dish))
                    {
                        dishName = dish.НазваниеБлюда ?? dishName;
                    }

                    viewModels.Add(new OrderItemDisplayViewModel
                    {
                        НазваниеБлюда = dishName,
                        Количество = item.Количество,
                        ЦенаНаМоментЗаказа = item.ЦенаНаМоментЗаказа
                    });
                }

                OrderItemsItemsControl.ItemsSource = viewModels;

                if (viewModels.Any())
                {
                    decimal totalSum = viewModels.Sum(vm => vm.СуммаПозиции);
                    TotalSumTextBlock.Text = $"Итого: {totalSum:N2} ₽";
                    TotalSumTextBlock.Visibility = Visibility.Visible;
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TotalSumTextBlock.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Ошибка загрузки состава заказа: {ex.Message}";
                if (ex.InnerException != null)
                {
                    StatusTextBlock.Text += $"\nДетали: {ex.InnerException.Message}";
                }
                StatusTextBlock.Visibility = Visibility.Visible;
                MessageBox.Show(StatusTextBlock.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                OrderItemsItemsControl.ItemsSource = new List<OrderItemDisplayViewModel>();
                TotalSumTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}