namespace Post.Cmd.Infrastructure.Config
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
    }
}