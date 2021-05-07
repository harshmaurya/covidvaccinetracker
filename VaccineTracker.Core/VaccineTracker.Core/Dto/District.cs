using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VaccineTracker.Core.Dto
{
    public class District : IEquatable<District>
    {
        [JsonPropertyName("district_id")]
        public int DistrictId { get; set; }


        [JsonPropertyName("district_name")]
        public string DistrictName { get; set; }

        public static District Any()
        {
            return new District
            {
                DistrictId = -1,
                DistrictName = "Any"
            };
        }

        public bool Equals(District other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DistrictId == other.DistrictId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((District) obj);
        }

        public override int GetHashCode()
        {
            return DistrictId;
        }
    }

    public class DistrictResponse
    {
        public List<District> Districts { get; set; }
    }
}