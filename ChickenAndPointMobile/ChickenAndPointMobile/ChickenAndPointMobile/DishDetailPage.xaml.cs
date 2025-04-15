using System;
using ChickenAndPointMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DishDetailPage : ContentPage
    {
        private readonly DishViewModel _dishViewModel;

        public DishDetailPage(DishViewModel dishViewModel)
        {
            InitializeComponent();
            _dishViewModel = dishViewModel ?? throw new ArgumentNullException(nameof(dishViewModel));
            BindingContext = _dishViewModel;
        }

        private async void AddToCartButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Корзина", $"Добавлено: {_dishViewModel.НазваниеБлюда}", "OK");
            await Navigation.PopModalAsync();
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}