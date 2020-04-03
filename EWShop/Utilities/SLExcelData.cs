using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Utilities
{
    public class SLExcelStatus
    {
        public string Message { get; set; }
    }

    public class SLExcelData
    {
        public SLExcelStatus Status { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> DataRows { get; set; }
        public SLExcelData()
        {
            Status = new SLExcelStatus();
            Headers = new List<string>();
            DataRows = new List<List<string>>();
        }
    }
}