using AWSServerless2.Models;

namespace AWSServerless2
{
    public class Receiver
    {
        public TelegramMessage input  { get; set; }
        public  string Cordinator(TelegramMessage incomingmessage)
        {
            input = incomingmessage;
            return "ok";
        }
    }
}
