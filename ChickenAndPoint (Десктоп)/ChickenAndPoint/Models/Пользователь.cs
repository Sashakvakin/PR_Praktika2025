﻿using Postgrest.Models;
using Postgrest.Attributes;
using System;

namespace ChickenAndPoint.Models
{
    [Table("Пользователи")]
    public class Пользователь : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("id_роли")]
        public Guid IdРоли { get; set; }

        [Column("полное_имя")]
        public string ПолноеИмя { get; set; }

        [Column("номер_телефона")]
        public string НомерТелефона { get; set; }
    }
}