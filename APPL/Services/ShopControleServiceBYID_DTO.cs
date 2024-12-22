using System.ComponentModel.DataAnnotations;

namespace APPL.Services
{
    public class ShopControleServiceBYID_DTO
    {
        public int Id { get; set; }


        [StringLength(40)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Region { get; set; } = string.Empty;

        [StringLength(20)]
        public string Street { get; set; } = string.Empty;

        [StringLength(80)]
        public string NerestPoint { get; set; } = string.Empty;
        public string StartWork { get; set; }
        public string EndWork { get; set; }
        public TimeOnly? StartWorkTime { get; set; } // القيم المحولة
        public TimeOnly? EndWorkTime { get; set; }   // القيم المحولة
        public string Notes { get; set; } = string.Empty;
    }
}
