﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Response.Vendor
{
    public class VendorLog_CodeDTO
    {
   
        public string Username { get; set; }
        public string Log_Code { get; set; } = "0";
    }
}
