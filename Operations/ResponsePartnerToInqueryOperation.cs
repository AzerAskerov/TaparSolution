using System.Text.RegularExpressions;
using TaparSolution.Helpers;
using TaparSolution.Models;

namespace TaparSolution.Operations
{
    public class ResponsePartnerToInqueryOperation : BaseOperation<ResponsePartnerToInquery>
    {
        PartnerActionEnum action;
       public string requestNumber;
        long chatid;
       public ComposeMessage message = new();
        public override void Validate()
        {
            base.Validate();
            action =(PartnerActionEnum)Enum.Parse<PartnerActionEnum>(Parameter.incoming.callback_query.data);
            requestNumber = Regex.Match(Parameter.incoming.callback_query.message.caption, ".*?(Sorğu N:[^.]*)")
                .Groups[0].Value
                .Replace("Sorğu N:","")
                .Trim();
            chatid = Parameter.incoming.callback_query.message.chat.id;

        }
        public override void DoExecute()
        {
            //get inline answer input from partner base on action

            //first action : var - exist
            // we should generate messsage that consist of two part. first part is request number 
            // second part action part. LIKE: ~691607149~ nomreli sorgu ucun ~qiymeti~ yazin. this format need us then parse from it 
            //AND also force reply with place holder base on action
             message = TelegramMessageComposerHelper.JustInformation(chatid.ToString(),"");
            switch (action)
            {
                case PartnerActionEnum.exist:
                    message.text = $"~{requestNumber}~nomreli sorgu ucun ~qiymət~yazin";
                    message.reply_markup = new ForceReply() { force_reply = true, input_field_placeholder = "Qiyməti bura yazın" };
                    break;
                case PartnerActionEnum.notexist:
                    break;
                case PartnerActionEnum.question:
                    message.text = $"~{requestNumber}~nomreli sorgu üzrə ~sualınızı~ verin";
                    message.reply_markup = new ForceReply() { force_reply = true, input_field_placeholder = "Sualı bura yazın" };
                    break;
                default:
                    break;
            }
        }
    }

    public  class ResponsePartnerToInquery:BaseOperationModel
    {
        public TelegramMessage incoming { get; set; }

    }

   public enum PartnerActionEnum
    {
        exist,
        notexist,
        question

    }
}
