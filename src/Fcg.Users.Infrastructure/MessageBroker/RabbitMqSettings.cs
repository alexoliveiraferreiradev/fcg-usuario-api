namespace Fcg.Users.Infrastructure.MessageBroker
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = string.Empty;
        public ushort Port { get; set; } = 5672;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public const string SectionName = "RabbitMqSettings";
        public string ConnectionString => $"amqp://{Username}:{Password}@{Host}:{Port}//";
    }
}
