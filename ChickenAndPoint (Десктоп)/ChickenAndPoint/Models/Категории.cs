using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPoint.Models
{
    [Table("Категории")]
    public class Категории : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("название_категории")]
        public string НазваниеКатегории { get; set; }

        [Column("ссылка_на_иконку")]
        public string СсылкаНаИконку { get; set; } // НОВОЕ
    }
}