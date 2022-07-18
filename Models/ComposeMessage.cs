using Newtonsoft.Json;

namespace AWSServerless2.Models
{
    public class ComposeMessage
    {
        public string text { get; set; }
        public string caption { get; set; }
        public string photo { get; set; }
        [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediaPhoto> media { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public string reply_to_message_id { get; set; }
        public string chat_id { get; set; }
        [JsonProperty("reply_markup", NullValueHandling = NullValueHandling.Ignore)]
        public dynamic reply_markup { get; set; }
        public string parse_mode { get { return "markdown"; } }
        public bool request_location { get; set; }
        public bool request_contact { get; set; }

    }

    public class MediaPhoto
    {
        public string type { get; set; }
        public string media { get; set; }
        public string caption { get; set; }

    }

    public class Keyboard
    {
        public List<List<Inline_keyboard>> keyboard { get; set; }
        public bool resize_keyboard { get { return true; } }
        public bool one_time_keyboard { get; set; }
    }

 

    public class Inline_Keyboard
    {
        public List<List<Inline_keyboard>> inline_keyboard { get; set; }
    }

    public class Inline_keyboard
    {
        public string text { get; set; }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string  url { get; set; }
        [JsonProperty("switch_inline_query_current_chat", NullValueHandling = NullValueHandling.Ignore)]
        public string switch_inline_query_current_chat { get; set; }
        public string callback_data { get; set; }
        public bool request_contact { get; set; }
        public bool request_location { get; set; }
        public Inline_keyboard()
        {
            callback_data = "";
        }
    }

}
