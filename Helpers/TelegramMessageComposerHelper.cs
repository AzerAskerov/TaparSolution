using AWSServerless2.Models;

namespace AWSServerless2.Helpers
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
    }
}
