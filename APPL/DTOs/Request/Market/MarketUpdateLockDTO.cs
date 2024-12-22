using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Request.Market
{
    public class MarketUpdateLockDTO
    {
        public int Id { get; set; }
        public bool IsLock { get; set; }
    }
}
