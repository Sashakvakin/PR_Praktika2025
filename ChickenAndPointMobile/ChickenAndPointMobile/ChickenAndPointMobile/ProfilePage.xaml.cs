using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ChickenAndPointMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserProfileData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void LoadUserProfileData()
        {
            var userProfile = App.LoggedInUserProfile;
            var currentUser = App.SupabaseClient.Auth.CurrentUser;

            if (userProfile != null)
            {
                NameLabel.Text = userProfile.ПолноеИмя ?? "Не указано";
            }
            else
            {
                NameLabel.Text = "Не удалось загрузить";
            }

            if (currentUser != null)
            {
                EmailLabel.Text = currentUser.Email ?? "Не указан";
            }
            else
            {
                EmailLabel.Text = "Не удалось загрузить";
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