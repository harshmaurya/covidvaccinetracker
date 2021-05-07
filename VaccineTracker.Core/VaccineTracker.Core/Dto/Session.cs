using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace VaccineTracker.Core.Dto
{
    public class Session
    {
        [JsonPropertyName("date")]
        public string DateString { get; set; }

        public DateTime AvailableDate => DateTime.ParseExact(DateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        [JsonPropertyName("available_capacity")]
        public int AvailableCapacity { get; set; }


        [JsonPropertyName("min_age_limit")]
        public int MinAgeLimit { get; set; }

        public string Vaccine { get; set; }
    }
}