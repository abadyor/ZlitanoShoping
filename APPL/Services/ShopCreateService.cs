namespace APPL.Services
{
    public class ShopCreateService
    {
        public String TableName { get; set; }

        public string ProductName { get; set; }


        public string Description { get; set; }


        public decimal Price { get; set; }

        public decimal DiscountedPrice { get; set; }
        public bool HasDiscount { get; set; }
    }
}
