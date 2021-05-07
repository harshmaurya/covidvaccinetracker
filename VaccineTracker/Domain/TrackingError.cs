using System;

namespace VaccineTracker.Domain
{
    public class TrackingError
    {
        public DateTime Time { get; set; }

        public string Message { get; set; }
    }
}
