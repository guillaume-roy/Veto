using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Appointments.UseCases;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Application.Tests.Mocks;
using Veto.Domain.Entities;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Application.Tests.Appointments
{
    internal class CancelAppointmentUseCaseTests
    {
        private IApplicationStore _applicationStore;
        private IAppointmentSlotConstraintsProvider _slotConstraintsProvider;
        private DateProviderMock _dateProvider;

        [SetUp]
        public void Setup()
        {
            _applicationStore = new ApplicationStoreMock();
            _slotConstraintsProvider = new AppointmentSlotConstraintsProviderMock();
            _dateProvider = new DateProviderMock();
            _dateProvider.MockValue = DateTime.Now.Date.Add(TimeSpan.Parse("06:15"));
        }

        [Test]
        public void CancelAppointmentUseCase_Should_Succeed()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("10:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new CancelAppointmentUseCase(_applicationStore);
            useCase.Execute(new CancelAppointmentUseCaseRequest("1", "3"));
            Assert.Pass();
        }

        [Test]
        public void CancelAppointmentUseCase_Should_Fail_With_NotFound_Appointment()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("10:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var useCase = new CancelAppointmentUseCase(_applicationStore);
            Assert.Throws<AppointmentNotFoundException>(() => useCase.Execute(new CancelAppointmentUseCaseRequest("1", "3")));
        }
    }
}
