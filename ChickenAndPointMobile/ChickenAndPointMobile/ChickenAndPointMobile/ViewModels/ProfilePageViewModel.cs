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
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private string _userName;
        public string UserName { get => _userName; set => SetProperty(ref _userName, value); }

        private string _userEmail;
        public string UserEmail { get => _userEmail; set => SetProperty(ref _userEmail, value); }

        private ObservableCollection<OrderHistoryViewModel> _orders;
        public ObservableCollection<OrderHistoryViewModel> Orders { get => _orders; set => SetProperty(ref _orders, value); }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        private string _loadingStatus;
        public string LoadingStatus { get => _loadingStatus; set => SetProperty(ref _loadingStatus, value); }

        private bool _showNoOrdersMessage;
        public bool ShowNoOrdersMessage { get => _showNoOrdersMessage; set => SetProperty(ref _showNoOrdersMessage, value); }

        private Dictionary<Guid, string> _statusCache = new Dictionary<Guid, string>();
        private Dictionary<Guid, string> _typeCache = new Dictionary<Guid, string>();


        public ProfilePageViewModel()
        {
            Orders = new ObservableCollection<OrderHistoryViewModel>();
            LoadUserData();
        }

        public void LoadUserData()
        {
            UserName = App.LoggedInUserProfile?.ПолноеИмя ?? "Не указано";
            UserEmail = App.SupabaseClient.Auth.CurrentUser?.Email ?? "Не указан";
        }

        public async Task LoadOrderHistoryAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            LoadingStatus = "Загрузка истории...";
            ShowNoOrdersMessage = false;
            Orders.Clear();

            try
            {
                var userId = App.LoggedInUserProfile?.Id;
                if (!userId.HasValue || userId.Value == Guid.Empty)
                {
                    LoadingStatus = "Ошибка: Не удалось получить ID пользователя.";
                    ShowNoOrdersMessage = true;
                    return;
                }

                if (!_statusCache.Any())
                {
                    var statuses = await App.SupabaseClient.From<СтатусЗаказа>().Select("id, название_статуса").Get();
                    if (statuses?.Models != null)
                        _statusCache = statuses.Models.ToDictionary(s => s.Id, s => s.НазваниеСтатуса ?? "?");
                }
                if (!_typeCache.Any())
                {
                    var types = await App.SupabaseClient.From<ТипЗаказа>().Select("id, название_типа").Get();
                    if (types?.Models != null)
                        _typeCache = types.Models.ToDictionary(t => t.Id, t => t.НазваниеТипа ?? "?");
                }

                var response = await App.SupabaseClient
                    .From<Заказы>()
                    .Select("*")
                    .Filter("id_клиента", Operator.Equals, userId.Value.ToString())
                    .Order("время_создания", Ordering.Descending)
                    .Get();

                if (response?.Models != null)
                {
                    if (!response.Models.Any())
                    {
                        LoadingStatus = "";
                        ShowNoOrdersMessage = true;
                    }
                    else
                    {
                        foreach (var order in response.Models)
                        {
                            Orders.Add(new OrderHistoryViewModel
                            {
                                Id = order.Id,
                                НомерЗаказа = order.НомерЗаказа ?? "б/н",
                                ВремяСоздания = order.ВремяСоздания,
                                ИтоговаяСумма = order.ИтоговаяСумма,
                                НазваниеСтатуса = _statusCache.TryGetValue(order.IdСтатуса, out var status) ? status : "??",
                                НазваниеТипа = _typeCache.TryGetValue(order.IdТипа, out var type) ? type : "??"
                            });
                        }
                        LoadingStatus = "";
                        ShowNoOrdersMessage = false;
                    }
                }
                else
                {
                    LoadingStatus = "Не удалось загрузить историю.";
                    ShowNoOrdersMessage = true;
                    System.Diagnostics.Debug.WriteLine($"Order History Loading Error: Response or Models were null. Response Status: {response?.ResponseMessage?.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                LoadingStatus = $"Ошибка загрузки: {ex.Message}";
                ShowNoOrdersMessage = true;
                System.Diagnostics.Debug.WriteLine($"Order History Loading Exception: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }


        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}