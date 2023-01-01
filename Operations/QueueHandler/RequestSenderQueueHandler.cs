using Amazon.DynamoDBv2;
using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;
using Microsoft.Extensions.Options;

namespace TaparSolution.Operations.QueueHandler
{
    public class RequestSenderQueueHandler : BaseOperation<RequestSenderQueueHandlerModel>
    {
        List<QueueTable> queues = new();




        public override void Validate()
        {
            base.Validate();

//#if DEBUG
//            queues = Parameter._db.GetQueueById(638076920232629390).Result;
//#else
   queues = Parameter._db.GetUnproccededQueue().Result;
//#endif




        }

        public override void DoExecute()
        {

            foreach (var _q in queues)
            {
                // collect required

                var currentcompotition = Parameter._db.GetReqRespCompotitionByOid(_q.identifier).Result;
                var currentrequest = Parameter._db.GetRequestByOid(currentcompotition.requestid).Result;
                var currentpartner = Parameter._db.GetPartnerByUserId(currentcompotition.partnerid).Result.FirstOrDefault();

                //  //bundling request for partners

                ComposeMessage Texpassport= TelegramMessageComposerHelper.PartnerInqueryMessage(currentrequest, currentcompotition, currentpartner);
              

                //send bundled reuqest message
                SendMessageResponse response = WebClient.SendMessagePostAsync<SendMessageResponse>(Texpassport, "sendPhoto", WebClient.Partnerbottoken).Result;
                if (response.ok)
                {
                    _q.status = queuestatus.success;
                    _q.proccededTime = DateTImeHelper.GetCurrentDate();
                    Parameter._db.SaveOrUpdateQueue(_q).Wait();
                    //TODO Deduct Partner Balance
                    currentpartner.balance -= currentcompotition.price;
                    Parameter._db.SaveOrUpdatePartner(currentpartner).Wait();
                    currentcompotition.partnerMessageid = response.result.message_id;

                    Parameter._db.SaveOrUpdateMessage(new ComposedMessageTable()
                    {
                        chat_id = Texpassport.chat_id,
                        messagedate = DateTImeHelper.GetCurrentDate(),
                        messageid = response.result.message_id,
                        messageoid = UniqueGeneratorHelper.UUDGenerate(),
                        origin = "Partner",
                        request_oid = currentrequest.requestid,
                        Type = "QueryToPartnerSent",
                        Text = $"{currentrequest.requestid} was send to partner {currentpartner.partnerid}"
                    }).Wait();

                }
                else
                    Result.AddInformation($"send response error: {response.result.text}");
            }
        }
       
    }


    public class RequestSenderQueueHandlerModel : BaseOperationModel
    {

    }

}
