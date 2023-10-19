using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Appointments.UseCases;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Application.Tests.Mocks;
using Veto.Domain.Entities;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Application.Tests.Appointments
{
    internal class RescheduleAppointmentUseCaseTests
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
        public void RescheduleAppointmentUseCase_Should_Succeed()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));
            var newAppointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("12:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate));
            Assert.Pass();
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_NotFound_Client()
        {
            var newAppointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("12:00"));

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<ClientNotFoundException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate)));
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_NotFound_Appointment()
        {
            var newAppointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("12:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentNotFoundException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate)));
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_Invalid_Date()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));
            var newAppointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("16:28"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsInvalidException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate)));
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_TooEarly_Date()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));
            var newAppointmentDate = _dateProvider.Now().AddDays(1).Date.Add(TimeSpan.Parse("07:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsInvalidException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate)));
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_TooLately_Date()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));
            var newAppointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("21:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsInvalidException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate)));
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_Past_Date()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));
            var newAppointmentDate = _dateProvider.Now().AddDays(-1).Date.Add(TimeSpan.Parse("14:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<UseCaseValidationException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", newAppointmentDate)), "date is invalid");
        }

        [Test]
        public void RescheduleAppointmentUseCase_Should_Fail_With_Existing_Appointment()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));
            var appointmentDate2 = _dateProvider.Now().Date.Add(TimeSpan.Parse("16:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            client.TryBookAppointment("2", Appointment.New("4", appointmentDate2), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsAlreadyBookedException>(() => useCase.Execute(new RescheduleAppointmentUseCaseRequest("1", "3", appointmentDate2)));
        }
    }
}
