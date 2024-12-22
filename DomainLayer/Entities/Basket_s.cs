using System.ComponentModel.DataAnnotations.Schema;

namespace DL.Entities
{

    public class Basket_s
    {
        public int Id { get; set; }

        [ForeignKey(nameof(basket))]
        public int basketId { get; set; }

        public Basket basket { get; set; }

        public int itemId { get; set; }

        public int quantity { get; set; }

        public decimal totoal { get; set; }

        public DateTime date { get; set; }
    }
}
