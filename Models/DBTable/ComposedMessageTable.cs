using Amazon.DynamoDBv2.DataModel;

namespace TaparSolution.Models.DBTable
{
    [DynamoDBTable("composedMessage")]
    public class ComposedMessageTable
    {
        [DynamoDBHashKey]
        public int messageid
        {
            get;
            set;
        }
        public string Text { get; set; }
        public string Type { get; set; }
        public string  chat_id { get; set; }
        public long request_oid { get; set; }
        public string origin { get; set; }
        public DateTime messagedate { get; set; }

    }
}
