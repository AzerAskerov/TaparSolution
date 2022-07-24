using Amazon.DynamoDBv2.DataModel;
using TaparSolution.Operations;

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
        public long partnerMessageid { get; set; }
        public PartnerActionEnum partnerResposeAction { get; set; }
        public string partnerReponseValue { get; set; }
        public DateTime respondedDate { get; set; }
        public long clientChatId { get; set; }

    }
}
