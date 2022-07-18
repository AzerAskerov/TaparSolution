using Amazon.DynamoDBv2.DataModel;

namespace TaparSolution.Models.DBTable
{
    [DynamoDBTable("ReqResComposition")]
    public class ReqResCompositionTable
    {
        [DynamoDBHashKey]
        public long compid { get; set; }
        public long requestid { get; set; }
        public long partnerid { get; set; }
        public int price { get; set; }
    }
}
