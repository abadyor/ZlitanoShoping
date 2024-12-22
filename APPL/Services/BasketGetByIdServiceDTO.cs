namespace APPL.Services
{
    public class BasketGetByIdServiceDTO
    {
        public int Id { get; set; }


        public int customerId { get; set; }



        public string nameIdShope { get; set; }

        public DateTime Date { get; set; }

        public decimal tootal { get; set; }

        public int countElementBasket { get; set; }

        //كود السلة =كود المحل+رقم الزبون+رقم السلة
        public string codeBasket { get; set; }

        public bool closeBasket { get; set; } = false;

        public string? loguser { get; set; }
    }
}
