using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Domain.Entities;

namespace Veto.Application.Appointments.UseCases
{
    public class CancelAppointmentUseCase
    {
        private readonly IApplicationStore _applicationStore;

        public CancelAppointmentUseCase(IApplicationStore applicationStore)
        {
            _applicationStore = applicationStore;
        }

        public void Execute(CancelAppointmentUseCaseRequest request)
        {
            var validator = new CancelAppointmentUseCaseValidator();
            validator.Validate(request);

            var client = _applicationStore.Clients.FirstOrDefault(c => c.Id == request.ClientId);
            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            client.CancelAppointment(request.AppointmentId);

            _applicationStore.Save(client);
        }
    }
}
