using Veto.Application.Exceptions;

namespace Veto.Application.Appointments.UseCases
{
    public class CancelAppointmentUseCaseValidator
    {
        public void Validate(CancelAppointmentUseCaseRequest request)
        {
            ValidateClientId(request.ClientId);
            ValidateAppointmentId(request.AppointmentId);
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
    }
}
