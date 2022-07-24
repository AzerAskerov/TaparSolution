using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaparSolution.Models;
using TaparSolution.Models.DBTable;

namespace TaparSolution.Controllers
{
    [Route("api/[controller]")]
    public class CallbackPartnerRegisterController : ControllerBase
    {
        /*
restart - Qeydiyyatı yenidən başla
   */
        private readonly IOptions<MyConfig> config;
        public CallbackPartnerRegisterController(IOptions<MyConfig> config)
        {
            this.config = config;
        }
        public async Task<string> FunctionHandler([FromBody] TelegramMessage input)
        {
            DynamoDbClient db = new DynamoDbClient(config);
          
           
            long chatid = (input.message ?? input.callback_query.message).chat.id;
            long userid = (input.message ?? input.callback_query.message).from.id;
            ComposedMessageTable composeMessage = new() { origin = "partnerregister",chat_id=chatid.ToString() };
            ComposeMessage responsemessage = new() { chat_id=chatid.ToString()};

            #region GetLastMessage

            var chatmessages = await db.GetLastMessage(chatid.ToString());
            ComposedMessageTable LastMessage = null ;
            if (chatmessages.Any())
                LastMessage = chatmessages.Where(x => x.origin == "partnerregister").OrderByDescending(x => x.messageid).FirstOrDefault();
            #endregion

            #region GetExisitngPartner
           
            var partner = await db.GetPartnerByUserId(chatid);
            PartnerTable Partner = null;
            if (partner.Any())
                Partner = partner.FirstOrDefault();
            #endregion

            #region restart registration

            if (input.message?.text=="/restart")
            {
                if (Partner is not null)
                {
                    if (Partner.status == "unapproved")
                    {
                        await db.DeletePartner(Partner);
                        Partner = null;
                       
                    }
                    else
                    {
                        responsemessage = new ComposeMessage()
                        {
                            chat_id = input.message.chat.id.ToString(),
                            text = $" ℹ Qeydiyyatı yenidən başlamaq mümkün olmadı. Qeydiyyat statusunuz buna uyğun deyil",



                        };

                        goto Endpoint;

                    }
                }
            }
            #endregion

            #region Defining begining of Registration
            if (input.message.text == "/start" || input.message?.text == "/restart")
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $" ℹ Zəhmət olmasa satış nöqtəsinin adını qeyd edin",
                    reply_markup=new ReplyKeyboardRemove()


                };
                if (Partner == null)
                {
                    Partner = new() { balance = 1000, status = "unapproved", partnerid = userid };
                    await db.SaveOrUpdatePartner(Partner);
                    composeMessage.Text = responsemessage.text;
                    composeMessage.Type = "PartnerName";
                }
                else if (Partner.status == "unapproved")
                {
                    responsemessage.text = $"Qeydiyyat üçün artıq müraciət olunub. Sorğunuz baxışdadır." +
                        $" Nəticə barədə məlumat veriləcək. Sorğu nömrəsi: {Partner.partnerid}";
                    composeMessage.Text = "alreadyregistered";
                    composeMessage.Type = "alreadyregistered";
                    goto Endpoint;
                }
                else if (Partner.status == "Approve")
                {
                    responsemessage.text = $"Qeydiyyat üçün artıq müraciət olunub. Qeydiyyatınız təsdiqlənib. Partnyor N: {Partner.partnerid}";
                    composeMessage.Text = "alreadyregistered";
                    composeMessage.Type = "alreadyregistered";
                    goto Endpoint;
                }
                else
                {
                    responsemessage.text = $"Qeydiyyat üçün artıq müraciət olunub. Sorğunuz təsdiq olunmayıb." +
                       $"Sorğu nömrəsi: {Partner.partnerid}";
                    composeMessage.Text = "alreadyregistered";
                    composeMessage.Type = "alreadyregistered";

                    goto Endpoint;
                }

            }
            #endregion


