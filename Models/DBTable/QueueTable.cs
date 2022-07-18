using Amazon.DynamoDBv2.DataModel;

namespace TaparSolution.Models.DBTable
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
      
        public queuestatus  status { get; set; }
      
        public DateTime? proccededTime { get; set; }
        public QueueTypeEnum type { get; set; }
        public long identifier { get; set; }
    }

    public enum queuestatus
    {
        created=0,
        success=1,
        error=2
    }
   public enum QueueTypeEnum
    {
        DistributionToPartner=1, // identifier of this queue is reqrescompotition id.
    }
}
