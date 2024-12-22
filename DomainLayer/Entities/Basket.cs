using System.ComponentModel.DataAnnotations.Schema;

namespace DL.Entities
{
    public class Basket
    {

        public int Id { get; set; }

        [ForeignKey(nameof(customer))]
        public int customerId { get; set; }

        public Customer customer { get; set; }

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
