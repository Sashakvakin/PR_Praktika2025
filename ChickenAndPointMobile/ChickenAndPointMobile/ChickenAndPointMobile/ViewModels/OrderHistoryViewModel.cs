using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChickenAndPointMobile.ViewModels
{
    public class OrderHistoryViewModel : INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public string НомерЗаказа { get; set; }
        public DateTimeOffset ВремяСоздания { get; set; }
        public decimal? ИтоговаяСумма { get; set; }
        public string НазваниеСтатуса { get; set; } 
        public string НазваниеТипа { get; set; } 

        public string ВремяСозданияDisplay => ВремяСоздания.ToString("dd.MM.yyyy HH:mm");
        public string ИтоговаяСуммаDisplay => ИтоговаяСумма.HasValue ? $"{ИтоговаяСумма.Value:N2} ₽" : "-";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}