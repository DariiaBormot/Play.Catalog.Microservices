namespace Play.Common.Settings
{
    public class MongoBdSettings
    {
        public string Host { get; init; }
        public int Port { get; init; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
