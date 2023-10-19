using Veto.Application.Exceptions;
using Veto.Domain.Entities;
using Veto.Domain.Providers;

namespace Veto.Application.Appointments.UseCases
{
    public class RescheduleAppointmentUseCaseValidator
    {
        public void Validate(RescheduleAppointmentUseCaseRequest request, IDateProvider dateProvider)
        {
            ValidateClientId(request.ClientId);
            ValidateAppointmentId(request.AppointmentId);
            ValidateAppointmentDate(request.NewAppointmentDate, dateProvider);
        }

        private void ValidateClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new UseCaseValidationException($"{nameof(clientId)} is invalid");
            }
        }

        private void ValidateAppointmentId(string appointmentId)
        {
            if (string.IsNullOrEmpty(appointmentId))
            {
                throw new UseCaseValidationException($"{nameof(appointmentId)} is invalid");
            }
        }

        private void ValidateAppointmentDate(DateTime newAppointmentDate, IDateProvider dateProvider)
        {
            if (newAppointmentDate < dateProvider.Now())
            {
                throw new UseCaseValidationException($"{nameof(newAppointmentDate)} is invalid");
            }
        }
    }
}
