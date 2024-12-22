using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Request.Vendor
{
    public class VendorLogCode_UpdateDTO
    {
        [StringLength(50)]
        public string Username { get; set; }
        [StringLength(8)]
        public string Log_Code { get; set; } = "0";
    }
}
