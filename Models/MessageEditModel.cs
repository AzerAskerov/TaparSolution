namespace TaparSolution.Models
{
    public class MessageEditModel
    {
        public string chat_id { get; set; }
        public long message_id { get; set; }
        public dynamic reply_markup { get; set; }
    }
}
