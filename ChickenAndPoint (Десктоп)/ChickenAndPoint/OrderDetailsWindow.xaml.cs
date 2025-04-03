using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ChickenAndPoint.Models;

namespace ChickenAndPoint
{

    public partial class OrderDetailsWindow : Window
    {
        private Guid _orderId;
        private Dictionary<Guid, Блюда> _allDishesDictionary = new Dictionary<Guid, Блюда>();
        string orderNumber;
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
            OrderItemsDataGrid.ItemsSource = null;
            List<OrderItemDisplayViewModel> viewModels = new List<OrderItemDisplayViewModel>();
            _allDishesDictionary.Clear();

            try
            {
                if (App.SupabaseClient == null)
                {
                    StatusTextBlock.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    return;
                }
                var orderHeaderResponse = await App.SupabaseClient
                        .From<Заказы>()
                        .Select("НомерЗаказа")
                        .Filter("id", Postgrest.Constants.Operator.Equals, _orderId.ToString())
                        .Single();
                if (orderHeaderResponse != null && !string.IsNullOrEmpty(orderHeaderResponse.НомерЗаказа))
                {
                    orderNumber = orderHeaderResponse.НомерЗаказа;
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
                }

                var allOrderItemsResponse = await App.SupabaseClient
                    .From<СоставЗаказа>()
                    .Select("*")
                    .Get();

                if (allOrderItemsResponse?.Models == null || !allOrderItemsResponse.Models.Any())
                {
                    StatusTextBlock.Text = "Позиции заказов не найдены.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                    OrderItemsDataGrid.ItemsSource = viewModels;
                    return;
                }

                var relevantOrderItems = allOrderItemsResponse.Models
                                            .Where(item => item.IdЗаказа == _orderId)
                                            .ToList();

                if (!relevantOrderItems.Any())
                {
                    StatusTextBlock.Text = "В этом заказе нет позиций.";
                    StatusTextBlock.Visibility = Visibility.Visible;
                    OrderItemsDataGrid.ItemsSource = viewModels;
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

                OrderItemsDataGrid.ItemsSource = viewModels;

                if (viewModels.Any())
                {
                    decimal totalSum = viewModels.Sum(vm => vm.СуммаПозиции);
                    TotalSumTextBlock.Text = $"Итого: {totalSum:N2} ₽";
                    TotalSumTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    TotalSumTextBlock.Visibility = Visibility.Collapsed;
                }

                StatusTextBlock.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Ошибка загрузки состава заказа: {ex.Message}";
                if (ex.InnerException is Postgrest.Exceptions.PostgrestException pgEx)
                {
                    StatusTextBlock.Text += $"\nДетали: {pgEx.Message}";
                }
                MessageBox.Show(StatusTextBlock.Text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}