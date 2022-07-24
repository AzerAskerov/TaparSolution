using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaparSolution.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Chat
    {
        public long id { get; set; }
        public string first_name { get; set; }
     
        public string type { get; set; }
    }

    public class Entity
    {
        public long offset { get; set; }
        public long length { get; set; }
        public string type { get; set; }
    }

    public class From
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }

        public string language_code { get; set; }
    }

    public class Message
    {
        public long message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public Message reply_to_message { get; set; }
        public long date { get; set; }
        public string text { get; set; }
        public List<Entity> entities { get; set; }
        public List<Photo> photo { get; set; }
        public Video video { get; set; }
        public TelegramLocation location { get; set; }
        public TelegramContact contact { get; set; }
        public string caption { get; set; }

        public class Video
        {
            public string file_id { get; set; }
        }

    }

  public  class TelegramContact
    {
        public string phone_number { get; set; }
    }
    public class Photo
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int file_size { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    public class TelegramMessage
    {
        public long update_id { get; set; }
        public Message message { get; set; }
        public Inline_query inline_query { get; set; }
        public Callback_query callback_query { get; set; }
        public originEnum Origin { get; set; }

    }

    public class Callback_query
    {
        public Message message { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public string  data { get; set; }
       
    }

    public class Inline_query
    {
        public string id { get; set; }
        public string chat_type { get; set; }
        public string query { get; set; }
    }



}
