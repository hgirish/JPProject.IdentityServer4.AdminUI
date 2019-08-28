namespace Jp.Infra.CrossCutting.Identity.Services
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public bool SendEmail { get; set; }
        public bool UseSsl { get; set; }
        public int SmtpDeliveryMethod { get; set; } = 0; // Network
        public string PickupDirectory { get; set; } = string.Empty; // Required if DeliveryMethod is 1 => SpecifiedPickupDirectory  
    }
}
