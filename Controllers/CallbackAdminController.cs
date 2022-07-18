using AWSServerless2.Models;
using AWSServerless2.Models.DBTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AWSServerless2.Controllers
{
    [Route("api/[controller]")]
    public class CallbackAdminController : ControllerBase
    {

        private readonly IOptions<MyConfig> config;
        public CallbackAdminController(IOptions<MyConfig> config)
        {
            this.config = config;
        }
        public async Task<string> FunctionHandler([FromBody] TelegramMessage input)
        {
            DynamoDbClient db = new DynamoDbClient(config);
            ComposeMessage responsemessage = new() ;
           
            long chatid = (input.message ?? input.callback_query.message).chat.id;
            long userid = (input.message ?? input.callback_query.message).from.id;
            long messageid = (input.message ?? input.callback_query.message).message_id;

            long partnerid = long.Parse(input.callback_query.data.Split(':')[0]);
            string action = input.callback_query.data.Split(':')[1];

            ComposedMessageTable composeMessage = new() { origin = "admin",chat_id=chatid.ToString() };


            #region GetLastMessage

            var chatmessages = await db.GetLastMessage(chatid.ToString());
            ComposedMessageTable LastMessage = null ;
            if (chatmessages.Any())
                LastMessage = chatmessages.Where(x => x.origin == "admin").OrderByDescending(x => x.messageid).FirstOrDefault();
            #endregion

            #region GetExisitngPartner
           
            var partner = await db.GetPartnerByUserId(partnerid);
            PartnerTable Partner = null;
            if (partner.Any())
                Partner = partner.FirstOrDefault();
            #endregion

            switch (action)
            {
                case "Approve":
                    Partner.status = action;
                    await db.SaveOrUpdatePartner(Partner);
                    composeMessage.messageid =(int)messageid;
                    composeMessage.Text = $"Partner {Partner.fullName} has been approved";
                    composeMessage.Type = $"PartnerApprove";
                    ComposeMessage partneresponse = new ComposeMessage()
                    {
                        chat_id = Partner.partnerid.ToString(),
                        text = "Sizin qeydiyyatınız təsdiq olundu "
                    };
                    await WebClient.SendMessagePostAsync<SendMessageResponse>(partneresponse, "sendMessage", WebClient.Partnerregistertoken);

                    responsemessage.text = composeMessage.Text;
                    responsemessage.chat_id = chatid.ToString();

                    MessageEditModel edit = new MessageEditModel()
                    {
                        chat_id = chatid.ToString(),
                        message_id = messageid
                    };

                    edit.reply_markup = new Inline_Keyboard() { inline_keyboard = new List<List<Inline_keyboard>>() {
                    new List<Inline_keyboard>()
                    {

                        new Inline_keyboard()
                        {
                            text="Approved",
                            callback_data="#"
                        }
                    }
                    } };

                    await WebClient.SendMessagePostAsync<object>(edit, "editMessageReplyMarkup",WebClient.Admintoken);
                    break;
                default:
                    break;
            }


            var sendresponse = await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage", WebClient. Admintoken);

            composeMessage.messageid = composeMessage.messageid == 0 ? sendresponse.result.message_id : composeMessage.messageid;
            await db.SaveOrUpdateMessage(composeMessage);
            return "ok";

        }
    }
}
