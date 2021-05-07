using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Core
{
    public class VaccineApi
    {
        private static ConcurrentDictionary<int, List<District>> _stateToDistricts = new ConcurrentDictionary<int, List<District>>();

        private static List<State> _states = new List<State>();

        public VaccineApi()
        {
            //LoadCache();
        }

        private static void LoadCache()
        {
            try
            {
                var states = File.ReadAllText("StatesCollection.json");
                _states = JsonSerializer.Deserialize<StateCollection>(states).States;

                var districtsMap = File.ReadAllText("DistrictMap.json");
                var stateToDistricts = JsonSerializer.Deserialize<DistrictsMap>(districtsMap).StateToDistricts;
                foreach (var key in stateToDistricts.Keys)
                {
                    _stateToDistricts.TryAdd(key, stateToDistricts[key]);
                }
            }
            catch (Exception e)
            {
                //
            }
        }

        public async Task<List<State>> GetStates()
        {
            try
            {
                if (_states.Any()) return _states;
                var client = CreateClient();
                var response = await client.GetFromJsonAsync<StateResponse>("admin/location/states");
                _states = response.States;
                return _states;
            }
            catch (Exception e)
            {
                //log
                throw;
            }
        }

        public async Task<List<District>> GetDistricts(int stateId)
        {
            try
            {
                if (!_stateToDistricts.TryGetValue(stateId, out var districts))
                {
                    var client = CreateClient();
                    var response = await client.GetFromJsonAsync<DistrictResponse>($"admin/location/districts/{stateId}");
                    _stateToDistricts.TryAdd(stateId, response.Districts);

                    return response.Districts;
                }

                return districts;
            }
            catch (Exception e)
            {
                //log
                throw;
            }
        }

        public async Task<Availability> GetSevenDaysAvailability(DateTime start, int districtId)
        {
            try
            {
                var client = CreateClient();
                var dateStr = start.ToString("dd-MM-yyyy");
                var response = await client.GetFromJsonAsync<Availability>(
                    $"appointment/sessions/public/calendarByDistrict?district_id={districtId}&date={dateStr}");
                return response;
            }
            catch (JsonException ex)
            {
                //ignored
                return new Availability
                {
                    Centers = new List<Center>(),

                };
            }
            catch (Exception e)
            {
                //log
                throw;
            }
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://cdn-api.co-vin.in/api/v2/")
            };
            var agent = Environment.GetEnvironmentVariable("Agent");
            if (!string.IsNullOrEmpty(agent))
            {
                client.DefaultRequestHeaders.Add("User-Agent", agent);
            }

            return client;
        }
    }

    public class StateCollection
    {
        public List<State> States { get; set; }
    }

    public class DistrictsMap
    {
        public Dictionary<int, List<District>> StateToDistricts { get; set; }
    }
}