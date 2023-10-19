using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Domain.Entities;

namespace Veto.Application.Patients.UseCases
{
    public class CreatePatientUseCase
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IEntityIdGenerator _entityIdGenerator;

        public CreatePatientUseCase(IApplicationStore _applicationStore, IEntityIdGenerator entityIdGenerator)
        {
            this._applicationStore = _applicationStore;
            _entityIdGenerator = entityIdGenerator;
        }

        public string Execute(CreatePatientUseCaseRequest request)
        {
            var validator = new CreatePatientUseCaseValidator();
            validator.Validate(request);

            var client = _applicationStore.Clients.FirstOrDefault(c => c.Id == request.ClientId);
            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            var patient = Patient.New(
                _entityIdGenerator.Generate(),
                request.PatientName);

            client.AddPatient(patient);

            _applicationStore.Save(client);

            return patient.Id;
        }
    }
}
