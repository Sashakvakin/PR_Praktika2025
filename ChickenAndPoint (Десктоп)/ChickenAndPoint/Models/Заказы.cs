using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPoint.Models
{
    [Table("Заказы")]
    public class Заказы : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("id_клиента")]
        public Guid IdКлиента { get; set; }

        [Column("id_статуса")]
        public Guid IdСтатуса { get; set; }

        [Column("id_типа")]
        public Guid IdТипа { get; set; }

        [Column("номер_заказа")]
        public string НомерЗаказа { get; set; }

        [Column("адрес_доставки")]
        public string АдресДоставки { get; set; }

        [Column("время_создания")]
        public DateTimeOffset ВремяСоздания { get; set; }

        [Column("время_обновления")]
        public DateTimeOffset ВремяОбновления { get; set; }

        [Column("итоговая_сумма")]
        public decimal? ИтоговаяСумма { get; set; }
    }
}