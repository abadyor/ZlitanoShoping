using System.ComponentModel.DataAnnotations;

namespace DL.Entities
{
    public class Dictionary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Description { get; set; }


        public string TableName { get; set; }


    }
}
