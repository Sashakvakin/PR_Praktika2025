using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPointMobile.Models
{
    [Table("Роли")]
    public class Роль : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("название_роли")]
        public string НазваниеРоли { get; set; }
    }
}