using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChickenAndPointMobile.Models;
using Xamarin.Forms;
using static Postgrest.Constants;

namespace ChickenAndPointMobile.ViewModels
{
    public class OrderDetailsViewModel : INotifyPropertyChanged
    {
        private Заказы _order;
        public Заказы Order { get => _order; set => SetProperty(ref _order, value); }

        private ObservableCollection<OrderDetailItemViewModel> _orderItems;
        public ObservableCollection<OrderDetailItemViewModel> OrderItems { get => _orderItems; set => SetProperty(ref _orderItems, value); }

        private string _statusName;
        public string StatusName { get => _statusName; set => SetProperty(ref _statusName, value); }

        private string _typeName;
        public string TypeName { get => _typeName; set => SetProperty(ref _typeName, value); }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        private string _loadingError;
        public string LoadingError { get => _loadingError; set => SetProperty(ref _loadingError, value); }
        public bool HasLoadingError => !string.IsNullOrEmpty(LoadingError);

        public string OrderNumberDisplay => Order?.НомерЗаказа ?? "Загрузка...";
        public string CreatedAtDisplay => Order?.ВремяСоздания.ToString("dd.MM.yyyy HH:mm") ?? "-";
        public string TotalSumDisplay => Order?.ИтоговаяСумма.HasValue ?? false ? $"{Order.ИтоговаяСумма.Value:N2} ₽" : "-";
        public bool HasDeliveryAddress => !string.IsNullOrWhiteSpace(Order?.АдресДоставки);

        public Command LoadDetailsCommand { get; }
        private Guid _orderId;

        private static Dictionary<Guid, string> _statusCache = new Dictionary<Guid, string>();
        private static Dictionary<Guid, string> _typeCache = new Dictionary<Guid, string>();
        private static Dictionary<Guid, Блюда> _dishesCache = new Dictionary<Guid, Блюда>();

        public OrderDetailsViewModel(Guid orderId)
        {
            _orderId = orderId;
            OrderItems = new ObservableCollection<OrderDetailItemViewModel>();
            LoadDetailsCommand = new Command(async () => await ExecuteLoadDetailsCommand());
        }

        public async Task ExecuteLoadDetailsCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            LoadingError = null;
            OrderItems.Clear();

            try
            {
                var orderResponse = await App.SupabaseClient.From<Заказы>()
                                          .Select("*")
                                          .Filter("id", Operator.Equals, _orderId.ToString())
                                          .Single();
                if (orderResponse == null) throw new Exception("Заказ не найден.");
                Order = orderResponse;

                var loadLookupsTask = LoadLookupsAsync(Order.IdСтатуса, Order.IdТипа);
                var loadItemsTask = App.SupabaseClient.From<СоставЗаказа>()
                                        .Select("id_блюда, количество, цена_на_момент_заказа")
                                        .Filter("id_заказа", Operator.Equals, _orderId.ToString())
                                        .Get();

                await Task.WhenAll(loadLookupsTask, loadItemsTask);

                var itemsResponse = loadItemsTask.Result;
                if (itemsResponse?.Models == null) throw new Exception("Не удалось загрузить состав заказа.");

                var neededDishIds = itemsResponse.Models.Select(i => i.IdБлюда).Distinct()
                                        .Where(id => !_dishesCache.ContainsKey(id)).ToList();
                if (neededDishIds.Any())
                {
                    var dishesResponse = await App.SupabaseClient.From<Блюда>()
                                                .Select("id, название_блюда")
                                                .Filter("id", Operator.In, neededDishIds.Select(id => id.ToString()).ToList())
                                                .Get();
                    if (dishesResponse?.Models != null)
                    {
                        foreach (var dish in dishesResponse.Models) _dishesCache[dish.Id] = dish;
                    }
                }

                foreach (var item in itemsResponse.Models.OrderBy(i => _dishesCache.TryGetValue(i.IdБлюда, out var d) ? d.НазваниеБлюда : string.Empty))
                {
                    OrderItems.Add(new OrderDetailItemViewModel
                    {
                        DishName = _dishesCache.TryGetValue(item.IdБлюда, out var dish) ? dish.НазваниеБлюда : "Блюдо неизвестно",
                        Quantity = item.Количество,
                        PriceAtOrder = item.ЦенаНаМоментЗаказа
                    });
                }

                StatusName = _statusCache.TryGetValue(Order.IdСтатуса, out var status) ? status : "??";
                TypeName = _typeCache.TryGetValue(Order.IdТипа, out var type) ? type : "??";
            }
            catch (Exception ex)
            {
                LoadingError = $"Ошибка загрузки деталей: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Order Details Loading Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadLookupsAsync(Guid statusId, Guid typeId)
        {
            List<Task> tasks = new List<Task>();
            if (!_statusCache.ContainsKey(statusId))
            {
                tasks.Add(App.SupabaseClient.From<СтатусЗаказа>().Select("id, название_статуса").Get().ContinueWith(t => {
                    if (t.Result?.Models != null) foreach (var s in t.Result.Models) _statusCache[s.Id] = s.НазваниеСтатуса ?? "?";
                }));
            }
            if (!_typeCache.ContainsKey(typeId))
            {
                tasks.Add(App.SupabaseClient.From<ТипЗаказа>().Select("id, название_типа").Get().ContinueWith(t => {
                    if (t.Result?.Models != null) foreach (var ty in t.Result.Models) _typeCache[ty.Id] = ty.НазваниеТипа ?? "?";
                }));
            }
            if (tasks.Any()) await Task.WhenAll(tasks);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            if (propertyName == nameof(LoadingError)) OnPropertyChanged(nameof(HasLoadingError));
            if (propertyName == nameof(Order))
            {
                OnPropertyChanged(nameof(OrderNumberDisplay));
                OnPropertyChanged(nameof(CreatedAtDisplay));
                OnPropertyChanged(nameof(TotalSumDisplay));
                OnPropertyChanged(nameof(HasDeliveryAddress));
            }
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}