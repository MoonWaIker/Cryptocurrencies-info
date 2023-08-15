﻿using System.ComponentModel.DataAnnotations;

namespace Cryptocurrencies_info.Models.Cryptocurrencies
{
    public class Market
    {
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Logo { get; set; } = string.Empty;
        [MinLength(1)]
        public string Base { get; set; } = string.Empty;
        [MinLength(1)]
        public string Target { get; set; } = string.Empty;
        public string Trust { get; set; } = string.Empty;
        public string? Link { get; set; } = string.Empty;
    }
}