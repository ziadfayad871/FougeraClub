namespace FougeraClub.Web.Otp
{
    public class ManagerOtpOptions
    {
        public bool UseFixedCode { get; set; } = true;
        public string FixedCode { get; set; } = "1111";
        public bool EnableDelivery { get; set; } = false;
        public int CodeLength { get; set; } = 4;
        public int ExpiryMinutes { get; set; } = 5;
        public string ManagerName { get; set; } = "ziad";
        public string Recipient { get; set; } = string.Empty;
    }
}
