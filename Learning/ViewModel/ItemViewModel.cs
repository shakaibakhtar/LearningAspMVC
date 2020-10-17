using Learning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learning.ViewModel
{
    public class ItemViewModel
    {
        public ItemModel Item { get; set; }
        public List<ItemModel> ItemsList { get; set; }
        public List<CountryModel> CountriesList { get; set; }
    }
}