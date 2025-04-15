using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RestaurantsPage : ContentPage
    {
        private const string RestaurantAddress = "ул. Воровского, 77, Киров, Кировская обл.";
        private const string RestaurantWorkingHours = "Ежедневно 8:00 - 20:00";
        private Location _restaurantLocation;
        private bool _permissionsGranted = false;
        private bool _mapInitialized = false;

        public RestaurantsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await CheckAndRequestLocationPermission();
            Device.BeginInvokeOnMainThread(() =>
            {
                if (restaurantMap != null)
                {
                    restaurantMap.IsShowingUser = _permissionsGranted;
                    System.Diagnostics.Debug.WriteLine($"OnAppearing: IsShowingUser set to {_permissionsGranted}");
                }
            });

            if (!_mapInitialized || _restaurantLocation == null)
            {
                await SetupMapAndPin();
                _mapInitialized = true;
            }
        }

        private async Task CheckAndRequestLocationPermission()
        {
            var initialStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            System.Diagnostics.Debug.WriteLine($"Initial Location Permission Status: {initialStatus}");

            if (initialStatus == PermissionStatus.Granted)
            {
                _permissionsGranted = true;
                return;
            }

            _permissionsGranted = false;

            var requestedStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            System.Diagnostics.Debug.WriteLine($"Requested Location Permission Status: {requestedStatus}");

            _permissionsGranted = requestedStatus == PermissionStatus.Granted;
        }
        private async Task SetupMapAndPin()
        {
            try
            {
                if (_restaurantLocation == null)
                {
                    System.Diagnostics.Debug.WriteLine("Geocoding address...");
                    var locations = await Geocoding.GetLocationsAsync(RestaurantAddress);
                    _restaurantLocation = locations?.FirstOrDefault();
                    System.Diagnostics.Debug.WriteLine($"Geocoding result: {_restaurantLocation?.Latitude}, {_restaurantLocation?.Longitude}");
                }

                if (_restaurantLocation != null)
                {
                    double zoomRadiusKm = 0.5;
                    Position restaurantPosition = new Position(_restaurantLocation.Latitude, _restaurantLocation.Longitude);

                    if (!_mapInitialized)
                    {
                        MapSpan mapSpan = MapSpan.FromCenterAndRadius(restaurantPosition, Distance.FromKilometers(zoomRadiusKm));
                        restaurantMap.MoveToRegion(mapSpan);
                        System.Diagnostics.Debug.WriteLine("Map region moved.");
                    }

                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = restaurantPosition,
                        Label = "Курочка и точка",
                        Address = RestaurantWorkingHours
                    };

                    if (!restaurantMap.Pins.Any())
                    {
                        restaurantMap.Pins.Add(pin);
                        System.Diagnostics.Debug.WriteLine("Pin added.");
                    }
                    else
                    {
                        var existingPin = restaurantMap.Pins.First();
                        if (existingPin.Position != pin.Position || existingPin.Label != pin.Label || existingPin.Address != pin.Address)
                        {
                            restaurantMap.Pins.Clear();
                            restaurantMap.Pins.Add(pin);
                            System.Diagnostics.Debug.WriteLine("Pin updated.");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось найти координаты для адреса ресторана.", "OK");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Ошибка", "Геокодирование не поддерживается.", "OK");
                System.Diagnostics.Debug.WriteLine($"Geocoding Error: {fnsEx}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось настроить карту: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Map Setup Error: {ex}");
            }
        }
    }
}