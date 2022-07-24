using TaparSolution.Models;
using TaparSolution.Models.DBTable;

namespace TaparSolution.Helpers
{
    public static class TelegramMessageComposerHelper
    {
        public static ComposeMessage JustInformation(string chatid, string info)
        {
            return new ComposeMessage()
            {
                text = info,
                chat_id = chatid
            };
        }

        public static  Task<SendMessageResponse> SendInfoToAdmin(string text)
        {
            return WebClient.SendMessagePostAsync<SendMessageResponse>(
                JustInformation(WebClient.adminchatid, text),
                WebClient.SendMessageMehtod,
                WebClient.Admintoken);
        }

        internal static ComposeMessage PartnerInqueryMessage(ClientRequestTable currentrequest, ReqResCompositionTable currentcompotition, PartnerTable currentpartner)
        {
            string info = $"*Ehtiyyat Hissəsinin məlumatlari*: \n" +
                     $"*Hissə:* _{currentrequest.auto_part}_\n" +
                     $"*Marka:* _{currentrequest.selected_brand}_\n" +
                     $"*Sorğu N:* _{currentrequest.requestid}_." +
                     $"\n *Balans:*\n" +
                     $"*sorğuya istifadə olunan:*_{currentcompotition.price}_" +
                     $"\n *qalan balans:* _{currentpartner.balance}_";


            //texpass message compose
            ComposeMessage Texpassport = new ComposeMessage();
            Texpassport.caption = info;
            freeimageresponse techpassportfilelinkresult = WebClient.GeneratePhotoLinkoutside(currentrequest.tech_document_img).Result;
            Texpassport.photo = techpassportfilelinkresult.image.url;
            Texpassport.chat_id = currentcompotition.partnerid.ToString();
            // TExpass end   

            Texpassport.reply_markup = new Inline_Keyboard()
            {
                inline_keyboard = new List<List<Inline_keyboard>>() {
                    new List<Inline_keyboard>()
                    {

                        new Inline_keyboard()
                        {
                            text="Var",
                            callback_data="exist"
                        },

                        new Inline_keyboard()
                        {
                            callback_data="notexist",
                            text="Yoxdur"
                        },

                        new Inline_keyboard()
                        {
                            text="Söz Soruş",
                            callback_data="question"
                        }
                    }
                    }
            };

            return Texpassport;
        }


        public static ComposeMessage ResponseBackToClient(PartnerTable currentpartner, ReqResCompositionTable currentcompotition, ClientRequestTable currentrequest)
        {
          var composedmessage = new ComposeMessage();

            composedmessage.chat_id = currentcompotition.clientChatId.ToString();

            switch (currentcompotition.partnerResposeAction)
            {
                case Operations.PartnerActionEnum.exist:

                    // compose text
                    composedmessage.caption = $"*GERİ DÖNÜŞ:*" +
                        $"\n *Mağaza adı:* _{currentpartner.fullName}_" +
                        $"\n *Ehtiyyat hissəsi:* _{currentrequest.auto_part}_" +
                        $"\n *Cavab:*_✅ Var. Qiymət:{currentcompotition.partnerReponseValue}_";
                    

                    // generate image
                    freeimageresponse partnerpointimage =WebClient.GeneratePhotoLinkoutside(currentpartner.photo,WebClient.Partnerregistertoken).Result;
                    composedmessage.photo = partnerpointimage.image.url;

                    //construct buttons
                    composedmessage.reply_markup = new Inline_Keyboard()
                    {
                        inline_keyboard = new List<List<Inline_keyboard>>()
                        {
                            new List<Inline_keyboard>()
                            {
                                new Inline_keyboard() {
                                    callback_data = "getPhone",
                                    text = "Əlaqə nömrəsi"
                                },
                                new Inline_keyboard() {
                                    callback_data = "getLocation",
                                    text = "Ünvan"
                                },

                            }
                        }
                    };

                    break;
                case Operations.PartnerActionEnum.notexist:
                    break;
                case Operations.PartnerActionEnum.question:
                    break;
                default:
                    break;
            }

          

            return composedmessage;
        }
    }
}
