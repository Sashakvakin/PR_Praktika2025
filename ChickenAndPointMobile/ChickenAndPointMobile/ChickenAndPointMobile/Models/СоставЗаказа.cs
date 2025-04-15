using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPointMobile.Models
{
    [Table("СоставЗаказа")]
    public class СоставЗаказа : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("id_заказа")]
        public Guid IdЗаказа { get; set; }

        [Column("id_блюда")]
        public Guid IdБлюда { get; set; }

        [Column("количество")]
        public int Количество { get; set; }

        [Column("цена_на_момент_заказа")]
        public decimal ЦенаНаМоментЗаказа { get; set; }
    }
}