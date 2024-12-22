using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Response.ShopControle
{
    public class ShopControleResponseDTO
    {
        public int Id { get; set; }

        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(8)]
        public string Shop_Code { get; set; }

        [StringLength(8)]
        public string Last_VendorCode { get; set; }

        [StringLength(20)]
        public string City { get; set; }

        [StringLength(20)]
        public string Region { get; set; }

        [StringLength(20)]
        public string Street { get; set; }

        [StringLength(80)]
        public string NerestPoint { get; set; }

        public int NVistor { get; set; }
    }


}
