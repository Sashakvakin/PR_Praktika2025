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
    public class MinimumValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minimumValue = 0; // Минимальное значение по умолчанию

            // Пытаемся получить минимальное значение из параметра конвертера
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
                // Добавим поддержку int на всякий случай
                else if (parameter is int paramInt)
                {
                    minimumValue = paramInt;
                }
            }

            // Пытаемся получить текущее значение от слайдера
            if (value is double currentValue)
            {
                // Возвращаем наибольшее из текущего значения и минимума
                return Math.Max(currentValue, minimumValue);
            }

            // Если значение не double, возвращаем просто минимум (запасной вариант)
            return minimumValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Для привязки размера обычно не нужен
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

    public class OrderItemDisplayViewModel
    {
        public string НазваниеБлюда { get; set; }
        public int Количество { get; set; }
        public decimal ЦенаНаМоментЗаказа { get; set; }
        public string СсылкаНаИзображение { get; set; }
        public decimal СуммаПозиции => Количество * ЦенаНаМоментЗаказа;

        public Guid IdКатегории { get; set; } // НОВОЕ
        public string СсылкаНаИконкуКатегории { get; set; } //НОВОЕ
    }

    public class CurrentOrderViewModel
    {
        public Guid Id { get; set; }
        public string НомерЗаказа { get; set; }
        public List<OrderItemDisplayViewModel> Items { get; set; }
        public DateTimeOffset ВремяСоздания { get; set; }
        public Guid IdТипа { get; set; }

        public Guid IdСтатуса { get; set; }
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
                    return "Заказ получен";
                }
                else if (statusId == SotrydnikMainWindow.AwaitingCourierStatusUUID)
                {
                    return "Передан курьеру";
                }
                else if (statusId == SotrydnikMainWindow.AcceptedStatusUUID)
                {
                    return "Готово";
                }
                else if (statusId == SotrydnikMainWindow.PreparingStatusUUID)
                {
                    return "Готово";
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
        private Пользователь _loggedInUser;
        private ObservableCollection<CurrentOrderViewModel> _currentOrders = new ObservableCollection<CurrentOrderViewModel>();

        private Dictionary<Guid, string> _clientNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _statusNamesDictionary = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _typeNamesDictionary = new Dictionary<Guid, string>();
        private List<OrderDisplayViewModel> _allLoadedOrders = new List<OrderDisplayViewModel>();
        private const string AllItemsFilterKey = "[Все]";

        public static readonly Guid DeliveryTypeUUID = Guid.Parse(ConfigurationManager.AppSettings["DeliveryTypeUUID"] ?? Guid.Empty.ToString());

        public static readonly Guid ReadyForPickupStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["ReadyForPickupStatusUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid AwaitingCourierStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["AwaitingCourierStatusUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid PreparingStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["PreparingStatusUUID"] ?? Guid.Empty.ToString());
        public static readonly Guid AcceptedStatusUUID = Guid.Parse(ConfigurationManager.AppSettings["AcceptedStatusUUID"] ?? Guid.Empty.ToString());

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

        private async void OrderActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid orderId)
            {
                try
                {
                    // Получаем текущий заказ из коллекции _currentOrders
                    CurrentOrderViewModel orderToComplete = CurrentOrders.FirstOrDefault(x => x.Id == orderId);
                    if (orderToComplete == null)
                    {
                        MessageBox.Show("Заказ не найден в списке текущих заказов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // --- Заранее получим все нужные GUID'ы из конфигурации ---
                    // Для статусов, НА КОТОРЫЕ будем менять
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["CompletedStatusUUID"], out Guid completedGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для CompletedStatusUUID", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["InTransitStatusUUID"], out Guid inTransitGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для InTransitStatusUUID", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // --- Ключевые GUID'ы для логики "Готово" ---
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["ReadyForPickupStatusUUID"], out Guid readyForPickupGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для ReadyForPickupStatusUUID", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["AwaitingCourierStatusUUID"], out Guid awaitingCourierGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для AwaitingCourierStatusUUID", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // --- GUID типа "Доставка" ---
                    if (!Guid.TryParse(ConfigurationManager.AppSettings["DeliveryTypeUUID"], out Guid deliveryGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для DeliveryTypeUUID", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Используем уже имеющиеся статические поля для Accepted и Preparing
                    // Guid acceptedGuid = AcceptedStatusUUID;
                    // Guid preparingGuid = PreparingStatusUUID;
                    // Guid readyForPickupCurrentGuid = ReadyForPickupStatusUUID; // Для сравнения текущего статуса
                    // Guid awaitingCourierCurrentGuid = AwaitingCourierStatusUUID; // Для сравнения текущего статуса

                    Guid targetStatusGuid = Guid.Empty; // Инициализируем
                    string targetStatusName = string.Empty; // Инициализируем

                    // --- Логика определения следующего статуса ---

                    // 1. Если текущий статус "Готов к выдаче" -> меняем на "Выполнен" (Кнопка "Заказ получен")
                    if (orderToComplete.IdСтатуса == ReadyForPickupStatusUUID)
                    {
                        targetStatusGuid = completedGuid;
                        targetStatusName = "Выполнен";
                    }
                    // 2. Если текущий статус "Ожидает курьера" -> меняем на "В пути" (Кнопка "Передан курьеру")
                    else if (orderToComplete.IdСтатуса == AwaitingCourierStatusUUID)
                    {
                        targetStatusGuid = inTransitGuid;
                        targetStatusName = "В пути";
                    }
                    // 3. Если текущий статус "Принят" ИЛИ "Готовится" -> проверяем тип заказа (Кнопка "Готово")
                    else if (orderToComplete.IdСтатуса == AcceptedStatusUUID || orderToComplete.IdСтатуса == PreparingStatusUUID)
                    {
                        // --- Вот она, проверка типа заказа! ---
                        if (orderToComplete.IdТипа == deliveryGuid) // Если тип = Доставка
                        {
                            targetStatusGuid = awaitingCourierGuid; // Меняем на "Ожидает курьера"
                            targetStatusName = "Ожидает курьера";
                        }
                        else // Если тип НЕ Доставка (Самовывоз и т.д.)
                        {
                            targetStatusGuid = readyForPickupGuid; // Меняем на "Готов к выдаче"
                            targetStatusName = "Готов к выдаче";
                        }
                    }
                    else // Обработка других статусов, если они вдруг попадут в этот список
                    {
                        // Найдем имя текущего статуса для сообщения
                        _statusNamesDictionary.TryGetValue(orderToComplete.IdСтатуса, out string currentStatusName);
                        MessageBox.Show($"Для заказа со статусом '{currentStatusName ?? orderToComplete.IdСтатуса.ToString()}' действие не определено в этой логике.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        return; // Ничего не делаем
                    }

                    // --- Если целевой статус определен, обновляем заказ ---
                    if (targetStatusGuid != Guid.Empty)
                    {
                        var updateResponse = await App.SupabaseClient
                           .From<Заказы>()
                           .Where(x => x.Id == orderId)
                           .Set(x => x.IdСтатуса, targetStatusGuid)
                           .Update();

                        if (updateResponse.ResponseMessage.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Статус заказа успешно обновлен на '{targetStatusName}'.");
                            // Обновляем список текущих заказов, чтобы убрать/обновить карточку
                            await LoadCurrentOrdersAsync();
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка при обновлении статуса заказа: {updateResponse.ResponseMessage?.ReasonPhrase ?? "Неизвестная ошибка"}", "Ошибка Supabase", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    // Если targetStatusGuid остался пустым (хотя по логике выше этого не должно быть, кроме информационного сообщения), ничего не делаем
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при обработке действия с заказом: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid orderId)
            {
                try
                {
                    // Получаем ID типа заказа "Доставка" из App.config
                    string deliveryTypeUUID = ConfigurationManager.AppSettings["DeliveryTypeUUID"];
                    if (!Guid.TryParse(deliveryTypeUUID, out Guid deliveryGuid))
                    {
                        MessageBox.Show($"Неверный формат GUID для DeliveryTypeUUID: {deliveryTypeUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получаем ID статусов из App.config
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
                    //Находим заказ в коллекции
                    CurrentOrderViewModel orderToComplete = CurrentOrders.FirstOrDefault(x => x.Id == orderId);
                    if (orderToComplete == null)
                    {
                        MessageBox.Show("Заказ не найден в списке текущих заказов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    Guid targetStatusGuid = (orderToComplete.IdТипа == deliveryGuid) ? awaitingCourierGuid : readyForPickupGuid;
                    string targetStatusName = (orderToComplete.IdТипа == deliveryGuid) ? "Ожидает курьера" : "Готов к выдаче";

                    // Обновляем статус заказа
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
                        MessageBox.Show($"Статус заказа успешно обновлен на '{targetStatusName}'.");
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
            ObservableCollection<CurrentOrderViewModel> targetCollection = new ObservableCollection<CurrentOrderViewModel>();

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
                string readyForPickupStatusUUID = ConfigurationManager.AppSettings["ReadyForPickupStatusUUID"];
                string awaitingCourierStatusUUID = ConfigurationManager.AppSettings["AwaitingCourierStatusUUID"];

                List<Guid> statusUUIDs = new List<Guid>();

                if (Guid.TryParse(acceptedStatusUUID, out Guid acceptedGuid)) statusUUIDs.Add(acceptedGuid);
                else MessageBox.Show($"Неверный формат GUID для AcceptedStatusUUID: {acceptedStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);

                if (Guid.TryParse(preparingStatusUUID, out Guid preparingGuid)) statusUUIDs.Add(preparingGuid);
                else MessageBox.Show($"Неверный формат GUID для PreparingStatusUUID: {preparingStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);

                if (Guid.TryParse(readyForPickupStatusUUID, out Guid readyGuid)) statusUUIDs.Add(readyGuid); // Добавляем новый статус
                else MessageBox.Show($"Неверный формат GUID для ReadyForPickupStatusUUID: {readyForPickupStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);

                if (Guid.TryParse(awaitingCourierStatusUUID, out Guid awaitingCourierGuid)) statusUUIDs.Add(awaitingCourierGuid); // Добавляем в список
                else MessageBox.Show($"Неверный формат GUID для AwaitingCourierStatusUUID: {awaitingCourierStatusUUID}", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);

                if (statusUUIDs.Count == 0)
                {
                    MessageBox.Show($"Не удалось определить UUID статусов из конфигурации.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        .OrderBy(o => o.ВремяСоздания)
                        .ToList();

                    if (!filteredOrders.Any())
                    {
                        return;
                    }

                    var dishesResponse = await App.SupabaseClient.From<Блюда>().Select("*").Get();
                    var allDishesDictionary = dishesResponse?.Models?.ToDictionary(d => d.Id, d => d);

                    var categoriesResponse = await App.SupabaseClient.From<Категории>().Select("*").Get();
                    var allCategoriesDictionary = categoriesResponse?.Models?.ToDictionary(category => category.Id, category => category);

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
                                Guid categoryId = Guid.Empty;
                                string iconUrl = null;

                                if (allDishesDictionary != null && allDishesDictionary.TryGetValue(item.IdБлюда, out Блюда dish))
                                {
                                    dishName = dish.НазваниеБлюда ?? dishName;
                                    categoryId = dish.IdКатегории;

                                    if (allCategoriesDictionary != null && allCategoriesDictionary.TryGetValue(dish.IdКатегории, out Категории category))
                                    {
                                        iconUrl = category.СсылкаНаИконку;
                                    }
                                }

                                orderItems.Add(new OrderItemDisplayViewModel
                                {
                                    НазваниеБлюда = dishName,
                                    Количество = item.Количество,
                                    ЦенаНаМоментЗаказа = item.ЦенаНаМоментЗаказа,
                                    IdКатегории = categoryId,
                                    СсылкаНаИконкуКатегории = iconUrl //НОВОЕ
                                });
                            }
                        }

                        if (orderItems.Any())
                        {
                            targetCollection.Add(new CurrentOrderViewModel
                            {
                                Id = order.Id,
                                НомерЗаказа = order.НомерЗаказа ?? "Нет номера",
                                Items = orderItems,
                                ВремяСоздания = order.ВремяСоздания,
                                IdТипа = order.IdТипа,
                                IdСтатуса = order.IdСтатуса
                            });
                        }
                    }
                    CurrentOrders = targetCollection;
                    CurrentOrdersItemsControl.ItemsSource = CurrentOrders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки текущих заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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