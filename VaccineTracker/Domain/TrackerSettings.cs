using System;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Domain
{
    public class TrackerSettings
    {
        public string Name { get; set; }

        public State State { get; set; }

        public District District { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MinimumSlots { get; set; }

        public string AgeGroup { get; set; }

        public bool SoundAlarm { get; set; }

        public string Email { get; set; }
    }
}