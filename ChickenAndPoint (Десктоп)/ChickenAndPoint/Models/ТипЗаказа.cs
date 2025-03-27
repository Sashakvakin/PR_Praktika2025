// Models/ТипЗаказа.cs
using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPoint.Models
{
    [Table("ТипыЗаказов")]
    public class ТипЗаказа : BaseModel
    {
        [PrimaryKey("id")] // Используем "id"
        public Guid Id { get; set; }

        [Column("название_типа")]
        public string НазваниеТипа { get; set; }
    }
}