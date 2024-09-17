﻿using System.ComponentModel.DataAnnotations;

namespace EnglishCenter.Models.DTO
{
    public class UserBackgroundDto
    {
        [Required]
        public string UserName { set; get; }

        public string? Description { set; get; }

        public List<string>? Roles { set; get; }

        public string? Image { set; get; }

        public string? BackgroundImage { set; get; }
    }
}
