using Veto.Application.Exceptions;
using Veto.Domain.Entities;
using Veto.Domain.Providers;

namespace Veto.Application.Appointments.UseCases
{
    public class BookAppointmentUseCaseValidator
    {
        public void Validate(BookAppointmentUseCaseRequest request, IDateProvider dateProvider)
        {
            ValidateClientId(request.ClientId);
            ValidatePatientId(request.PatientId);
            ValidateAppointmentDate(request.AppointmentDate, dateProvider);
        }

        private void ValidateClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new UseCaseValidationException($"{nameof(clientId)} is invalid");
            }
        }

        private void ValidatePatientId(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                throw new UseCaseValidationException($"{nameof(patientId)} is invalid");
            }
        }

        private void ValidateAppointmentDate(DateTime appointmentDate, IDateProvider dateProvider)
        {
            if (appointmentDate < dateProvider.Now())
            {
                throw new UseCaseValidationException($"{nameof(appointmentDate)} is invalid");
            }
        }
    }
}
