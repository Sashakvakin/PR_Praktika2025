using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ChickenAndPoint.Models;

namespace ChickenAndPoint
{
    public partial class SotrydnikMainWindow : Window, INotifyPropertyChanged
    {
        private Пользователь _loggedInUser;

        private List<Заказы> _ordersList;
        public List<Заказы> OrdersList
        {
            get => _ordersList;
            set
            {
                _ordersList = value;
                OnPropertyChanged();
            }
        }

        public SotrydnikMainWindow(Пользователь user)
        {
            InitializeComponent();
            DataContext = this;
            _loggedInUser = user;
            ShowProfile();
        }

        private void ShowProfile()
        {
            HideAllPanels();
            ProfilePanel.Visibility = Visibility.Visible;
            LoadProfileData();
        }

        private async void ShowOrders()
        {
            HideAllPanels();
            OrdersPanel.Visibility = Visibility.Visible;
            await LoadOrdersAsync();
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

        private async Task LoadOrdersAsync()
        {
            OrdersStatusText.Text = "Загрузка заказов...";
            OrdersStatusText.Visibility = Visibility.Visible;
            OrdersDataGrid.ItemsSource = null;
            OrdersList = null;

            try
            {
                if (App.SupabaseClient == null)
                {
                    OrdersStatusText.Text = "Ошибка: Клиент Supabase не инициализирован.";
                    return;
                }

                var response = await App.SupabaseClient
                    .From<Заказы>()
                    .Select("*")
                    .Get();

                if (response?.Models != null)
                {
                    var sortedOrders = response.Models.OrderByDescending(order => order.ВремяСоздания).ToList();

                    OrdersList = sortedOrders;
                    OrdersDataGrid.ItemsSource = OrdersList;
                    OrdersStatusText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OrdersStatusText.Text = "Не удалось загрузить заказы или заказов нет.";
                }
            }
            catch (Exception ex)
            {
                OrdersStatusText.Text = $"Ошибка загрузки заказов: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void HideAllPanels()
        {
            ProfilePanel.Visibility = Visibility.Collapsed;
            OrdersPanel.Visibility = Visibility.Collapsed;
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
                    case "Заказы":
                        ShowOrders();
                        break;
                }
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.SupabaseClient != null)
                {
                    await App.SupabaseClient.Auth.SignOut();
                }
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}