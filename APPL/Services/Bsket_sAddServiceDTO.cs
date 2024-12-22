namespace APPL.Services
{
    public class Bsket_sAddServiceDTO
    {
        public int BasketId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Totoal { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
