using System;
using System.Threading.Tasks;
using ChickenAndPointMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MenuPageViewModel vm;
        private bool _isNavigating = false;

        public MenuPage()
        {
            InitializeComponent();
            vm = BindingContext as MenuPageViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _isNavigating = false;
            if (vm != null && vm.GroupedMenu.Count == 0)
            {
                await vm.ExecuteLoadMenuCommand();
            }
        }

        private async void DishItem_Tapped(object sender, EventArgs e)
        {
            if (_isNavigating)
            {
                return;
            }

            DishViewModel selectedDishVm = null;

            if (sender is BindableObject bindable && bindable.BindingContext is DishViewModel contextVm)
            {
                selectedDishVm = contextVm;
            }
            else if (e is TappedEventArgs tappedArgs && tappedArgs.Parameter is DishViewModel paramVm)
            {
                selectedDishVm = paramVm;
            }

            if (selectedDishVm != null)
            {
                try
                {
                    _isNavigating = true;
                    await Navigation.PushModalAsync(new DishDetailPage(selectedDishVm));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
                    await DisplayAlert("Ошибка", "Не удалось открыть детали блюда.", "OK");
                }
                finally
                {
                    _isNavigating = false;
                }
            }
        }
    }
}