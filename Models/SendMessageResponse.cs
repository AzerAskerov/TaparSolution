namespace TaparSolution.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);


    public class Result
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
    }

    public class SendMessageResponse
    {
        public bool ok { get; set; }
        public Result result { get; set; }
        public string description { get; set; }
    }


}
