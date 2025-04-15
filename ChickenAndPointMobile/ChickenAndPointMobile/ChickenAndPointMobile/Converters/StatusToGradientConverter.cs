using System;
using System.Globalization;
using Xamarin.Forms;

namespace ChickenAndPointMobile.Converters
{
    public class StatusToGradientConverter : IValueConverter
    {

        private readonly Color _completedStart = Color.FromHex("#C8E6C9");
        private readonly Color _completedEnd = Color.FromHex("#A5D6A7");

        private readonly Color _cancelledStart = Color.FromHex("#FFCDD2");
        private readonly Color _cancelledEnd = Color.FromHex("#EF9A9A");

        private readonly Color _activeStart = Color.FromHex("#FFF9C4");
        private readonly Color _activeEnd = Color.FromHex("#FFF59D");

        private readonly Color _defaultStart = Color.White;
        private readonly Color _defaultEnd = Color.FromHex("#FAFAFA");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                Color startColor, endColor;
                switch (status?.ToLowerInvariant())
                {
                    case "выполнен":
                        startColor = _completedStart;
                        endColor = _completedEnd;
                        break;
                    case "отменен":
                        startColor = _cancelledStart;
                        endColor = _cancelledEnd;
                        break;
                    case "в пути":
                    case "готовится":
                    case "принят":
                    case "готов к выдаче":
                    case "ожидает курьера":
                        startColor = _activeStart;
                        endColor = _activeEnd;
                        break;
                    default: 
                        startColor = _defaultStart;
                        endColor = _defaultEnd;
                        break;
                }

                return new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop { Color = startColor, Offset = 0.0f },
                        new GradientStop { Color = endColor, Offset = 1.0f }
                    }
                };
            }

            return new SolidColorBrush(Color.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}