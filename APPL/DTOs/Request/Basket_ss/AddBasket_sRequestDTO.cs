namespace APPL.DTOs.Request.Basket_ss
{
    public class AddBasket_sRequestDTO
    {
        public int BasketId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Totoal { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
