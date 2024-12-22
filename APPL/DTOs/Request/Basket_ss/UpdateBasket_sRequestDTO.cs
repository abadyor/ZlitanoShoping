namespace APPL.DTOs.Request.Basket_ss
{
    public class UpdateBasket_sRequestDTO
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Totoal { get; set; } = 0;

    }
}
