﻿using Microsoft.AspNetCore.Mvc;

namespace EnglishCenter.Presentation.Models
{
    public class TokenRequest
    {
        public string AccessToken { set; get; }
        public string RefreshToken { set; get; }
    }
}