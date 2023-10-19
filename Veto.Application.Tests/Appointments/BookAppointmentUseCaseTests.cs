using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Appointments.UseCases;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Patients.UseCases;
using Veto.Application.Persistence;
using Veto.Application.Tests.Mocks;
using Veto.Domain.Entities;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Application.Tests.Appointments
{
    internal class BookAppointmentUseCaseTests
    {
        private IApplicationStore _applicationStore;
        private IEntityIdGenerator _entityIdGenerator;
        private IAppointmentSlotConstraintsProvider _slotConstraintsProvider;
        private DateProviderMock _dateProvider;

        [SetUp]
        public void Setup()
        {
            _applicationStore = new ApplicationStoreMock();
            _entityIdGenerator = new EntityIdGeneratorMock();
            _slotConstraintsProvider = new AppointmentSlotConstraintsProviderMock();
            _dateProvider = new DateProviderMock();
            _dateProvider.MockValue = DateTime.Now.Date.Add(TimeSpan.Parse("06:15"));
        }

        [Test]
        public void BookAppointmentUseCase_Should_Succeed()
        {
            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("10:00"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            var appointmentId = useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate));
            Assert.IsNotNull(appointmentId);
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_NotFound_Client()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("10:00"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<ClientNotFoundException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)));
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_NotFound_Patient()
        {
            var client = Client.New("1", "test@test.com", "123456789Test!");
            _applicationStore.Save(client);

            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("10:00"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<PatientNotFoundException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)));
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_Invalid_Date()
        {
            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:33"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsInvalidException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)));
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_TooEarly_Date()
        {
            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("07:00"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsInvalidException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)));
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_TooLately_Date()
        {
            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("19:00"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsInvalidException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)));
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_Past_Date()
        {
            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            _applicationStore.Save(client);

            var appointmentDate = _dateProvider.Now().AddDays(-1).Date.Add(TimeSpan.Parse("15:00"));

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<UseCaseValidationException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)), "date is invalid");
        }

        [Test]
        public void BookAppointmentUseCase_Should_Fail_With_Existing_Appointment()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("15:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
            Assert.Throws<AppointmentDateIsAlreadyBookedException>(() => useCase.Execute(new BookAppointmentUseCaseRequest("1", "2", appointmentDate)));
        }
    }
}
