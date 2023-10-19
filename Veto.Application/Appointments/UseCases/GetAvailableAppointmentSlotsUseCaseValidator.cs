using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Application.Appointments.UseCases
{
    public class GetAvailableAppointmentSlotsUseCaseValidator
    {
        public void Validate(GetAvailableAppointmentSlotsUseCaseRequest request, IDateProvider dateProvider)
        {
            ValidateClientId(request.ClientId);
            ValidateRequestDay(request.RequestDay, dateProvider);
        }

        private void ValidateClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new UseCaseValidationException($"{nameof(clientId)} is invalid");
            }
        }

        private void ValidateRequestDay(DateTime requestDay, IDateProvider dateProvider)
        {
            if (requestDay.Date < dateProvider.Now().Date)
            {
                throw new UseCaseValidationException($"{nameof(requestDay)} is invalid");
            }
        }
    }
}
