using System;
using ChickenAndPointMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderDetailsWindow : ContentPage
    {
        OrderDetailsViewModel vm;

        public OrderDetailsWindow(Guid orderId)
        {
            InitializeComponent();
            vm = new OrderDetailsViewModel(orderId);
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (vm != null && vm.OrderItems.Count == 0)
            {
                await vm.ExecuteLoadDetailsCommand();
            }
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}