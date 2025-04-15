using System;
using ChickenAndPointMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        ProfilePageViewModel vm;

        public ProfilePage()
        {
            InitializeComponent();
            vm = new ProfilePageViewModel();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            vm.LoadUserData();
            await vm.LoadOrderHistoryAsync();
        }

        private async void OrderDetails_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is OrderHistoryViewModel selectedOrder)
            {
                await Navigation.PushAsync(new OrderDetailsWindow(selectedOrder.Id));
            }
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Выход", "Вы уверены, что хотите выйти?", "Да", "Нет");
            if (confirm)
            {
                App.LoggedInUserProfile = null;
                try
                {
                    await App.SupabaseClient.Auth.SignOut();
                    Application.Current.MainPage = new LoginPage();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Не удалось выйти: {ex.Message}", "OK");
                }
            }
        }
    }
}