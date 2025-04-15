using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChickenAndPointMobile.ViewModels
{
    public class OrderDetailItemViewModel : INotifyPropertyChanged
    {
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
        public decimal TotalItemPrice => Quantity * PriceAtOrder;

        public string QuantityDisplay => $"x {Quantity}";
        public string PriceAtOrderDisplay => $"{PriceAtOrder:N2} ₽";
        public string TotalItemPriceDisplay => $"{TotalItemPrice:N2} ₽";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}