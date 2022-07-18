using TaparSolution.Models;
using Newtonsoft.Json;
using System.Text;

namespace TaparSolution
{
    public class WebClient
    {
        public const string Clientbottoken = "5307056020:AAEIcB_eN73QRKGmmbryzbnHrKF3E42V7gg";
        public const string Partnerbottoken = "5455666053:AAFTf-tefVzw5wjFVnNxzrCCROTp1Kg7FTE";
        public const string Partnerregistertoken = "5588139078:AAG9wveDauS67SGHYDtX_UrO9giAqAxw4z0";
        public const string Admintoken = "5148883933:AAGhrEGQ7DhrY8sW_-8b2SGmII8vLEjA1oc";
        public const string adminchatid = "1762884854";
        public const string SendMessageMehtod = "sendMessage";

        public static async Task<T> SendMessagePostAsync<T>(object requestObject, string methodName, string destination = Clientbottoken)
        {

            using (var client = new HttpClient())
            {
                var result = await client.PostAsync(
                    $"https://api.telegram.org/bot{destination}/{methodName}",
                     new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json"));
                string jsonContent = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(jsonContent);
            }
        }

        public static async Task<freeimageresponse> GeneratePhotoLinkoutside(string telegramfileid, string destination = Clientbottoken)
        {
            TelegramFileResult photofile = await WebClient.SendMessagePostAsync<TelegramFileResult>((new { file_id = telegramfileid }), "getFile", WebClient.Clientbottoken);
            string telegramphotopath = $"https://api.telegram.org/file/bot{WebClient.Clientbottoken}/{photofile.result.file_path}";

          
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(
                    $"https://freeimage.host/api/1/upload?key=6d207e02198a847aa98d0a2a901485a5&source={telegramphotopath}");
                string jsonContent = result.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<freeimageresponse>(jsonContent);
            }
        }

      

    }
}
