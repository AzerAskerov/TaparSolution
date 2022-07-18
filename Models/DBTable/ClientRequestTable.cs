using Amazon.DynamoDBv2.DataModel;

namespace TaparSolution.Models.DBTable
{
    [DynamoDBTable("clientRequest")]
    public class ClientRequestTable
    {
        public long requestid { get; set; }
        public long user_id { get; set; }
        public long chat_id { get; set; }
        public DateTime request_date { get; set; }
        public string selected_brand { get; set; }
        public string auto_part { get; set; }
        public string tech_document_img { get; set; }
        public List<string> part_imgs { get; set; }
        public List<string> regions { get; set; }
    }
}
