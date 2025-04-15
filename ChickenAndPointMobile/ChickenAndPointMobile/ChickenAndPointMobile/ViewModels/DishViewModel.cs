using ChickenAndPointMobile.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ChickenAndPointMobile.ViewModels
{
    public class DishViewModel : INotifyPropertyChanged
    {
        public Блюда Dish { get; }

        public DishViewModel(Блюда dish)
        {
            Dish = dish;
            SetImageSource();
        }

        public string НазваниеБлюда => Dish.НазваниеБлюда;
        public string Описание => Dish.Описание;
        public decimal Цена => Dish.Цена;
        public string СсылкаНаИзображение => Dish.СсылкаНаИзображение;

        private ImageSource _imageSourceToShow;
        public ImageSource ImageSourceToShow
        {
            get => _imageSourceToShow;
            private set => SetProperty(ref _imageSourceToShow, value);
        }

        private void SetImageSource()
        {
            if (!string.IsNullOrWhiteSpace(СсылкаНаИзображение))
            {
                try
                {
                    ImageSourceToShow = new UriImageSource
                    {
                        Uri = new System.Uri(СсылкаНаИзображение),
                        CachingEnabled = true,
                        CacheValidity = System.TimeSpan.FromDays(30)
                    };
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating UriImageSource for {СсылкаНаИзображение}: {ex.Message}");
                    ImageSourceToShow = null;
                }
            }
            else
            {
                ImageSourceToShow = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}