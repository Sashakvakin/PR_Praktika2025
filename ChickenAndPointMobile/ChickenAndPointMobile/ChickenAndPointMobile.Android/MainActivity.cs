// MainActivity.cs
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
using FFImageLoading;
using FFImageLoading.Config;
using System;

namespace ChickenAndPointMobile.Droid
{
    [Activity(Label = "ChickenAndPointMobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // *** ИНИЦИАЛИЗАЦИЯ КАРТ ***
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            // **************************

            var config = new Configuration()
            {
                VerboseLogging = false,
                DiskCacheDuration = TimeSpan.FromDays(90),
                MaxMemoryCacheSize = 100 * 1024 * 1024,
            };
            ImageService.Instance.Initialize(config);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}