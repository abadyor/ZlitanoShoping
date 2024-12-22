﻿namespace APPL.Services
{
    public class UpdateShopeServiceDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public bool HasDiscount { get; set; }
    }
}
