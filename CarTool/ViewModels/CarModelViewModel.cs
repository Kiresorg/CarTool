using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarTool.ViewModels
{
    public class CarModelViewModel
    {
        public int ModelID { get; set; }
        public int LineID { get; set; }

        [Display(Name = "Line")]
        public string LineName { get; set; }

        [Display(Name = "Name")]
        public string ModelName { get; set; }

        [Display(Name = "Description")]
        public string ModelDescription { get; set; }
        public Nullable<int> Price { get; set; }
        public byte[] Picture { get; set; }
        public string ImagePath { get; set; }
    }
}