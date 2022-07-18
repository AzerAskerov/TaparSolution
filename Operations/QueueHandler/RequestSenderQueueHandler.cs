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
        IAmazonDynamoDB amazondb;

        DynamoDbClient db = DynamoDbClient.GetInstance();

        public override void Validate()
        {
            base.Validate();

            Result.AddInformation("validation started");
            queues = db.GetUnproccededQueue().Result;
        }

        public override void DoFinalize()
        {
            base.DoFinalize();
            Result.AddInformation("finalize");
        }
        public override  void DoExecute()
        {

            Result.AddInformation($"after queue call");

         
            foreach (var _q in queues)
            {
                Result.AddInformation("inside queue");
                var currentrequest =  db.GetRequestByOid(_q.requestid).Result;
                Result.AddInformation("got request");
                //  //bundling request for partners
                string info = $"*Ehtiyyat Hissəsinin məlumatlari*: \n" +
                    $"*Hissə:* _{currentrequest.auto_part}_\n" +
                    $"*Marka:* _{currentrequest.selected_brand}_\n" +
                    $"*Sorğu N:* _{currentrequest.requestid}_";


                //texpass message compose
                ComposeMessage Texpassport = new ComposeMessage();
                Texpassport.caption = info;
                freeimageresponse techpassportfilelinkresult =  WebClient.GeneratePhotoLinkoutside(currentrequest.tech_document_img).Result;
                Texpassport.photo = techpassportfilelinkresult.image.url;

                // TExpass end


                // partnerbundle.chat_id =
                Texpassport.chat_id = _q.partnerid.ToString();

                //TODO add buttons: Var, yoxdur, sikayet et, soz sorus
                //


                Result.AddInformation("before send message");
                //send bundled reuqest message
                SendMessageResponse response=  WebClient.SendMessagePostAsync<SendMessageResponse>(Texpassport, "sendPhoto", WebClient.Partnerbottoken).Result;
                if (response.ok)
                {
                    _q.status = queuestatus.success;
                    _q.proccededTime = DateTImeHelper.GetCurrentDate();
                    Parameter._db.SaveOrUpdateQueue(_q);
                    //TODO Deduct Partner Balance

                }
                else
                    Result.AddInformation($"send response error: {response.result.text}");
            }
        }
    }


    public class RequestSenderQueueHandlerModel:BaseOperationModel
    {
       
    }

}
