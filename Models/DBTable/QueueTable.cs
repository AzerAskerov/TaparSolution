using Amazon.DynamoDBv2.DataModel;

namespace AWSServerless2.Models.DBTable
{
    [DynamoDBTable("Queue")]
    public class QueueTable
    {
        [DynamoDBHashKey]
        public long queueid
        {
            get;
            set;
        }
        public DateTime  proccess_after { get; set; }
        public long requestid { get; set; }
        public long partnerid  { get; set; }
        public queuestatus  status { get; set; }
        public int price { get; set; }
        public DateTime? proccededTime { get; set; }
    }

    public enum queuestatus
    {
        created=0,
        success=1,
        error=2
    }
}
