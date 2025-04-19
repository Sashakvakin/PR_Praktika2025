using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Supabase;
using Postgrest.Models;
using Postgrest.Attributes;
using static Postgrest.Constants;

namespace LibraryCAP
{
    public class CalcOrderItem
    {
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; }
    }

    [Table("Заказы")]
    internal class OrderNumberModel : BaseModel
    {
        // Атрибут [Column] обязателен для сопоставления
        [Column("номер_заказа")]
        public string НомерЗаказа { get; set; }

        [PrimaryKey("id")]
        public Guid Id { get; set; }
    }

    public static class OrderUtils
    {
        // Рассчитывает итоговую сумму на основе списка позиций.
        public static decimal CalculateTotalSum(IEnumerable<CalcOrderItem> items)
        {
            if (items == null || !items.Any())
            {
                return 0m;
            }
            return items.Sum(item => item.Quantity * item.PricePerItem);
        }

        // Подсчитывает общее количество единиц товаров во всех позициях.
        public static int CountTotalUnits(IEnumerable<CalcOrderItem> items)
        {
            if (items == null)
            {
                return 0;
            }
            return items.Sum(item => item.Quantity);
        }

        // Подсчитывает количество уникальных позиций товаров.
        public static int CountOrderLines(IEnumerable<CalcOrderItem> items)
        {
            if (items == null)
            {
                return 0;
            }
            return items.Count();
        }

        // Форматирует цену в стандартную строку валюты ("199.90 ₽").
        public static string FormatPrice(decimal price)
        {
            return price.ToString("N2", CultureInfo.InvariantCulture) + " ₽";
        }

        // Генерирует следующий порядковый номер заказа с заданным префиксом.
        public static async Task<string> GenerateNextOrderNumberClientSideAsync(Supabase.Client client, string prefix)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Клиент Supabase не может быть null.");
            }
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException("Префикс номера заказа не может быть пустым.", nameof(prefix));
            }

            string newOrderNumber = prefix + "1";

            try
            {
                const string columnsToSelect = "номер_заказа";

                var response = await client.From<OrderNumberModel>()
                    .Select(columnsToSelect) // Выбираем только нужную колонку
                    .Filter("номер_заказа", Operator.ILike, $"{prefix}%")
                    .Order("номер_заказа", Ordering.Descending)
                    .Limit(1)
                    .Get();

                var lastOrderModel = response?.Models?.FirstOrDefault();

                if (lastOrderModel != null && !string.IsNullOrEmpty(lastOrderModel.НомерЗаказа))
                {
                    Match numberMatch = Regex.Match(lastOrderModel.НомерЗаказа, @"\d+$");
                    if (numberMatch.Success && int.TryParse(numberMatch.Value, out int lastNumberValue))
                    {
                        newOrderNumber = $"{prefix}{lastNumberValue + 1}";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при генерации номера заказа: {ex.Message}");
            }

            return newOrderNumber;
        }
    }
}