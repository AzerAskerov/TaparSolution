using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;

namespace TaparSolution.Operations
{
    public class DistributeRequestToPartnerOperation : BaseOperation<DistributeRequestToPartnerModel>
    {
     

        public override async void DoExecute()
        {
            
            var currentrequest = await Parameter._db.GetRequestByOid(Parameter._lastMessage.request_oid);

            //getting  corresponding partners
            List<PartnerTable> partnerlist = await Parameter._db.GetPartnerByBrandAndRegionSubscription(currentrequest.selected_brand, currentrequest.regions);


            // if there is any partner that can receive request
            if (partnerlist.Count() > 0)
            {
              
                foreach (var p in partnerlist)
                {

#region getting request price
                  
                    int price;

                    using (GetRequestPriceOperation op =new ())
                    {
                        var result = await op.ExecuteAsync(new ()
                        {
                            _partner = p
                        });

                        price = op.Price;
                    }
                    #endregion

                    #region creating compotition
                    ReqResCompositionTable compotition = new ReqResCompositionTable()
                    {
                        compid = UniqueGeneratorHelper.UUDGenerate(),
                        partnerid = p.partnerid,
                        price = price,
                        requestid = currentrequest.requestid,
                    };

                  await  Parameter._db.SaveOrUpdateReqRespCompotition(compotition);
                    #endregion

                    using (PutQueueTheRequestOperation op = new())
                    {
                        var result = await op.ExecuteAsync(new ()
                        {
                           
                            queue = new QueueTable()
                            {
                                queueid = UniqueGeneratorHelper.UUDGenerate(),
                                type = QueueTypeEnum.DistributionToPartner,
                                proccess_after = DateTImeHelper.GetCurrentDate().AddMinutes(p.rate),
                                status = queuestatus.created,
                                identifier = compotition.compid

                            }
                        });
                    }

                

                }
            }
            else
            {
                await WebClient.SendMessagePostAsync<SendMessageResponse>
                    (
                    TelegramMessageComposerHelper.JustInformation(Parameter._lastMessage.chat_id, "Hec bir partnor tapilmadi"),
                    WebClient.SendMessageMehtod,
                    WebClient.Clientbottoken
                    );
            }

        }
    }

    public class DistributeRequestToPartnerModel:BaseOperationModel
    {
    public ComposedMessageTable _lastMessage { get; set; }  
    }
}
