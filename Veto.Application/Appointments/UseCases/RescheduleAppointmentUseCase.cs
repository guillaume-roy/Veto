using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Domain.Entities;
using Veto.Domain.Providers;

namespace Veto.Application.Appointments.UseCases
{
    public class RescheduleAppointmentUseCase
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IDateProvider _dateProvider;
        private readonly IAppointmentSlotConstraintsProvider _appointmentSlotConstraintsProvider;

        public RescheduleAppointmentUseCase(IApplicationStore applicationStore, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            _applicationStore = applicationStore;
            _dateProvider = dateProvider;
            _appointmentSlotConstraintsProvider = appointmentSlotConstraintsProvider;
        }

        public void Execute(RescheduleAppointmentUseCaseRequest request)
        {
            var validator = new RescheduleAppointmentUseCaseValidator();
            validator.Validate(request, _dateProvider);

            var client = _applicationStore.Clients.FirstOrDefault(c => c.Id == request.ClientId);
            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            client.TryRescheduleAppointment(request.AppointmentId, request.NewAppointmentDate, _dateProvider, _appointmentSlotConstraintsProvider);

            _applicationStore.Save(client);
        }
    }
}
