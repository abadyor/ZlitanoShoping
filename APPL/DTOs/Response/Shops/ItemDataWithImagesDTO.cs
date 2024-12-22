namespace APPL.DTOs.Response.Shops
{
    public class ItemDataWithImagesDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public bool HasDiscount { get; set; }
        public List<string> Images { get; set; }  // قائمة الصور

        public string path { get; set; }
    }
}
