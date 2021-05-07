namespace VaccineTracker.Core
{
    public class EmailSettings
    {
        public string SmtpAddress { get; set; }

        public int PortNumber { get; set; }

        public string EmailFrom { get; set; }

        public string EmailTo { get; set; }

        public string Password { get; set; }
    }
}