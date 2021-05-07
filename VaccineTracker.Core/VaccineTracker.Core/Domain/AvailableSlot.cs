namespace VaccineTracker.Core.Domain
{
    public class AvailableSlot
    {
        public string Date { get; set; }

        public string CenterName { get; set; }

        public int PinCode { get; set; }

        public string Address { get; set; }

        public int Available { get; set; }

        public string VaccineType { get; set; }
    }
}