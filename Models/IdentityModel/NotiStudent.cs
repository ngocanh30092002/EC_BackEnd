﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishCenter.Models
{
    public class NotiStudent
    {
        [Key]
        public long NotiStuId { set;get; }
        public long NotiId { get; set; }

        [ForeignKey("NotiId")]
        [InverseProperty("NotiStudents")]
        public Notification? Notification { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("NotiStudents")]
        public Student? Student { get; set; }

        public bool IsRead { get; set; }
    }
}
