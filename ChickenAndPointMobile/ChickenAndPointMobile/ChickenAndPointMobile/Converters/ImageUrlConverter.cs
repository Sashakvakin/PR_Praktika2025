using System;
using System.Globalization;
using Xamarin.Forms;

namespace ChickenAndPointMobile.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imageUrl && !string.IsNullOrWhiteSpace(imageUrl))
            {
                try
                {
                    return new UriImageSource
                    {
                        Uri = new Uri(imageUrl),
                        CachingEnabled = true,
                        CacheValidity = TimeSpan.FromDays(1)
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Image conversion error: {ex.Message}");
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}