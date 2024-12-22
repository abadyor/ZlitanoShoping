using System.ComponentModel.DataAnnotations;

namespace APPL.Services
{
    public class CustomerAddServiceDTO
    {
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

        [Required, EmailAddress, DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [StringLength(20), Phone]
        public string Mobile { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
