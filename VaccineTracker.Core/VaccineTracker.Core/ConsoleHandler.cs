using System;
using VaccineTracker.Core.Domain;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Core
{
    public class ConsoleHandler : IAlertHandler
    {
        public void HandleVaccineAvailability(VaccineAvailability vaccineAvailability)
        {
            if (vaccineAvailability.AvailableSlots.Count == 0)
            {
                Console.WriteLine($"As of {DateTime.Now}, no session is available with selected criteria");
            }
            else
            {
                foreach (var slot in vaccineAvailability.AvailableSlots)
                {
                    Console.WriteLine($"As of {DateTime.Now}, session available at pincode {slot.PinCode}, {slot.CenterName} on {slot.Date}");
                }
            }
        }

        public void HandlerError(Exception exception)
        {
            Console.WriteLine($"An error occured. {exception.Message}");
        }
    }
}