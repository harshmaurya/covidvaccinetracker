using System;
using VaccineTracker.Core.Domain;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Core
{
    public interface IAlertHandler
    {
        void HandleVaccineAvailability(VaccineAvailability vaccineAvailability);

        void HandlerError(Exception exception);
    }

}