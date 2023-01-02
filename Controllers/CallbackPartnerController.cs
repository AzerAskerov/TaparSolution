using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using TaparSolution.Operations;

namespace TaparSolution.Controllers
{

    /*
 start - servisi yenidən başlatmaq
 stop -  servisi müvəqqəti dayandırmaq
 balans - qalan balansı göstər
 addbalans - balans artırmaq
 subscribe - yeni markaya üzv olmaq
 unsubscribe - üzv olduğum markanı ləğv etmək
 register - partnyor olmaq üçün qeydiyyat
 profile - istifadəçi məlumatları
 pricelist - mövcud qiymətlər
    */

    [Route("api/[controller]")]
    public class CallbackPartnerController : ControllerBase
    {
        private readonly IOptions<MyConfig> config;
        public CallbackPartnerController(IOptions<MyConfig> config)
        {
            this.config = config;
        }
        public async Task<string> FunctionHandler([FromBody] TelegramMessage input)
        {
            DynamoDbClient db = new DynamoDbClient(config);
            long chatid = (input.message ?? input.callback_query.message).chat.id;
            long userid = (input.message ?? input.callback_query.message).from.id;




            ComposeMessage responsemessage = new ComposeMessage()
            {
                chat_id= chatid.ToString()
            };
            ComposedMessageTable composeMessage = new()
            {
                messageoid = UniqueGeneratorHelper.UUDGenerate(),
                messagedate = DateTImeHelper.GetCurrentDate(),
                origin = "Partner",
                chat_id = chatid.ToString(),

            };

            #region GetLastMessage

          var chatmessages = await db.GetLastMessage(chatid.ToString());
            ComposedMessageTable LastMessage = null;
            if (chatmessages.Any())
                LastMessage = chatmessages.Where(x => x.origin == "Partner").OrderByDescending(x => x.messageid).FirstOrDefault();
            #endregion

            #region Registeration
            if (input.message?.text=="/register")
            {
                responsemessage.text = "Qeydiyyat üçün aşağıdakı düyməni basın";
                responsemessage.reply_markup = new Inline_Keyboard()
                {
                    inline_keyboard = new List<List<Inline_keyboard>>()
                    {
                      new List<Inline_keyboard>()
                      {
new Inline_keyboard()
{
 url="http://t.me/TaparPartnerRegister_bot",
 text= "Qeydiyyata keçid"
}
                      }
                    }
                };
                composeMessage.Type = "PartnerRegistercommand";                 
            

            }
            #endregion



            #region initial action button clicked
            if (input.callback_query is not null)
            {
                switch (input.callback_query.data)
                {
                    case "exist":
                    case "notexist":
                    case "question":
                        using (ResponsePartnerToInqueryOperation op = new () )
                        {
                            await op.ExecuteAsync(new ResponsePartnerToInquery()
                            {
                                incoming = input
                            });
                            responsemessage = op.message;
                            composeMessage.request_oid = long.Parse( op.requestNumber);
                        }

                        composeMessage.Type = "PartnerActionToRequest";
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region inline response text to request message replied
            if (input.message?.reply_to_message is not null)
            {
                using (ActionInlineInputGotOperation op = new())
                {
                    await op.ExecuteAsync(new ActionInlineInputGotModel()
                    {
                        incoming = input
                    });
                    responsemessage = op.message;
                }

                composeMessage.Type = "PartnerInlineAnswerGot";
            }
            #endregion



            if(responsemessage.chat_id is not null)
            {
                var sendresponse = await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage", WebClient.Partnerbottoken);
                composeMessage.messageid = composeMessage.messageid == 0 ? sendresponse.result.message_id : composeMessage.messageid;
                await db.SaveOrUpdateMessage(composeMessage);
            }

           
            return "OK";




            ;
        }
    }
}
