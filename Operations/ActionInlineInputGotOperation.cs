using System.Linq;
using System.Text.RegularExpressions;
using TaparSolution.Helpers;
using TaparSolution.Models;

namespace TaparSolution.Operations
{
    public class ActionInlineInputGotOperation : BaseOperation<ActionInlineInputGotModel>
    {
        string requestnumber;
        PartnerActionEnum action;
        string respondedtext;
        public ComposeMessage message = new();

        public override void Validate()
        {
            base.Validate();
            Regex regex = new Regex("(?:~!?(\\w+)~)");
            var matches = regex.Matches(Parameter.incoming.message.reply_to_message.text);
           
            requestnumber = matches[0].Groups[1].Value;

            switch (matches[1].Groups[1].Value)
            {
                case "qiymət":
                    action = PartnerActionEnum.exist;
                    break;
                default:
                    break;
            }


            respondedtext = Parameter.incoming.message.text;
        }
        public override async void DoExecute()
        {

            if (action == PartnerActionEnum.exist)
            {


                var requestRelatedMessage =  Parameter._db.GetAllPartnerMessageByCurrentRequest(long.Parse(requestnumber)).Result;

                var mainMessageHistoryforEdit = requestRelatedMessage.FirstOrDefault(x => x.Type == "QueryToPartnerSent");

                var ratherthanmainMessage = requestRelatedMessage.Where(x => x != mainMessageHistoryforEdit).ToList();

                var currentrequest = Parameter._db.GetRequestByOid(long.Parse(requestnumber)).Result;
                var currentpartner = Parameter._db.GetPartnerByUserId(long.Parse(mainMessageHistoryforEdit?.chat_id)).Result.FirstOrDefault();
                var currentcompotition = Parameter._db.GetReqRespComotitionByPartnerAndRequest(currentrequest.requestid, currentpartner.partnerid).Result.FirstOrDefault();

                currentcompotition.partnerReponseValue = respondedtext;
                currentcompotition.partnerResposeAction = action;
                currentcompotition.respondedDate = DateTImeHelper.GetCurrentDate();


               

                ComposeMessage mainMessageContent = TelegramMessageComposerHelper.PartnerInqueryMessage(currentrequest, currentcompotition, currentpartner);

                mainMessageContent.message_id = mainMessageHistoryforEdit.messageid;
                mainMessageContent.reply_markup = null;
                mainMessageContent.caption += mainMessageContent.text + $"\n " +
                    $"\n *Sorğuya cavab:*" +
                    $"\n✅ Var. Qiymət:{respondedtext}";

                 WebClient.SendMessagePostAsync<object>(mainMessageContent, "editMessageCaption", WebClient.Partnerbottoken).Wait();

                //deleting redundant messages

                foreach (var item in ratherthanmainMessage)
                {
                    var deletemessagemodel = new { chat_id = item.chat_id, message_id = item.messageid };
                     WebClient.SendMessagePostAsync<object>(deletemessagemodel, "deleteMessage", WebClient.Partnerbottoken).Wait();
                }

                var deletemessage = new { chat_id = mainMessageHistoryforEdit.chat_id, message_id = Parameter.incoming.message.message_id };
                WebClient.SendMessagePostAsync<object>(deletemessage, "deleteMessage", WebClient.Partnerbottoken).Wait();


                Parameter._db.SaveOrUpdateReqRespCompotition(currentcompotition).Wait();

                // send notification to client about response of partner
              var responseback=  TelegramMessageComposerHelper.ResponseBackToClient(currentpartner, currentcompotition, currentrequest);
                WebClient.SendMessagePostAsync<SendMessageResponse>(responseback, WebClient.SendPhotoMehtod, WebClient.Clientbottoken).Wait();
            }


        }
    }

    public class ActionInlineInputGotModel:BaseOperationModel
    {
        public TelegramMessage incoming { get; set; }
    }
}