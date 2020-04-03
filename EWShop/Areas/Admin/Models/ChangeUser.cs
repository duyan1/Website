using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Admin.Models
{
    public class ChangeUser
    {
        public string userName { get; set; }
        public string newPass { get; set; }
        public string confirmPass { get; set; }
    }
}