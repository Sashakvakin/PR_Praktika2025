using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPointMobile.Models
{
    [Table("ТипыЗаказов")]
    public class ТипЗаказа : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("название_типа")]
        public string НазваниеТипа { get; set; }
    }
}