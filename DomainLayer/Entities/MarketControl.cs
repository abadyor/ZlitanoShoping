using System.ComponentModel.DataAnnotations;

namespace DL.Entities
{
    public class MarketControl
    {
        public int Id { get; set; }

        [StringLength(40)]
        public string Name { get; set; }
        [StringLength(8)]
        public string Market_Code { get; set; }
        [StringLength(8)]
        public string Last_VendorCode { get; set; }
        [StringLength(20)]
        public string City { get; set; } = string.Empty;

        [StringLength(20)]
        public string Region { get; set; } = string.Empty;
        [StringLength(20)]
        public string Street { get; set; } = string.Empty;
        [StringLength(80)]
        public string NerestPoint { get; set; } = string.Empty;
        public int NVistor { get; set; } = 0;
        public int Vendor_Id { get; set; }

    }
}
