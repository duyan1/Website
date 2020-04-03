using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class ItemResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}