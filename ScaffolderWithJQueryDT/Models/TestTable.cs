using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScaffolderWithJQueryDT.Models
{
    public class TestModel
    {
        public int ID { get; set; }

        [Display(Name ="First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Last Name")]
        public string Hobby { get; set; }

        [Display(Name = "Degree")]
        public string Degree { get; set; }
    }
}