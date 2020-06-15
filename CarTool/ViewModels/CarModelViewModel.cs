using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarTool.ViewModels
{
    public class CarModelViewModel
    {
        public int ModelID { get; set; }
        public int LineID { get; set; }

        [Display(Name = "Line")]
        public string LineName { get; set; }

        [Required(ErrorMessage = "Model name required"), MaxLength(20)]
        [Display(Name = "Name")]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Description required"), MaxLength(100)]
        [Display(Name = "Description")]
        public string ModelDescription { get; set; }

        public Nullable<int> Price { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Picture required")]
        public byte[] Picture { get; set; }
        public string ImagePath { get; set; }
    }
}