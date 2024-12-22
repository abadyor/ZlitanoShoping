namespace APPL.DTOs.Response.Basket_ss
{
    public class Basket_sGetByIdResponseDTO
    {
        public int Id { get; set; }
        public int BasketId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Totoal { get; set; }
        public DateTime Date { get; set; }
    }
}
