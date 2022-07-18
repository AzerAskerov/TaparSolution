namespace AWSServerless2
{
    public interface IConfig
    {
        public string awsacessId { get; set; }
        public string awsSecretAccessKey { get; set; }
        public string AwsRegion { get; set; }

    }

    public class MyConfig : IConfig
    {
        public string awsacessId { get; set; }
        public string awsSecretAccessKey { get; set; }
        public string AwsRegion { get; set; }
    }
}
