using TaparSolution.Models;

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
    }
}
