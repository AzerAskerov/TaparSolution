using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using TaparSolution.Helpers;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;
using TaparSolution.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace TaparSolution.Controllers
{
    [Route("api/[controller]")]
    public class CallbackController : ControllerBase
    {
        private readonly IOptions<MyConfig> config;
        public CallbackController(IOptions<MyConfig> config)
        {
            this.config = config;
        }
        public async Task<string> FunctionHandler([FromBody] TelegramMessage input)
        {
            DynamoDbClient db = new DynamoDbClient(config);

            List<string> possibleRegions = new List<string>() { "Bakı", "Sumqayıt", "Xırdalan" };

            #region GetBrandList
            if (input.inline_query != null)
            {
            
                string[] queryparams = input.inline_query.query.ToUpper().Split(' ');
                AnswerInlineQuery answer = new AnswerInlineQuery()
                {
                    inline_query_id = input.inline_query.id,
                    results = new List<InlineArticleAnswer>()

                };
                foreach (var b in Brandtable.Fulllist().Where(x => x.Brand.ToUpper().Contains(queryparams[0])))
                {
                    answer.results.Add(new InlineArticleAnswer()
                    {
                        id = b.brand_oid,
                        title = b.Brand,
                        thumb_url = b.src,
                        input_message_content = new Input_message_content() { message_text = b.Brand }
                    }
                    );
                }

                await WebClient.SendMessagePostAsync<object>(answer, "answerInlineQuery");
                return "OK";

            }
            #endregion

           
            ComposeMessage responsemessage = new ComposeMessage();

            #region GetLastMessage
            long chatid = (input.message ?? input.callback_query.message).chat.id;
            var chatmessages = await db.GetLastMessage(chatid.ToString());
            ComposedMessageTable LastMessage = new() { origin="client"};
            if (chatmessages.Any())
                LastMessage = chatmessages.Where(x=>x.origin == "client").OrderByDescending(x => x.messageid).FirstOrDefault();
            #endregion

            #region selecting Region
            if (input.callback_query != null)
            {
               

                var currentrequest = await db.GetRequestByOid(LastMessage.request_oid);
                var targetregion = input.callback_query.data.Split('-')[1];
                var targetoperation = input.callback_query.data.Split('-')[0];

                switch (targetoperation)
                {
                    case "selected":
                        if (currentrequest.regions.Any(x => x.Equals(targetregion)))
                        {
                            currentrequest.regions.Remove(targetregion);
                        }
                        break;

                    case "notselected":
                        if (currentrequest.regions==null || !currentrequest.regions.Any(x => x.Equals(targetregion)))
                        {
                            if (currentrequest.regions==null)
                                currentrequest.regions = new List<string>();

                            currentrequest.regions.Add(targetregion);
                        }

                        break;
                    default:
                        break;
                }

                MessageEditModel edit = new MessageEditModel()
                {
                    chat_id = LastMessage.chat_id,
                    message_id = LastMessage.messageid
                };

                edit.reply_markup = new Inline_Keyboard() { inline_keyboard = new List<List<Inline_keyboard>>() };

                foreach (var r in possibleRegions)
                {
                    var operation = currentrequest.regions.Any(x => x == r) ? "selected" : "notselected";
                    string buttontext;
                    string callbackdata;
                    if (operation == "selected")
                    {
                        buttontext = $"✅-{r}";
                        callbackdata = $"selected-{r}";
                    }

                    else
                    {
                        buttontext = $"❌-{r}";
                        callbackdata = $"notselected-{r}";

                    }

                    ((Inline_Keyboard)edit.reply_markup).inline_keyboard.Add(
                        new List<Inline_keyboard>()
                        {


                           new Inline_keyboard()
                           {
                               text =buttontext,
                               callback_data=callbackdata
                           }
                       }
                       );
                }
           
                await db.SaveOrUpdateRequest(currentrequest);
                await WebClient.SendMessagePostAsync<object>(edit, "editMessageReplyMarkup");
                return "wait";
            }
            #endregion

            ComposedMessageTable composeMessage = new ComposedMessageTable()
            {
                messageoid = UniqueGeneratorHelper.UUDGenerate(),
                chat_id = input.message?.chat.id.ToString(),
                origin = "client",
                messagedate = DateTImeHelper.GetCurrentDate()


            };

            #region Defining begining of chat
            if (input.message.text == "/start")
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $" ℹ Zəhmət olmasa aşağıda olan \n ⏬⏬'*Yeni sorğu*'⏬⏬ düyməsi ilə başlayın",

                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            }
                        }
                        }
                    }

                };

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "initialize";
            }
            #endregion






            #region Start new request
            else if (input.message.text == "✋✋✋Yeni sorğu ✋✋✋" ||( input.message.text!=null && input.message.text.ToUpper().Contains("SALAM")))
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"ℹZəhmət olmasa Marka axtara basdıqdan sonra 🚕 Avtomobilinizin *Marka*sını yazdiqca çıxacaq *siyahıdan* secin",

                    reply_markup = new Inline_Keyboard()
                    {
                        inline_keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="Marka axtar 🔍",
                                switch_inline_query_current_chat=""
                            }
                        }
                        }
                    }

                };

              

                await db.SaveOrUpdateRequest(new ClientRequestTable()
                {
                    chat_id = input.message.chat.id,
                    request_date = DateTImeHelper.GetCurrentDate(),
                    requestid = input.update_id,
                    user_id = input.message.from.id
                });


                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "Brandselect";
                composeMessage.request_oid = input.update_id;
            }
            #endregion

            #region Setting selected brand
            else if (LastMessage.Type == "Brandselect")
            {
                ComposeMessage regionmessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text="Seçin",



                };

                regionmessage.reply_markup = new Inline_Keyboard() { inline_keyboard = new List<List<Inline_keyboard>>() };

                foreach (var r in possibleRegions)
                {
                    
                    string buttontext= $"❌-{r}";
                    string callbackdata= $"notselected-{r}";
                   

                    ((Inline_Keyboard)regionmessage.reply_markup).inline_keyboard.Add(
                        new List<Inline_keyboard>()
                        {


                           new Inline_keyboard()
                           {
                               text =buttontext,
                               callback_data=callbackdata
                           }
                       }
                       );
                }

             SendMessageResponse response =  await  WebClient.SendMessagePostAsync<SendMessageResponse>(regionmessage, "sendMessage");

                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"ℹ Zəhmət olmasa yuxaridan uyğun region(lar)ı seçin. Seçimi bitirdikdən sonra *Növbəti* düyəmisini basın",

                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            },
                            new Inline_keyboard()
                            {
                                text="Növbəti"
                            }

                        }
                        }
                    }

                };


         
                var currentrequest = await db.GetRequestByOid(LastMessage.request_oid);
                currentrequest.regions = new List<string>();
                currentrequest.selected_brand = input.message.text;
                await db.SaveOrUpdateRequest(currentrequest);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "regionselected";
                composeMessage.request_oid = LastMessage.request_oid;
                composeMessage.messageid = response.result.message_id;




            }
            #endregion

            #region Setting selected region
            else if (LastMessage.Type == "regionselected" && input.message.text== "Növbəti")
            {

                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"ℹ Zəhmət olmasa axtardığınız *avto hissəni ətraflı*  qeyd edin.məs: qabaq sol asqı",

                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            }
                        }
                        }
                    }

                };
                var currentrequest = await db.GetRequestByOid(LastMessage.request_oid);
               
                await db.SaveOrUpdateRequest(currentrequest);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "autopart";
                composeMessage.request_oid = LastMessage.request_oid;




            }
            #endregion


            #region setting auto part
            else if (LastMessage.Type == "autopart")
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Zəhmət olmasa texniki sənədin (texpasport) şəklini göndərin",

                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            }
                        }
                        }
                    }

                };
                var currentrequest = await db.GetRequestByOid(LastMessage.request_oid);
                currentrequest.auto_part = input.message.text;
                await db.SaveOrUpdateRequest(currentrequest);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "texpasport";
                composeMessage.request_oid = LastMessage.request_oid;

            }
            #endregion

            #region setting Tech Document
            else if (LastMessage.Type == "texpasport")
            {
                if (input.message.photo == null)
                    return "Wait";
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Varsa avto hissənin (zapcast) şəkillərini yükləyin və sonda yaxud yoxdursa 'Yükləməni sonlandır' düyməsini basın",

                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            },
                            new Inline_keyboard()
                            {
                                text = "Yükləməni sonlandır"
                            }
                        }
                        }
                    }

                };
                var currentrequest = await db.GetRequestByOid(LastMessage.request_oid);
                currentrequest.tech_document_img = input.message.photo?.LastOrDefault()?.file_id;
                await db.SaveOrUpdateRequest(currentrequest);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "autopartimage";
                composeMessage.request_oid = LastMessage.request_oid;

            }
            #endregion

            #region setting autopart images
            else if (LastMessage.Type == "autopartimage")
            {
                if (input.message.text != "Yükləməni sonlandır")
                {
                    if (input.message.photo != null)
                    {
                        var requestforphoto = await db.GetRequestByOid(LastMessage.request_oid);
                        List<string> exisitnglist = new List<string>();
                        if (requestforphoto.part_imgs?.Count > 0)
                        {
                            exisitnglist = requestforphoto.part_imgs;
                        }
                        exisitnglist.Add(input.message.photo?.LastOrDefault()?.file_id);
                        requestforphoto.part_imgs = exisitnglist;
                        await db.SaveOrUpdateRequest(requestforphoto);
                    }
                    return "Wait";
                }
                else
                {
                    responsemessage = new ComposeMessage()
                    {
                        chat_id = input.message.chat.id.ToString(),
                        text = $"Sorğunuz aidiyyatı mağazalara göndərildi. Cavab gəldikcə sizə bildiriləcək." +
                        $"Sorğu İD:{LastMessage?.request_oid.ToString()}",

                        reply_markup = new Keyboard()
                        {
                            keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            }
                        }
                        }
                        }

                    };
                    composeMessage.Text = responsemessage.text;
                    composeMessage.Type = "requestsent";
                    composeMessage.request_oid = LastMessage.request_oid;


                    #region Partner Inquery Sending
                    using (DistributeRequestToPartnerOperation op = new())
                    {
                        DistributeRequestToPartnerModel model = new()
                        {
                            
                            _lastMessage = LastMessage
                           
                        };

                    
                        await  op.ExecuteAsync(model);
                    }
                    #endregion
                   

                  

                    



                }


            }
            #endregion

            else
            {

                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Zəhmət olmasa yuxarıdakı instruksiyaya uyğun edin yaxud Yeni sorğu düyməsini sıxın",

                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>() {
                        new List<Inline_keyboard>()
                        {
                            new Inline_keyboard()
                            {
                                text="✋✋✋Yeni sorğu ✋✋✋"
                            }
                        }
                        }
                    }

                };
                composeMessage.Text = responsemessage.text;
                composeMessage.Type = LastMessage.Type;
                composeMessage.request_oid = LastMessage.request_oid;
            }

            var sendresponse = await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage");

            composeMessage.messageid = composeMessage.messageid==0?sendresponse.result.message_id: composeMessage.messageid;
            await db.SaveOrUpdateMessage(composeMessage);
            return "OK";




            ;
        }
    }
}
