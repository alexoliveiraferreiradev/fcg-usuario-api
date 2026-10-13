using System.ComponentModel.DataAnnotations;

namespace Fcg.Users.Infrastructure.MessageBroker
{
    internal class AzureServiceBusSettings
    {
        public const string SectionName = "AzureServiceBus";
        [Required]
        public string ConnectionString { get; set; } = string.Empty;    
    }
}
