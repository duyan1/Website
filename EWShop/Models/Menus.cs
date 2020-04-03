using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class Menus
    {
        public string menuId { get; set; }
        public string menuName { get; set; }
        public string isMainMenu { get; set; }
        public string mainMenuId { get; set; }
        public string menuUrl { get; set; }
        public string remarks { get; set; }
        public string iconMenu { get; set; }
        public string menuShortName { get; set; }
    }
}