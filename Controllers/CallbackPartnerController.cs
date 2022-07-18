using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using AWSServerless2.Helpers;
using AWSServerless2.Models;
using AWSServerless2.Models.DBTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace AWSServerless2.Controllers
{

    /*
 start - servisi yenidən başlatmaq
 stop -  servisi müvəqqəti dayandırmaq
 balans - qalan balansı göstər
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

            #region GetLastMessage
           
            var chatmessages = await db.GetLastMessage(chatid.ToString());
            ComposedMessageTable LastMessage = null;
            if (chatmessages.Any())
                LastMessage = chatmessages.OrderByDescending(x => x.messageid).FirstOrDefault();
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

            }
            #endregion

            ComposedMessageTable composeMessage = new ComposedMessageTable()
            {

                chat_id = input.message.chat.id.ToString(),
               origin="Partner",
               Type="PartnerRegistercommand",
               messagedate = DateTImeHelper.GetCurrentDate()


            };

       




          

            var sendresponse = await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage",WebClient.Partnerbottoken);

            composeMessage.messageid = composeMessage.messageid==0?sendresponse.result.message_id: composeMessage.messageid;
            await db.SaveOrUpdateMessage(composeMessage);
            return "OK";




            ;
        }
    }
}
