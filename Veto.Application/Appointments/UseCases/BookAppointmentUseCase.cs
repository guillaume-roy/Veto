using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Domain.Entities;
using Veto.Domain.Providers;

namespace Veto.Application.Appointments.UseCases
{
    public class BookAppointmentUseCase
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IEntityIdGenerator _entityIdGenerator;
        private readonly IDateProvider _dateProvider;
        private readonly IAppointmentSlotConstraintsProvider _appointmentSlotConstraintsProvider;

        public BookAppointmentUseCase(IApplicationStore applicationStore, IEntityIdGenerator entityIdGenerator, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            _applicationStore = applicationStore;
            _entityIdGenerator = entityIdGenerator;
            _dateProvider = dateProvider;
            _appointmentSlotConstraintsProvider = appointmentSlotConstraintsProvider;
        }

        public string Execute(BookAppointmentUseCaseRequest request)
        {
            var validator = new BookAppointmentUseCaseValidator();
            validator.Validate(request, _dateProvider);

            var client = _applicationStore.Clients.FirstOrDefault(c => c.Id == request.ClientId);
            if (client == null)
            {
                throw new ClientNotFoundException();
            }

            var appointment = Appointment.New(_entityIdGenerator.Generate(), request.AppointmentDate);

            client.TryBookAppointment(request.PatientId, appointment, _dateProvider, _appointmentSlotConstraintsProvider);

            _applicationStore.Save(client);

            return appointment.Id;
        }
    }
}
