namespace AWSServerless2.Helpers
{
    public static class DateTImeHelper
    {
        public static DateTime GetCurrentDate()
        {
            return DateTime.Now.AddHours(0);
        }
    }
}
