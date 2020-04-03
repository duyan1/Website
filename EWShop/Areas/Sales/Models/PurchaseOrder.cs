using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Sales.Models
{
    public class PurchaseOrder
    {
        public string numberPO { get; set; }
        public string orderDate { get; set; }
        public string requestedDate { get; set; }
        public string currency { get; set; }
        public string createdPO { get; set; }
        public string createdEmail { get; set; }
        public string orderType { get; set; }
        public string soldTo { get; set; }
        public string shipTo { get; set; }
        public string contactUser { get; set; }
        public string contactOtherUser { get; set; }
        public string remarks { get; set; }
        public string confirmed { get; set; }
        public string dateConfirmed { get; set; }
    }
    public class PurchaseOrderDetail {
        public string numberPO { get; set; }
        public string changeFlag { get; set; }
        public string matCode { get; set; }
        public string modelType { get; set; }
        public string plantCode { get; set; }
        public string quantity { get; set; }
        public string qtyConfirm { get; set; }
        public string price { get; set; }
        public string priceConfirm { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
        public string giftFlag { get; set; }
        public string isLock { get; set; }
    }
}