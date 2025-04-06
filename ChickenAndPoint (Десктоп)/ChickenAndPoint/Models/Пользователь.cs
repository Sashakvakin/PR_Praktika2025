// Файл: Models/Пользователь.cs
using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPoint.Models
{
    [Table("Пользователи")]
    public class Пользователь : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("id_роли")]
        public Guid IdРоли { get; set; }

        [Column("полное_имя")]
        public string ПолноеИмя { get; set; }

        [Column("номер_телефона")]
        public string НомерТелефона { get; set; }

        [Column("почта")]
        public string Почта { get; set; }

    }
}