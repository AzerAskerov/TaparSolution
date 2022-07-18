using TaparSolution.Models;

namespace TaparSolution
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
