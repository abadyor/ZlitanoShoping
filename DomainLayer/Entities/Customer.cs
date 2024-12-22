using System.ComponentModel.DataAnnotations;

namespace DL.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string GivenNames { get; set; }

        [StringLength(20)]
        public string Nickname { get; set; }



        [StringLength(1)]
        public string Gender { get; set; }

        [StringLength(10)]
        public string DocId { get; set; }

        [StringLength(12)]
        public string DocType { get; set; }

        [EmailAddress, Required, DataType(DataType.EmailAddress)]
        [RegularExpression("[^@ \\t\\r\\n]+@[^@ \\t\\r\\n]+\\.[^@ \\t\\r\\n]+", ErrorMessage = "الإيميل الدي اقترحته يجب ان يكون في الشكل التالي ,, @gmail ,@hotmail,@yahoo")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string Mobile { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public DateTime Timestamp_create { get; set; }

        public int? CountBasket { get; set; } = 0;

        public decimal? TootalPurchases { get; set; } = decimal.Zero;



        public string? loguser { get; set; }

    }
}
