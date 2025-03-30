using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ChickenAndPoint.Models;

namespace ChickenAndPoint
{
    public class OrderItemDisplayViewModel
    {
        public string НазваниеБлюда { get; set; }
        public int Количество { get; set; }
        public decimal ЦенаНаМоментЗаказа { get; set; }
        public decimal СуммаПозиции => Количество * ЦенаНаМоментЗаказа;
    }

    public partial class OrderDetailsWindow : Window
    {
        private Guid _orderId;
        private Dictionary<Guid, Блюда> _allDishesDictionary = new Dictionary<Guid, Блюда>();

        public OrderDetailsWindow(Guid orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            WindowTitleTextBlock.Text += orderId.ToString("D");
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