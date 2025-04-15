using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPointMobile.Models
{
    [Table("СтатусыЗаказов")]
    public class СтатусЗаказа : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("название_статуса")]
        public string НазваниеСтатуса { get; set; }
    }
}