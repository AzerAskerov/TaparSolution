namespace AWSServerless2.Helpers
{
    public static class UniqueGeneratorHelper
    {
        public static long UUDGenerate()
        {
            return DateTImeHelper.GetCurrentDate().Ticks;
        }
    }
}
