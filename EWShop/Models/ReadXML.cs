using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace EWShop.Models
{
    public class ReadXML
    {
        private static XmlDocument xmlDoc;
        public static XmlNodeList InitializeCulture()
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath("~/App_Data/status.xml"));
            return xmlDoc.GetElementsByTagName("obj");
        }
        public static string GetText(string objid, XmlNodeList nodes)
        {
            string result = "";
            foreach(XmlNode no in nodes)
            {
                if(no.Attributes.Item(0).InnerText == objid)
                {
                    result = no.FirstChild.InnerText;
                    return result;
                }
            }
            return result;
        }
    }
}