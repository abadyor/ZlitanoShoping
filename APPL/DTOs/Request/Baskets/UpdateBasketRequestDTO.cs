namespace APPL.DTOs.Request.Baskets
{
    public class UpdateBasketRequestDTO
    {
        public int Id { get; set; }
        public int customerId { get; set; }



        public string nameIdShope { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public decimal tootal { get; set; } = decimal.Zero;

        public int countElementBasket { get; set; } = 0;

        //كود السلة =كود المحل+رقم الزبون+رقم السلة
        public string codeBasket { get; set; }

        public bool closeBasket { get; set; } = false;

        public string? loguser { get; set; } = null;
    }
}
