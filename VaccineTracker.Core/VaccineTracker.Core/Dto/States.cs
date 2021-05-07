using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VaccineTracker.Core.Dto
{
    public class State
    {
        [JsonPropertyName("state_id")]
        public int StateId { get; set; }

        [JsonPropertyName("state_name")]
        public string StateName { get; set; }
    }

    public class StateResponse
    {
        public List<State> States { get; set; }
    }
}
