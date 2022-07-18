namespace TaparSolution.Models
{
    public class AnswerInlineQuery
    {
        public string inline_query_id { get; set; }
        public List<InlineArticleAnswer> results { get; set; }

    }
   public class InlineArticleAnswer
    {
        public InlineArticleAnswer()
        {
            thumb_url = "";
        }
        public string type { get { return "article"; } }

        public string id { get; set; }
        public string title { get; set; }
        public string thumb_url { get; set; }
        public Input_message_content input_message_content { get; set; }

    }

  public  class Input_message_content
    {
        public string message_text { get; set; }
    }
}
