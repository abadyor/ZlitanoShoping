using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Response.Cities
{
    public class CitiesResponseDTO
    {
        public int Id { get; set; }
        public string CCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

}
