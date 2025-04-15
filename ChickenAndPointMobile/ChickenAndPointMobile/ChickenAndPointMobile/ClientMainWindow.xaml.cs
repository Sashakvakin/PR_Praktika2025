using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientMainWindow : TabbedPage
    {
        public ClientMainWindow()
        {
            InitializeComponent();
        }

        private async void ProfileToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (CurrentPage is NavigationPage currentNavPage)
            {
                await currentNavPage.Navigation.PushAsync(new ProfilePage());
            }
            else if (Navigation != null)
            {
                await Navigation.PushAsync(new ProfilePage());
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось открыть профиль.", "OK");
            }
        }
    }
}