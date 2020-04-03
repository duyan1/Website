using System.Collections.Generic;

namespace EWShop.Areas.Admin.Models
{
    public class AnswerItem
    {
        public string QAId { get; set; }
        public string AId { get; set; }
        public string AContent { get; set; }
        public string Priority { get; set; }
        public string isCheck { get; set; }
        public string QIdNext { get; set; }
        public string QContentNext { get; set; }
    }
    public class RecordItem
    {
        public string ItemId { get; set; }
        public string ItemContent { get; set; }
    }
    public class QAItem
    {
        public RecordItem question { get; set; }
        public List<AnswerItem> answers { get; set; }
    }
}