using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class ResultReturn
    {
        public string code { get; set; }
        public string msg { get; set; }
    }
    public class ItemReturn
    {
        public string code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
    public class DataReturn
    {
        public string value { get; set; }
        public string text { get; set; }
    }
    public class InfoReturn
    {
        public string total { get; set; }
        public string valid { get; set; }
        public string inValid { get; set; }
        public string verify { get; set; }
    }
    public class ItemCompleted
    {
        public string id { get; set; }
        public string label { get; set; }
        public string value { get; set; }
    }
    public class Ministry
    {
        public string MId { get; set; }
        public string SRNum { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
    }
}