            #region setting partnername
            else if (LastMessage.Type == "PartnerName")
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Zəhmət olmasa ünvan üçün aşağıdakı düyməni basın",
                    reply_markup = new Keyboard()
                    {
                        one_time_keyboard = true,
                        keyboard = new List<List<Inline_keyboard>>()
                        {
                            new List<Inline_keyboard>()
                            {
                                new Inline_keyboard(){
                                    request_location=true,
                                    text="Unvanı avtomatik göndər"
                                }
                            }
                        }
                    }


                };

                Partner.fullName = input.message.text;
                await db.SaveOrUpdatePartner(Partner);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "locationRequest";


            }
            #endregion

            #region setting location
            else if (LastMessage.Type == "locationRequest")
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Zəhmət olmasa əlaqə üçün aşağıdakı düyməni basın",
                    reply_markup = new Keyboard()
                    {
                        one_time_keyboard = true,
                        keyboard = new List<List<Inline_keyboard>>()
                        {
                            new List<Inline_keyboard>()
                            {
                                new Inline_keyboard(){
                                    request_contact=true,
                                    text="Əlaqə nömrəsini avtomatik göndər"
                                }
                            }
                        }
                    }

                };


                Partner.location = input.message.location;
                await db.SaveOrUpdatePartner(Partner);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "ContactInfoRequest";


            }
            #endregion

            #region setting contact info
            else if (LastMessage.Type == "ContactInfoRequest")
            {
                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Zəhmət olmasa Magazanin on tərəfindən aydin gorunen seklini çəkib göndərin"

                };


                Partner.contactInfo = input.message.contact.phone_number;
                await db.SaveOrUpdatePartner(Partner);

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "PhotoRequest";


            }
            #endregion



            #region setting photo
            else if (LastMessage.Type == "PhotoRequest")
            {
                if (input.message.photo == null)
                {
                    responsemessage.text = "Zəhmət olmasa yuxarıdakı instruksiyaya uyğun məlumat göndərin";
                    goto Endpoint;
                }

                List<Inline_keyboard> buttons = new List<Inline_keyboard>();

                foreach (var r in db.GetAviableRegion())
                {
                    buttons.Add(new Inline_keyboard() { text = r });
                }

                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Zəhmət olmasa regionu secin",
                    reply_markup = new Keyboard()
                    {
                        keyboard = new List<List<Inline_keyboard>>()
                        {
                           buttons
                        }
                    }

                };
                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "RegionRequest";



                Partner.photo = input.message.photo.OrderByDescending(x => x.file_size).FirstOrDefault().file_id;
                await db.SaveOrUpdatePartner(Partner);
            }


            #endregion

            #region setting region
           else if(LastMessage.Type == "RegionRequest")
            {

                composeMessage.Text = responsemessage.text;
                composeMessage.Type = "EndRegister";

                Partner.region = input.message.text;
                await db.SaveOrUpdatePartner(Partner);

            }
            #endregion



            #region send to admin

            if (composeMessage.Type == "EndRegister")
            {


                ComposeMessage sendtoAdminPhoto = new()
                {
                    chat_id = WebClient.adminchatid,
                    caption = $@"
_Partner ID_: *{Partner.partnerid}*
_Fullname_: *{Partner.fullName}*,
_ContactInfo_: +*{Partner.contactInfo}*
",
                    photo = Partner.photo,

                }
                    ;

                ComposeMessage sendtoAdminLocation = new()
                {
                    chat_id = WebClient.adminchatid,
                    latitude = Partner.location.latitude,
                    longitude = Partner.location.longitude,
                    reply_markup = new Inline_Keyboard()
                    {

                        inline_keyboard = new List<List<Inline_keyboard>>()
                        {
                            new List<Inline_keyboard>()
                            {
                                new Inline_keyboard(){

                                    text="Approve",
                                    callback_data=$"{Partner.partnerid}:Approve"
                                },
                                new Inline_keyboard(){

                                    text="Decline",
                                    callback_data=$"{Partner.partnerid}:Decline"
                                },
                            }
                        }
                    }
                };

                freeimageresponse photolink = await WebClient.GeneratePhotoLinkoutside(Partner.photo,WebClient.Partnerregistertoken);

                sendtoAdminPhoto.photo = photolink.image.url;


                await WebClient.SendMessagePostAsync<SendMessageResponse>(sendtoAdminPhoto, "sendPhoto", WebClient.Admintoken);
                await WebClient.SendMessagePostAsync<SendMessageResponse>(sendtoAdminLocation, "sendLocation", WebClient.Admintoken);

                responsemessage = new ComposeMessage()
                {
                    chat_id = input.message.chat.id.ToString(),
                    text = $"Qeydiyyat sorğunuz baxılması və təsdiqi üçün nəzərə alındı. Nəticə buraya göndəriləcək.",
                    reply_markup = new ReplyKeyboardRemove()

                };
            }
                #endregion







              


                      

            Endpoint:
            var sendresponse = await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage", WebClient. Partnerregistertoken);

            composeMessage.messageid = composeMessage.messageid == 0 ? sendresponse.result.message_id : composeMessage.messageid;
            await db.SaveOrUpdateMessage(composeMessage);
            return "ok";

        }
    }
}
