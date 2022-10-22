using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;

namespace TaparSolution.Operations
{
    public class PartnerRegisterBrandSettingOp : BaseOperation<PartnerRegisterBrandSettingModel>
    {
        public ComposeMessage message;
        string selectedBrand;

        string instructionText = $"Zəhmət olmasa satışı ilə məşğul olduğunuz avtomobil markalarını secin: Bunun üçün: \n" +
                 $" *1.* [@Tapar](tg://user?id=123456789) klikləyib  boşluq qoyun. \n" +
                 $" *2.* Markanın adını yazdqca siyahı çıxacaq. Həmin siyahıdan seçin.\n" +
                 $" *3.* Əgər başqa da marka əlavə etmək istəyirsinizdə 1ci addımdan təkrarlayın \n" +
                 $" *4.* Əgər seçimi bitirdinizsə Sonlandır düyməsini basın";


        public override async void Validate()
        {
            selectedBrand = Parameter.incomingMessage.message.text;

            if (!Brandtable.Fulllist().Select(x=>x.Brand).Contains(selectedBrand))
            {
                Result.AddError("Zəhmət olmasa siyahıdan seçin");
            }
        }
        public override async void DoExecute()
        {

          
            var EndButton = new Inline_Keyboard()
            {
                inline_keyboard = new List<List<Inline_keyboard>>()
        {
            new List<Inline_keyboard>(){ new Inline_keyboard()
                     {
                text= "Sonlandır",
                callback_data="endbrandselection"

            } }
        }
            };


            //initiating brand selecting proccess
            if (Parameter._lastMessage.Type== "RegionRequest")
            {

                message.text = instructionText;
                message.reply_markup = EndButton;
            }



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

                    using (GetRequestPriceOperation op = new())
                    {
                        var result = await op.ExecuteAsync(new()
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
                        clientChatId = long.Parse(Parameter._lastMessage.chat_id)
                    };

                    await Parameter._db.SaveOrUpdateReqRespCompotition(compotition);
                    #endregion

                    using (PutQueueTheRequestOperation op = new())
                    {
                        var result = await op.ExecuteAsync(new()
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

    public class PartnerRegisterBrandSettingModel : BaseOperationModel
    {
       public TelegramMessage incomingMessage { get; set; }
       public PartnerTable Partner { get; set; }
        public ComposedMessageTable _lastMessage  { get; set; }
    }
}
