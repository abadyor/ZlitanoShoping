using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class Market
    {
        public int Id { get; set; }
        [Required]
        [StringLength(48)]
        public string Name { get; set; }
        [Required]
        [StringLength(8)]
        public string Mcode { get; set; }
        [Required]
        [StringLength(200)]
        public string MInstance { get; set; }
        public bool IsLock { get; set; } = false;
    }
}
