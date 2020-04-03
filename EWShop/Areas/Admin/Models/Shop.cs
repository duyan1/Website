
namespace EWShop.Areas.Admin.Models
{
    public class ShopItem
    {
        public string shopId { get; set; }
        public string shopType { get; set; }
        public string shopGroup { get; set; }
        public string shopLevel { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string region { get; set; }
        public string area { get; set; }
        public string groupId { get; set; }
        public string groupCode { get; set; }
        public string groupName { get; set; }
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public string shopAddress { get; set; }
        public string isActive { get; set; }
        public string customerCode { get; set; }
        public string transUser { get; set; }
        public string transDate { get; set; }
        public string remarks { get; set; }
    }
}