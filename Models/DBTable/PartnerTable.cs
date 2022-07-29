using Amazon.DynamoDBv2.DataModel;

namespace TaparSolution.Models.DBTable
{
    [DynamoDBTable("Partners")]
    public class PartnerTable
    {
        [DynamoDBHashKey]
        public long partnerid
        {
            get;
            set;
        }
        public string fullName { get; set; }
        public TelegramLocation location { get; set; }
        public string address { get; set; }
        public string  contactInfo { get; set; }
        public int balance { get; set; }
        public List<string> subscribedBrands { get; set; }
        public string region { get; set; }
        public string status { get; set; }
        public string photo { get; set; }
        public int rate { get; set; }
        public DateTime createdDate { get; set; }

    }
}
