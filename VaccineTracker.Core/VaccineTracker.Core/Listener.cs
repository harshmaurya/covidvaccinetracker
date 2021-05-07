using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VaccineTracker.Core.Domain;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Core
{
    public class Listener
    {
        private readonly VaccineApi _api = new VaccineApi();
        private const int PollMinutes = 5;

        public async Task<IObservable<VaccineAvailability>> SubscribeAvailableSessions(Criteria criteria)
        {
            try
            {
                var states = await _api.GetStates();
                var state = states.First(st => criteria.State.Equals(st.StateName));
                var districts = await _api.GetDistricts(state.StateId);

                return Observable.Interval(TimeSpan.FromMinutes(PollMinutes))
                    .StartWith(-1)
                    .Select(l =>
                    {
                        var availabilityList = new List<AvailableSlot>();
                        foreach (var district in districts)
                        {
                            var districtMatch = criteria.District.Equals(District.Any()) || criteria.District.Equals(district);
                            if (!districtMatch) continue;
                            var startDate = DateTime.Today;
                            while (startDate <= criteria.EndDate)
                            {
                                FindAndAddSessions(criteria, district,
                                    availabilityList, startDate);
                                startDate = startDate.AddDays(7);
                            }

                        }

                        if (availabilityList.Any())
                        {
                            return new VaccineAvailability
                            {
                                AvailableSlots = availabilityList
                            };
                        }

                        return new VaccineAvailability
                        {
                            AvailableSlots = new List<AvailableSlot>()
                        };
                    });
            }
            catch (Exception e)
            {
                return Observable.Throw<VaccineAvailability>(e);
            }
        }

        private void FindAndAddSessions(Criteria criteria, District district, List<AvailableSlot> availabilityList, DateTime startDate)
        {
            var availability = _api.GetSevenDaysAvailability(startDate, district.DistrictId).Result;
            foreach (var center in availability.Centers)
            {
                foreach (var session in center.Sessions)
                {
                    var minAge = criteria.AgeGroup == AgeGroup.Over18 ? 19 : 46;

                    if (session.AvailableCapacity >= criteria.MinimumAvailability
                        && session.MinAgeLimit < minAge
                        && session.AvailableDate >= criteria.StartDate
                        && session.AvailableDate <= criteria.EndDate)
                    {
                        availabilityList.Add(
                            new AvailableSlot
                            {
                                Date = session.DateString,
                                Address = center.Address,
                                CenterName = center.Name,
                                Available = session.AvailableCapacity,
                                PinCode = center.Pincode,
                                VaccineType = session.Vaccine
                            });
                    }
                }
            }
        }
    }

    public class Criteria
    {
        public string State { get; set; }

        public District District { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MinimumAvailability { get; set; }

        public AgeGroup AgeGroup { get; set; }
    }


    public enum AgeGroup
    {
        Over18,
        Over45
    }
}