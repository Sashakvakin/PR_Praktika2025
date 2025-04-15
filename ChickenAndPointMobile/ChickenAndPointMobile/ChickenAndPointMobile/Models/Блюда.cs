using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPointMobile.Models
{
    [Table("Блюда")]
    public class Блюда : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("id_категории")]
        public Guid IdКатегории { get; set; }

        [Column("название_блюда")]
        public string НазваниеБлюда { get; set; }

        [Column("описание")]
        public string Описание { get; set; }

        [Column("цена")]
        public decimal Цена { get; set; }

        [Column("ссылка_на_изображение")]
        public string СсылкаНаИзображение { get; set; }

        [Column("доступно")]
        public bool Доступно { get; set; } = true;
    }
}
