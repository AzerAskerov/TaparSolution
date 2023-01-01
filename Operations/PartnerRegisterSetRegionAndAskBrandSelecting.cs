using Amazon.DynamoDBv2.Model;
using System.Reflection.Metadata;
using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;

namespace TaparSolution.Operations
{
    public class PartnerRegisterSetRegionAndAskBrandSelecting : BaseOperation<PartnerRegisterSetRegionAndAskBrandSelectingModel>
    {
        public ComposeMessage message;
        string selectedRegion;
        private const string NotFromListError = "Zəhmət olmasa təklif olunan regionlardan seçin";
       

        string instructionText = $"Zəhmət olmasa satışı ilə məşğul olduğunuz avtomobil markalarını secin: Bunun üçün: \n" +
                 $" *1.* @Taparbot yazıb boşluq qoyun. \n" +
                 $" *2.* Daha sonra markanın adını yazdqca siyahı çıxacaq. Həmin siyahıdan seçin.\n" +
                 $" *3.* Əgər başqa da marka əlavə etmək istəyirsinizdə 1ci addımdan təkrarlayın \n" +
                 $" *4.* Əgər seçimi bitirdinizsə *Sonlandır* düyməsini basın";


        public override async void Validate()
        {
            selectedRegion = Parameter.incomingMessage.message.text;


            if (!Parameter._db.GetAviableRegion().Contains(selectedRegion))
            {
                Result.AddError(NotFromListError);
            }
        }

        public override async void DoFinalize()
        {
          
            if (Result.selectedError == NotFromListError)
            {
                Parameter.OutputComposedMessage.Type = "RegionRequest";
            }
            base.DoFinalize();

        }
        public override  void DoExecute()
        {




            Parameter.Partner.region  = selectedRegion;
            Parameter._db.SaveOrUpdatePartner(Parameter.Partner).Wait();

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

            message = new ComposeMessage()
            {
                chat_id = Parameter._lastMessage.chat_id.ToString(),
                text = instructionText,
                reply_markup = EndButton
            };
            Parameter.OutputComposedMessage.Type = "StartBrandSelection";
           
         
        }
    }

    public class PartnerRegisterSetRegionAndAskBrandSelectingModel : BaseOperationModel
    {
       public TelegramMessage incomingMessage { get; set; }
       public PartnerTable Partner { get; set; }
        public ComposedMessageTable _lastMessage  { get; set; }
        public ComposedMessageTable OutputComposedMessage { get; set; }

    }
}
