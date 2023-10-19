using Veto.Application.Exceptions;

namespace Veto.Application.Patients.UseCases
{
    internal class CreatePatientUseCaseValidator
    {
        public void Validate(CreatePatientUseCaseRequest request)
        {
            ValidateClientId(request.ClientId);
            ValidateName(request.PatientName);
        }

        private void ValidateClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new UseCaseValidationException($"{nameof(clientId)} is invalid");
            }
        }

        private void ValidateName(string patientName)
        {
            if(string.IsNullOrEmpty(patientName))
            {
                throw new UseCaseValidationException($"{nameof(patientName)} is invalid");
            }
        }
    }
}
