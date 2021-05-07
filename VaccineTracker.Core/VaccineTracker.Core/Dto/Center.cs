using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VaccineTracker.Core.Dto
{
    public class Center
    {
        [JsonPropertyName("center_id")]
        public int CenterId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [JsonPropertyName("state_name")]
        public string StateName { get; set; }


        [JsonPropertyName("district_name")]
        public string DistrictName { get; set; }


        [JsonPropertyName("block_name")]
        public string BlockName { get; set; }

        public int Pincode { get; set; }


        [JsonPropertyName("fee_type")]
        public string FeeType { get; set; }

        public List<Session> Sessions { get; set; }
    }
}