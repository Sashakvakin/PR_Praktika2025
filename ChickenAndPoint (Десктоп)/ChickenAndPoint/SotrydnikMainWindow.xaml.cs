using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ChickenAndPoint.Models;
using static Postgrest.Constants;
using Supabase.Realtime.Interfaces;
using System.Threading;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace ChickenAndPoint
{

    public class SubtractValueConverter : IValueConverter
    {
        private const double MinimumDateFontSize = 10.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double sliderValue)
            {
                double subtractAmount = 0;
                if (parameter is string paramString && double.TryParse(paramString, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount))
                {
                    subtractAmount = amount;
                }
                else if (parameter is double paramDouble)
                {
                    subtractAmount = paramDouble;
                }

                double rawResult = sliderValue - subtractAmount;
                double finalResult = Math.Max(MinimumDateFontSize, rawResult);
                return finalResult;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented for SubtractValueConverter.");
        }

    }
    public class ItemCountToColumnsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = 0;

            if (value is int intCount)
            {
                count = intCount;
            }
            else if (value is ICollection collection)
            {
                count = collection.Count;
            }
            if (count > 15)
                return 4;
            if (count > 10)
                return 3;
            if (count > 5)
                return 2;
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MinimumValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minimumValue = 0;

            if (parameter != null)
            {
                if (parameter is string paramString && double.TryParse(paramString, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedMin))
                {
                    minimumValue = parsedMin;
                }
                else if (parameter is double paramDouble)
                {
                    minimumValue = paramDouble;
                }
                else if (parameter is int paramInt)
                {
                    minimumValue = paramInt;
                }
            }

            if (value is double currentValue)
            {
                return Math.Max(currentValue, minimumValue);
            }

            return minimumValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OrderTypeDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string typeName)
            {
                if (typeName.Equals("Самовывоз с упаковкой", StringComparison.OrdinalIgnoreCase))
                {
                    return "Упаковать";
                }
                return typeName;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
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
    }

    public class OrderItemDisplayViewModel : INotifyPropertyChanged
    {
        private string _названиеБлюда;
        private int _количество;
        private decimal _ценаНаМоментЗаказа;
        private string _ссылкаНаИзображение;
        private Guid _idКатегории;
        private string _ссылкаНаИконкуКатегории;

        public string НазваниеБлюда
        {
            get => _названиеБлюда;
            set => SetProperty(ref _названиеБлюда, value);
        }
        public int Количество
        {
            get => _количество;
            set => SetProperty(ref _количество, value);
        }
        public decimal ЦенаНаМоментЗаказа
        {
            get => _ценаНаМоментЗаказа;
            set => SetProperty(ref _ценаНаМоментЗаказа, value);
        }
        public string СсылкаНаИзображение
        {
            get => _ссылкаНаИзображение;
            set => SetProperty(ref _ссылкаНаИзображение, value);
        }
        public decimal СуммаПозиции => Количество * ЦенаНаМоментЗаказа;

        public Guid IdКатегории
        {
            get => _idКатегории;
            set => SetProperty(ref _idКатегории, value);
        }
        public string СсылкаНаИконкуКатегории
        {
            get => _ссылкаНаИконкуКатегории;
            set => SetProperty(ref _ссылкаНаИконкуКатегории, value);
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
            if (propertyName == nameof(Количество) || propertyName == nameof(ЦенаНаМоментЗаказа))
            {
                OnPropertyChanged(nameof(СуммаПозиции));
            }
            return true;
        }
    }

    public class CurrentOrderViewModel : INotifyPropertyChanged
    {
        private Guid _id;
        private string _номерЗаказа;
        private ObservableCollection<OrderItemDisplayViewModel> _items;
        private DateTimeOffset _времяСоздания;
        private DateTimeOffset _времяОбновления;
        private Guid _idТипа;
        private string _названиеТипа;
        private Guid _idСтатуса;

        public Guid Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string НомерЗаказа
        {
            get => _номерЗаказа;
            set => SetProperty(ref _номерЗаказа, value);
        }

        public ObservableCollection<OrderItemDisplayViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public DateTimeOffset ВремяСоздания
        {
            get => _времяСоздания;
            set => SetProperty(ref _времяСоздания, value);
        }

        public DateTimeOffset ВремяОбновления
        {
            get => _времяОбновления;
            set => SetProperty(ref _времяОбновления, value);
        }

        public Guid IdТипа
        {
            get => _idТипа;
            set => SetProperty(ref _idТипа, value);
        }

        public string НазваниеТипа
        {
            get => _названиеТипа;
            set => SetProperty(ref _названиеТипа, value);
        }

        public Guid IdСтатуса
        {
            get => _idСтатуса;
            set => SetProperty(ref _idСтатуса, value);
        }

        public CurrentOrderViewModel()
        {
            Items = new ObservableCollection<OrderItemDisplayViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void UpdateItems(IEnumerable<OrderItemDisplayViewModel> freshItems)
        {
            var freshItemsList = freshItems?.ToList() ?? new List<OrderItemDisplayViewModel>();
            var currentItemsList = this.Items.ToList(); // Копия текущих

            bool collectionChanged = false; // Флаг, что были изменения

            // 1. Удалить те, которых нет в свежих
            var itemsToRemove = currentItemsList
                .Where(currentItem => !freshItemsList.Any(freshItem => AreItemsEqual(currentItem, freshItem)))
                .ToList();

            if (itemsToRemove.Any())
            {
                foreach (var itemToRemove in itemsToRemove)
                {
                    this.Items.Remove(itemToRemove);
                }
                collectionChanged = true;
            }


            // 2. Обновить существующие и добавить новые
            foreach (var freshItem in freshItemsList)
            {
                var existingItem = currentItemsList.FirstOrDefault(currentItem => AreItemsEqual(currentItem, freshItem));
                if (existingItem != null)
                {
                    // Обновляем свойства существующего, если они изменились
                    // (INotifyPropertyChanged внутри OrderItemDisplayViewModel позаботится об обновлении UI для этого элемента)
                    if (existingItem.Количество != freshItem.Количество) { existingItem.Количество = freshItem.Количество; }
                    if (existingItem.ЦенаНаМоментЗаказа != freshItem.ЦенаНаМоментЗаказа) { existingItem.ЦенаНаМоментЗаказа = freshItem.ЦенаНаМоментЗаказа; }
                }
                else
                {
                    // Добавляем новый
                    this.Items.Add(freshItem);
                    collectionChanged = true; // Добавление - это тоже изменение
                }
            }

            // 3. ЯВНО УВЕДОМЛЯЕМ ОБ ИЗМЕНЕНИИ КОЛЛЕКЦИИ ITEMS
            // Это заставит Binding для UniformGrid.Columns пересчитаться
            if (collectionChanged)
            {
                OnPropertyChanged(nameof(Items));
            }

            // Опциональная сортировка, если нужна
            // ...
        }

        private bool AreItemsEqual(OrderItemDisplayViewModel item1, OrderItemDisplayViewModel item2)
        {
            return item1 != null && item2 != null &&
                   item1.НазваниеБлюда == item2.НазваниеБлюда;
        }
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
    public class StatusToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid statusId)
            {
                if (statusId == SotrydnikMainWindow.ReadyForPickupStatusUUID)
                {
                    return "Получен клиентом";
                }
                else if (statusId == SotrydnikMainWindow.AwaitingCourierStatusUUID)
                {
                    return "Передан курьеру";
                }
                else if (statusId == SotrydnikMainWindow.AcceptedStatusUUID)
                {
                    return "Готов";
                }
                else if (statusId == SotrydnikMainWindow.PreparingStatusUUID)
                {
                    return "Готов";
                }
                else
                {
                    return "Неизвестный статус";
                }
            }
            return "Неизвестный статус";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class SotrydnikMainWindow : Window
    {
        private Timer _refreshDebounceTimer;
        private bool _isRefreshQueued = false;
        private readonly object _refreshLock = new object();

        private Пользователь _loggedInUser;
        private ObservableCollection<CurrentOrderViewModel> _currentOrders = new ObservableCollection<CurrentOrderViewModel>();
        private IRealtimeChannel _ordersChannel;
        private IRealtimeChannel _orderItemsChannel;

        private Dictionary<Guid, string> _clientNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _statusNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _typeNamesDictionary = new Dictionary<Guid, string>();
        private List<OrderDisplayViewModel> _allLoadedOrders = new List<OrderDisplayViewModel>();
        private const string AllItemsFilterKey = "[Все]";

        public static readonly Guid DeliveryTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["DeliveryTypeUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid PickupTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["PickupTypeUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid PackagingPickupTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["PackagingPickupTypeUUID"] ?? Guid.Empty.ToString());

        public static readonly Guid ReadyForPickupStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["ReadyForPickupStatusUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid AwaitingCourierStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["AwaitingCourierStatusUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid PreparingStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["PreparingStatusUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid AcceptedStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["AcceptedStatusUUID"] ?? Guid.Empty.ToString());

        private List<Guid> _currentOrderStatusGuids = new List<Guid>();
        private Dictionary<Guid, Блюда> _allDishesDictionaryCache = new Dictionary<Guid, Блюда>();
        private Dictionary<Guid, Категории> _allCategoriesDictionaryCache = new Dictionary<Guid, Категории>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public ObservableCollection<CurrentOrderViewModel> CurrentOrders
        {
            get { return _currentOrders; }
            set { _currentOrders = value; }
        }
        private void QueueOrTriggerRefresh()
        {
            lock (_refreshLock)
            {
                if (_isRefreshQueued)
                {
                    return;
                }
                _isRefreshQueued = true;
                _refreshDebounceTimer?.Dispose();
                _refreshDebounceTimer = new Timer(async (_) =>
                {
                    try
                    {
                        await LoadCurrentOrdersAsync();
                    }
                    finally
                    {
                        lock (_refreshLock)
                        {
                            _isRefreshQueued = false;
                        }
                        _refreshDebounceTimer?.Dispose();
                        _refreshDebounceTimer = null;
                    }
                }, null, 500, Timeout.Infinite);
            }
        }
        public SotrydnikMainWindow(Пользователь user)
        {
            InitializeComponent();
            _loggedInUser = user;
            DataContext = this; // Устанавливаем DataContext
            InitializeStatusGuids();
            CurrentOrdersItemsControl.ItemsSource = _currentOrders;

            ShowProfile();

            Loaded += SotrydnikMainWindow_Loaded;
            Closed += SotrydnikMainWindow_Closed;
        }

        private void InitializeStatusGuids()
        {
            _currentOrderStatusGuids.Clear();
            if (AcceptedStatusUUID != Guid.Empty) _currentOrderStatusGuids.Add(AcceptedStatusUUID);
            if (PreparingStatusUUID != Guid.Empty) _currentOrderStatusGuids.Add(PreparingStatusUUID);
            if (ReadyForPickupStatusUUID != Guid.Empty) _currentOrderStatusGuids.Add(ReadyForPickupStatusUUID);
            if (AwaitingCourierStatusUUID != Guid.Empty) _currentOrderStatusGuids.Add(AwaitingCourierStatusUUID);
        }

        private async void SotrydnikMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await CacheDishesAndCategoriesAsync();
            InitializeRealtimeOrdersSubscription();
            await LoadCurrentOrdersAsync();
        }

        private async Task CacheDishesAndCategoriesAsync()
        {
            try
            {
                if (App.SupabaseClient == null) return;

                var dishesTask = App.SupabaseClient.From<Блюда>().Select("*").Get();
                var categoriesTask = App.SupabaseClient.From<Категории>().Select("*").Get();
                var typesTask = App.SupabaseClient.From<ТипЗаказа>().Select("*").Get();

                await Task.WhenAll(dishesTask, categoriesTask, typesTask);

                _allDishesDictionaryCache = dishesTask.Result?.Models?.ToDictionary(d => d.Id, d => d) ?? new Dictionary<Guid, Блюда>();
                _allCategoriesDictionaryCache = categoriesTask.Result?.Models?.ToDictionary(c => c.Id, c => c) ?? new Dictionary<Guid, Категории>();
                _typeNamesDictionary = typesTask.Result?.Models?.ToDictionary(t => t.Id, t => t.НазваниеТипа ?? "?") ?? new Dictionary<Guid, string>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка кэширования данных: {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void InitializeRealtimeOrdersSubscription()
        {
            if (App.SupabaseClient == null)
            {
                return;
            }

            try
            {
                if (_currentOrderStatusGuids.Count > 0)
                {
                    _ordersChannel = App.SupabaseClient.Realtime.Channel("realtime", "public", "Заказы");
                    if (_ordersChannel != null)
                    {
                        _ordersChannel.AddPostgresChangeHandler(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, (sender, change) =>
                        {
                            QueueOrTriggerRefresh();
                        });

                        _ordersChannel.AddPostgresChangeHandler(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Updates, async (sender, change) =>
                        {
                            if (change.Payload?.Data?.Record != null)
                            {
                                try
                                {
                                    string jsonData = JsonConvert.SerializeObject(change.Payload.Data.Record);
                                    var updatedOrder = JsonConvert.DeserializeObject<Заказы>(jsonData);
                                    if (updatedOrder != null)
                                    {
                                        QueueOrTriggerRefresh();
                                    }
                                }
                                catch (Exception)
                                {
                                    QueueOrTriggerRefresh();
                                }
                            }
                            else
                            {
                                QueueOrTriggerRefresh();
                            }
                        });

                        _ordersChannel.Subscribe(timeoutMs: 10000);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось подписаться на обновления заказов: {ex.Message}", "Ошибка Realtime", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            try
            {
                _orderItemsChannel = App.SupabaseClient.Realtime.Channel("realtime", "public", "СоставЗаказа");
                if (_orderItemsChannel != null)
                {
                    _orderItemsChannel.AddPostgresChangeHandler(Supabase.Realtime.PostgresChanges.PostgresChangesOptions.ListenType.Inserts, async (sender, change) =>
                    {
                        if (change.Payload?.Data?.Record != null)
                        {
                            try
                            {
                                string jsonData = JsonConvert.SerializeObject(change.Payload.Data.Record);
                                var newOrderItem = JsonConvert.DeserializeObject<СоставЗаказа>(jsonData);
                                if (newOrderItem != null)
                                {
                                    QueueOrTriggerRefresh();
                                }
                            }
                            catch (Exception)
                            {
                                QueueOrTriggerRefresh();
                            }
                        }
                        else
                        {
                            QueueOrTriggerRefresh();
                        }
                    });

                    _orderItemsChannel.Subscribe(timeoutMs: 10000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось подписаться на обновления состава заказов: {ex.Message}", "Ошибка Realtime", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SotrydnikMainWindow_Closed(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            if (_ordersChannel != null)
            {
                try { _ordersChannel.Unsubscribe(); } catch (Exception) { }
                _ordersChannel = null;
            }

            if (_orderItemsChannel != null)
            {
                try { _orderItemsChannel.Unsubscribe(); } catch (Exception) { }
                _orderItemsChannel = null;
            }

            lock (_refreshLock)
            {
                _refreshDebounceTimer?.Dispose();
                _refreshDebounceTimer = null;
            }
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

        private async void OrderActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.Tag is Guid orderId)) return;

            button.IsEnabled = false;

            CurrentOrderViewModel orderToUpdate = null;
            await Dispatcher.InvokeAsync(() => {
                orderToUpdate = _currentOrders.FirstOrDefault(x => x.Id == orderId);
            });


            if (orderToUpdate == null)
            {
                MessageBox.Show("Не удалось найти данные заказа для обновления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(() => button.IsEnabled = true);
                return;
            }

            Guid currentStatusGuid = orderToUpdate.IdСтатуса;
            Guid orderTypeGuid = orderToUpdate.IdТипа;

            try
            {
                if (!Guid.TryParse(ConfigurationManager.AppSettings["CompletedStatusUUID"], out Guid completedGuid) ||
                    !Guid.TryParse(ConfigurationManager.AppSettings["InTransitStatusUUID"], out Guid inTransitGuid) ||
                    !Guid.TryParse(ConfigurationManager.AppSettings["ReadyForPickupStatusUUID"], out Guid readyForPickupGuid) ||
                    !Guid.TryParse(ConfigurationManager.AppSettings["AwaitingCourierStatusUUID"], out Guid awaitingCourierGuid))
                {
                    MessageBox.Show($"Ошибка чтения GUID статусов/типов из конфигурации.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                    Dispatcher.Invoke(() => button.IsEnabled = true);
                    return;
                }

                Guid targetStatusGuid = Guid.Empty;
                string targetStatusName = string.Empty;

                if (currentStatusGuid == ReadyForPickupStatusUUID) { targetStatusGuid = completedGuid; targetStatusName = "Выполнен"; }
                else if (currentStatusGuid == AwaitingCourierStatusUUID) { targetStatusGuid = inTransitGuid; targetStatusName = "В пути"; }
                else if (currentStatusGuid == AcceptedStatusUUID || currentStatusGuid == PreparingStatusUUID)
                {
                    if (orderTypeGuid == DeliveryTypeUUID) { targetStatusGuid = awaitingCourierGuid; targetStatusName = "Ожидает курьера"; }
                    else { targetStatusGuid = readyForPickupGuid; targetStatusName = "Готов к выдаче"; }
                }
                else
                {
                    _statusNamesDictionary.TryGetValue(currentStatusGuid, out string currentStatusName);
                    MessageBox.Show($"Для заказа со статусом '{currentStatusName ?? currentStatusGuid.ToString()}' действие не определено.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Dispatcher.Invoke(() => button.IsEnabled = true);
                    return;
                }

                if (targetStatusGuid != Guid.Empty)
                {
                    var updateResponse = await App.SupabaseClient
                       .From<Заказы>()
                       .Where(x => x.Id == orderId)
                       .Set(x => x.IdСтатуса, targetStatusGuid)
                       .Update();

                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {

                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при обновлении статуса заказа: {updateResponse.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при обработке действия с заказом: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Dispatcher.Invoke(() => button.IsEnabled = true);
            }
        }

        private async void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid orderId)
            {
                try
                {
                    string deliveryTypeUUID = ConfigurationManager.AppSettings["DeliveryTypeUUID"];
                    if (!Guid.TryParse(deliveryTypeUUID, out Guid deliveryGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для DeliveryTypeUUID: {deliveryTypeUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string awaitingCourierStatusUUID = ConfigurationManager.AppSettings["AwaitingCourierStatusUUID"];
                    string readyForPickupStatusUUID = ConfigurationManager.AppSettings["ReadyForPickupStatusUUID"];

                    if (!Guid.TryParse(awaitingCourierStatusUUID, out Guid awaitingCourierGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для AwaitingCourierStatusUUID: {awaitingCourierStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!Guid.TryParse(readyForPickupStatusUUID, out Guid readyForPickupGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для ReadyForPickupStatusUUID: {readyForPickupStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    CurrentOrderViewModel orderToComplete = CurrentOrders.FirstOrDefault(x => x.Id == orderId);
                    if (orderToComplete == null)
                    {
                        MessageBox.Show("Заказ не найден в списке текущих заказов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    Guid targetStatusGuid = (orderToComplete.IdТипа == deliveryGuid) ? awaitingCourierGuid : readyForPickupGuid;
                    string targetStatusName = (orderToComplete.IdТипа == deliveryGuid) ? "Ожидает курьера" : "Готов к выдаче";

                    var update = new Заказы
                    {
                        Id = orderId,
                        IdСтатуса = targetStatusGuid
                    };

                    var updateResponse = await App.SupabaseClient
                        .From<Заказы>()
                        .Where(x => x.Id == orderId)
                        .Set(x => x.IdСтатуса, targetStatusGuid)
                        .Update();

                    if (updateResponse.ResponseMessage.IsSuccessStatusCode)
                    {
                        await LoadCurrentOrdersAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка при обновлении статуса заказа: {updateResponse.ResponseMessage.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async Task LoadCurrentOrdersAsync()
        {
            List<CurrentOrderViewModel> freshViewModels = new List<CurrentOrderViewModel>();

            try
            {
                if (App.SupabaseClient == null)
                {
                    MessageBox.Show("Ошибка: Клиент Supabase не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_currentOrderStatusGuids.Count == 0)
                {
                    return;
                }

                if (_allDishesDictionaryCache == null || !_allDishesDictionaryCache.Any() ||
                    _allCategoriesDictionaryCache == null || !_allCategoriesDictionaryCache.Any() ||
                    _typeNamesDictionary == null || !_typeNamesDictionary.Any())
                {
                    await CacheDishesAndCategoriesAsync();
                    if (_allDishesDictionaryCache == null || !_allDishesDictionaryCache.Any() ||
                        _allCategoriesDictionaryCache == null || !_allCategoriesDictionaryCache.Any() ||
                        _typeNamesDictionary == null || !_typeNamesDictionary.Any())
                    {
                        MessageBox.Show("Ошибка загрузки справочников. Данные могут отображаться не полностью.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                var ordersTask = App.SupabaseClient.From<Заказы>().Select("*").Get(_cancellationTokenSource.Token);
                var itemsTask = App.SupabaseClient.From<СоставЗаказа>().Select("*").Get(_cancellationTokenSource.Token);

                await Task.WhenAll(ordersTask, itemsTask);

                if (_cancellationTokenSource.Token.IsCancellationRequested) { return; }

                var ordersResponse = ordersTask.Result;
                var allOrderItemsResponse = itemsTask.Result;

                if (ordersResponse?.Models == null) { return; }
                if (!ordersResponse.Models.Any()) { await Dispatcher.InvokeAsync(() => _currentOrders.Clear()); return; }

                var filteredAndSortedOrders = ordersResponse.Models
                    .Where(order => _currentOrderStatusGuids.Contains(order.IdСтатуса))
                    .OrderBy(o => o.ВремяСоздания)
                    .ToList();

                Dictionary<Guid, List<СоставЗаказа>> orderItemsByOrderId = new Dictionary<Guid, List<СоставЗаказа>>();
                if (allOrderItemsResponse?.Models != null)
                {
                    orderItemsByOrderId = allOrderItemsResponse.Models
                        .GroupBy(item => item.IdЗаказа)
                        .ToDictionary(group => group.Key, group => group.ToList());
                }

                foreach (var order in filteredAndSortedOrders)
                {
                    var freshViewModel = new CurrentOrderViewModel
                    {
                        Id = order.Id,
                        НомерЗаказа = order.НомерЗаказа ?? "Нет номера",
                        ВремяСоздания = order.ВремяСоздания,
                        ВремяОбновления = order.ВремяОбновления,
                        IdТипа = order.IdТипа,
                        IdСтатуса = order.IdСтатуса,
                        НазваниеТипа = _typeNamesDictionary.TryGetValue(order.IdТипа, out string typeName) ? typeName : "Тип (?)"
                    };

                    if (orderItemsByOrderId.TryGetValue(order.Id, out List<СоставЗаказа> currentOrderItemsList))
                    {
                        foreach (var item in currentOrderItemsList)
                        {
                            string dishName = "Блюдо (?)"; Guid categoryId = Guid.Empty; string iconUrl = null;
                            if (_allDishesDictionaryCache.TryGetValue(item.IdБлюда, out Блюда dish))
                            {
                                dishName = dish.НазваниеБлюда ?? dishName; categoryId = dish.IdКатегории;
                                if (_allCategoriesDictionaryCache.TryGetValue(categoryId, out Категории category)) { iconUrl = category.СсылкаНаИконку; }
                            }
                            freshViewModel.Items.Add(new OrderItemDisplayViewModel
                            {
                                НазваниеБлюда = dishName,
                                Количество = item.Количество,
                                ЦенаНаМоментЗаказа = item.ЦенаНаМоментЗаказа,
                                IdКатегории = categoryId,
                                СсылкаНаИконкуКатегории = iconUrl
                            });
                        }
                    }

                    if (freshViewModel.Items.Any())
                    {
                        freshViewModels.Add(freshViewModel);
                    }
                }

                await Dispatcher.InvokeAsync(() =>
                {
                    var currentOrderIds = new HashSet<Guid>(_currentOrders.Select(vm => vm.Id));
                    var freshOrderIds = new HashSet<Guid>(freshViewModels.Select(vm => vm.Id));

                    var ordersToRemove = _currentOrders.Where(vm => !freshOrderIds.Contains(vm.Id)).ToList();
                    foreach (var vmToRemove in ordersToRemove)
                    {
                        _currentOrders.Remove(vmToRemove);
                    }

                    int insertIndex = 0;
                    foreach (var freshVm in freshViewModels)
                    {
                        var existingVm = _currentOrders.FirstOrDefault(vm => vm.Id == freshVm.Id);

                        if (existingVm != null)
                        {
                            bool needsUpdate = false;
                            if (existingVm.IdСтатуса != freshVm.IdСтатуса) { existingVm.IdСтатуса = freshVm.IdСтатуса; needsUpdate = true; }
                            if (existingVm.ВремяОбновления != freshVm.ВремяОбновления) { existingVm.ВремяОбновления = freshVm.ВремяОбновления; needsUpdate = true; }
                            if (existingVm.НомерЗаказа != freshVm.НомерЗаказа) { existingVm.НомерЗаказа = freshVm.НомерЗаказа; needsUpdate = true; }

                            existingVm.UpdateItems(freshVm.Items);

                            int currentIndex = _currentOrders.IndexOf(existingVm);
                            if (currentIndex != insertIndex && currentIndex >= 0)
                            {
                                _currentOrders.Move(currentIndex, insertIndex);
                            }
                            insertIndex++;
                        }
                        else
                        {
                            if (insertIndex >= _currentOrders.Count)
                            {
                                _currentOrders.Add(freshVm);
                            }
                            else
                            {
                                _currentOrders.Insert(insertIndex, freshVm);
                            }
                            insertIndex++;
                        }
                    }
                });

            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка полной загрузки текущих заказов:\n{ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                await Dispatcher.InvokeAsync(() => _currentOrders.Clear());
            }
            finally { }
        
        }

        private void ToggleFullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                //Скрываем левое меню
                LeftMenuPanel.Visibility = Visibility.Collapsed;

                ToggleFullScreenButton.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = {
                        new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,10,0) },
                        new TextBlock { Text = "Свернуть", VerticalAlignment = VerticalAlignment.Center }
                       }
                };
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                //Возвращаем видимость левого меню
                LeftMenuPanel.Visibility = Visibility.Visible;

                ToggleFullScreenButton.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = {
                        new MaterialDesignThemes.Wpf.PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Fullscreen, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0,0,10,0) },
                        new TextBlock { Text = "Во весь экран", VerticalAlignment = VerticalAlignment.Center }
                       }
                };
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
                    ИтоговаяСумма = order.ИтоговаяСумма,
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
                SearchTextBox.Text = string.Empty;
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