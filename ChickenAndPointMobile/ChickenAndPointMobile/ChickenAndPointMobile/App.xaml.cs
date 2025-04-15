using Supabase;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ChickenAndPointMobile.Models;
using System.Threading.Tasks;

namespace ChickenAndPointMobile
{
    public partial class App : Application
    {
        public static Supabase.Client SupabaseClient { get; private set; }
        public static Пользователь LoggedInUserProfile { get; set; }

        private const string SupabaseUrl = "https://xulxvorvdykxkbxwgbhl.supabase.co";
        private const string SupabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1bHh2b3J2ZHlreGtieHdnYmhsIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc0MzAxOTEzNSwiZXhwIjoyMDU4NTk1MTM1fQ.YMyzsHdoSMXTTZtROztbeptNwjE-8nRKprhI1B5ySaU";


        public App()
        {
            InitializeComponent();

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };

            SupabaseClient = new Supabase.Client(SupabaseUrl, SupabaseAnonKey, options);
            MainPage = new LoginPage();
        }

        protected override async void OnStart()
        {
            bool loggedIn = false;

            try
            {
                await SupabaseClient.InitializeAsync();
                var session = SupabaseClient.Auth.CurrentSession;

                if (session != null)
                {
                    if (Guid.TryParse(session.User.Id, out Guid userId))
                    {
                        var loginPageInstance = new LoginPage();
                        LoggedInUserProfile = await loginPageInstance.GetUserProfile(userId);
                        if (LoggedInUserProfile != null)
                        {
                            var userRole = await loginPageInstance.GetUserRole(LoggedInUserProfile.IdРоли);
                            if (userRole != null && "Клиент".Equals(userRole.НазваниеРоли, StringComparison.OrdinalIgnoreCase))
                            {
                                var clientWindow = new ClientMainWindow();
                                var navigationPage = new NavigationPage(clientWindow)
                                {
                                    Title = "Курочка и точка",
                                    BarBackgroundColor = Color.WhiteSmoke,
                                    BarTextColor = Color.OrangeRed
                                };
                                var profileToolbarItem = new ToolbarItem
                                {
                                    IconImageSource = "profile.png",
                                    Order = ToolbarItemOrder.Primary,
                                    Priority = 0
                                };
                                profileToolbarItem.Clicked += async (sender, args) => {
                                    await navigationPage.PushAsync(new ProfilePage());
                                };
                                navigationPage.ToolbarItems.Add(profileToolbarItem);
                                MainPage = navigationPage;
                                loggedIn = true;
                            }
                            else
                            {
                                await App.SupabaseClient.Auth.SignOut();
                                LoggedInUserProfile = null;
                            }
                        }
                        else
                        {
                            await App.SupabaseClient.Auth.SignOut();
                            LoggedInUserProfile = null;
                        }
                    }
                    else
                    {
                        await App.SupabaseClient.Auth.SignOut();
                        LoggedInUserProfile = null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в App.OnStart: {ex}");
                if (App.SupabaseClient?.Auth?.CurrentSession != null)
                {
                    try { await App.SupabaseClient.Auth.SignOut(); } catch { }
                }
                LoggedInUserProfile = null;
                loggedIn = false;
            }

            if (!loggedIn && !(MainPage is LoginPage))
            {
                MainPage = new LoginPage();
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}