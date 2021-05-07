using System;
using VaccineTracker.Core;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SetupAlarm();
            System.Console.ReadLine();
        }

        private static void SetupAlarm()
        {
            var alert = new VaccineAlert(new Criteria
            {
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(20),
                State = "Delhi",
                District = District.Any(),
                MinimumAvailability = 2,
                AgeGroup = AgeGroup.Over18

            }, new ConsoleHandler(), new SoundHandler());
            alert.Start().Wait();
        }
    }
}
