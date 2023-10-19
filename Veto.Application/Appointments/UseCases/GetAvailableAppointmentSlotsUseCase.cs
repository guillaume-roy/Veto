using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Domain.Providers;

namespace Veto.Application.Appointments.UseCases
{
    public class GetAvailableAppointmentSlotsUseCase
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IDateProvider _dateProvider;
        private readonly IAppointmentSlotConstraintsProvider _appointmentSlotConstraintsProvider;

        public GetAvailableAppointmentSlotsUseCase(IApplicationStore applicationStore, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            _applicationStore = applicationStore;
            _dateProvider = dateProvider;
            _appointmentSlotConstraintsProvider = appointmentSlotConstraintsProvider;
        }

        public List<DateTime> Execute(GetAvailableAppointmentSlotsUseCaseRequest request)
        {
            var validator = new GetAvailableAppointmentSlotsUseCaseValidator();
            validator.Validate(request, _dateProvider);

            var client = _applicationStore.Clients.FirstOrDefault(c => c.Id == request.ClientId);
            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            return client.GetAvailableAppointmentSlots(request.RequestDay, _dateProvider, _appointmentSlotConstraintsProvider);
        }
    }
}
