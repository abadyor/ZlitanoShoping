using System.ComponentModel.DataAnnotations;

namespace APPL.Services
{
    public class ShopControllCreateServiceDTO
    {
        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(8)]
        public string Market_Code { get; set; }



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
        //public string StartWork { get; set; }
        //public string EndWork { get; set; }
        public TimeOnly? StartWork { get; set; } // القيم المحولة
        public TimeOnly? EndWork { get; set; }   // القيم المحولة
        public string Notes { get; set; } = string.Empty;
    }
}
