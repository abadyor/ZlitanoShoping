using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Request.Cities
{
    public class UpdateCitiesRequestDTO:CreateCitiesRequestDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
