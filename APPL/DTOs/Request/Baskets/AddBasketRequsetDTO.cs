namespace APPL.DTOs.Request.Baskets
{
    public class AddBasketRequsetDTO
    {



        public int customerId { get; set; }



        public string nameIdShope { get; set; }



        public decimal tootal { get; set; } = decimal.Zero;

        public int countElementBasket { get; set; } = 0;



    }
}
