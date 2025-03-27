using System;
using System.Configuration;
using System.Windows;
using Supabase;

namespace ChickenAndPoint
{
    public partial class App : Application
    {
        public static Supabase.Client SupabaseClient { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string supabaseUrl = ConfigurationManager.AppSettings["SupabaseUrl"];
            string supabaseKey = ConfigurationManager.AppSettings["SupabaseAnonKey"];

            if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
            {
                MessageBox.Show("Критическая ошибка: Не удалось найти Supabase URL и/или ключ в App.config. Проверьте конфигурационный файл.",
                                "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };
            SupabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, options);

            try
            {
                await SupabaseClient.InitializeAsync();

                if (SupabaseClient.Auth.CurrentUser != null)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка при инициализации Supabase: {ex.Message}\nПриложение будет закрыто.",
                                "Ошибка инициализации", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }
        }
    }
}