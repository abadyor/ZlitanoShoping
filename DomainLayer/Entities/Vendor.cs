﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class Vendor
    {

        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string GivenNames { get; set; }

        [StringLength(20)]
        public string Nickname { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        [StringLength(10)]
        public string DocId { get; set; }

        [StringLength(12)]
        public string DocType { get; set; }

        [EmailAddress, Required, DataType(DataType.EmailAddress)]
        [RegularExpression("[^@ \\t\\r\\n]+@[^@ \\t\\r\\n]+\\.[^@ \\t\\r\\n]+", ErrorMessage = "الإيميل الدي اقترحته يجب ان يكون في الشكل التالي ,, @gmail ,@hotmail,@yahoo")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [StringLength(20)]
        [Phone]
        public string Mobile { get; set; }

    
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }
        [StringLength(8)]
        public string Log_Code { get; set; } = "0";
        [Required]

        public DateTime Timestamp_create { get; set; } = DateTime.Now;


    }
}
