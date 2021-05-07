using System.Collections.Generic;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Core.Domain
{
    public class VaccineAvailability
    {
        public List<AvailableSlot> AvailableSlots { get; set; }

    }
}