using System.ComponentModel.DataAnnotations;

namespace APPL.DTOs.Response.ShopControle
{
    public class ShopControleResponseBYID_DTO
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
        public TimeOnly? StartWork { get; set; }
        public TimeOnly? EndWork { get; set; }
        public string Notes { get; set; } = string.Empty;

    }
}
