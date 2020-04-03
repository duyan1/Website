using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Sales.Models
{
    public class MaterialPriceModel
    {
        public string matCode { get; set; }
        public string matName { get; set; }
        public string amount { get; set; }
        public string amount_old { get; set; }
        public string marketRetailPrice { get; set; }
        public string marketRetailPrice_old { get; set; }
        public string sampleDisplayPrice { get; set; }
        public string sampleDisplayPrice_old { get; set; }
        public string currency { get; set; }
        public string beginDate { get; set; }
        public string isChanged { get; set; }
        public string isTransfered { get; set; }
    }
    public class MaterialModel
    {
        public string matCode { get; set; }
        public string matName { get; set; }
        public string amount { get; set; }
        public string marketRetailPrice { get; set; }
        public string sampleDisplayPrice { get; set; }
        public string currency { get; set; }
        public string isChanged { get; set; }
        public string isTransfered { get; set; }
    }
    public class MaterialTransferModel
    {
        public string matCode { get; set; }
        public string matName { get; set; }
        public string amount { get; set; }
        public string mrp { get; set; }
        public string sdp { get; set; }
        public string beginDate { get; set; }
    }
}