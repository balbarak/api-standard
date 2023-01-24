﻿namespace Spoiler.Api.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Email
        {
            get
            {
                return $"{Username}@example.com";
            }
        }
    }
}
