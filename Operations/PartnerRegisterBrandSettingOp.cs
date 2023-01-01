using Amazon.DynamoDBv2.Model;
using System.Reflection.Metadata;
using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;

namespace TaparSolution.Operations
{
    public class PartnerRegisterBrandSettingOp : BaseOperation<PartnerRegisterBrandSettingModel>
    {
        public ComposeMessage message;
        string selectedBrand;
        private const string NotFromListError = "Zəhmət olmasa siyahıdan seçin";


        public override async void Validate()
        {
            selectedBrand = Parameter.incomingMessage.message?.text;

            if (!Brandtable.Fulllist().Select(x=>x.Brand).Contains(selectedBrand)&& selectedBrand!=null)
            {
                Result.AddError(NotFromListError);
            }
        }

        public override async void DoFinalize()
        {

            if (Result.selectedError == NotFromListError)
            {
                Parameter.OutputComposedMessage.Type = "StartBrandSelection";
            }
           
            base.DoFinalize();
        }
        public override  void DoExecute()
        {
            var useraction = Parameter.incomingMessage.callback_query?.data;
            if (useraction== "endbrandselection")
            {
                Parameter.OutputComposedMessage.Type = "endbrandselection";
                return;
            }

            List<string> brands = new List<string>();
            if (Parameter.Partner.subscribedBrands!=null)
            {
                brands.AddRange(Parameter.Partner.subscribedBrands);
            }
            brands.Add(selectedBrand.ToUpper());
            Parameter.Partner.subscribedBrands = brands;
            Parameter._db.SaveOrUpdatePartner(Parameter.Partner).Wait();
            message = new ComposeMessage()
            {
                chat_id = Parameter._lastMessage.chat_id.ToString(),
                text = "Daha da marka əlavə etmək üçün eyni addımları edin. Yaxud yuxaridakı *Sonlandır* düyməsini basın"                
            };
            Parameter.OutputComposedMessage.Type = "StartBrandSelection";
        }
    }

    public class PartnerRegisterBrandSettingModel : BaseOperationModel
    {
       public TelegramMessage incomingMessage { get; set; }
       public PartnerTable Partner { get; set; }
        public ComposedMessageTable _lastMessage  { get; set; }
        public ComposedMessageTable OutputComposedMessage { get; set; }


    }
}
