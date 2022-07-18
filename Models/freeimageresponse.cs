namespace TaparSolution.Models
{
    public class freeimageresponse
    {
        public int status_code { get; set; }
        public Success success { get; set; }
        public Image image { get; set; }
        public string status_txt { get; set; }



        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Chain
        {
            public string image { get; set; }
            public string thumb { get; set; }
            public string medium { get; set; }
        }

        public class ChainCode
        {
            public string image { get; set; }
            public string thumb { get; set; }
            public string medium { get; set; }
        }

        public class File
        {
            public Resource resource { get; set; }
        }

        public class Image
        {
            public string name { get; set; }
            public string extension { get; set; }
            public string width { get; set; }
            public string height { get; set; }
            public int size { get; set; }
            public string time { get; set; }
            public string expiration { get; set; }
            public string adult { get; set; }
            public string status { get; set; }
            public string cloud { get; set; }
            public string vision { get; set; }
            public string likes { get; set; }
            public object description { get; set; }
            public object original_exifdata { get; set; }
            public string original_filename { get; set; }
            public string views_html { get; set; }
            public string views_hotlink { get; set; }
            public string access_html { get; set; }
            public string access_hotlink { get; set; }
            public File file { get; set; }
            public int is_animated { get; set; }
            public int nsfw { get; set; }
            public string id_encoded { get; set; }
            public double ratio { get; set; }
            public string size_formatted { get; set; }
            public string filename { get; set; }
            public string url { get; set; }
            public string url_short { get; set; }
            public string url_seo { get; set; }
            public string url_viewer { get; set; }
            public string url_viewer_preview { get; set; }
            public string url_viewer_thumb { get; set; }
            public Image image { get; set; }
            public Thumb thumb { get; set; }
            public Medium medium { get; set; }
            public string display_url { get; set; }
            public string display_width { get; set; }
            public string display_height { get; set; }
            public string views_label { get; set; }
            public string likes_label { get; set; }
            public string how_long_ago { get; set; }
            public string date_fixed_peer { get; set; }
            public string title { get; set; }
            public string title_truncated { get; set; }
            public string title_truncated_html { get; set; }
            public bool is_use_loader { get; set; }
            public string mime { get; set; }
        }

        public class Medium
        {
            public string filename { get; set; }
            public string name { get; set; }
            public string mime { get; set; }
            public string extension { get; set; }
            public string url { get; set; }
        }

        public class Resource
        {
            public Chain chain { get; set; }
            public ChainCode chain_code { get; set; }
        }

         public class Success
        {
            public string message { get; set; }
            public int code { get; set; }
        }

        public class Thumb
        {
            public string filename { get; set; }
            public string name { get; set; }
            public string mime { get; set; }
            public string extension { get; set; }
            public string url { get; set; }
        }
    }
}
