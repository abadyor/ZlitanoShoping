using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Request.Cities
{
    public class CreateCitiesRequestDTO
    {
        [Required]
        [StringLength(8)]
        public string CCode { get; set; } = string.Empty;

        [Required]
        [StringLength(56)]
        public string Name { get; set; } = string.Empty;
    }

}
