using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPoint.Models
{
    [Table("Роли")]
    public class Роль : BaseModel
    {
        // Первичный ключ теперь "id"
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("название_роли")]
        public string НазваниеРоли { get; set; }
    }
}