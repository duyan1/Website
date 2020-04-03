using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class SupportRequest
    {
        public string SenderName  {get;set;}
        public string SenderAddress { get; set; }
        public string SenderCoName { get; set; }
        public string SenderPhone { get; set; }
        public string SenderFax { get; set; }
        public string SenderEmail { get; set; }
        public string MessageBody { get; set; }
        public int SupportRequestType { get; set; }
        public int MatID { get; set; }
        public string ModelOther { get; set; }
        public string captcha { get; set; }
    }
    public class Question
    {
        public string QId { get; set; }
        public string QContent { get; set; }
        public List<Answer> answers { get; set; }
    }
    public class Answer
    {
        public string QAId { get; set; }
        public string AId { get; set; }
        public string AContent { get; set; }
    }
}