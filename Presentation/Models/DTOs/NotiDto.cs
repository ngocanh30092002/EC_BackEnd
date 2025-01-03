﻿namespace EnglishCenter.Presentation.Models.DTOs
{
    public class NotiDto
    {
        public long? NotiStuId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? Time { get; set; }

        public bool? IsRead { get; set; }

        public string? Image { set; get; }

        public string? LinkUrl { set; get; }

    }
}
