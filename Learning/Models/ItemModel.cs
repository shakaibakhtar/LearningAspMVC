using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learning.Models
{
    public class ItemModel
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}