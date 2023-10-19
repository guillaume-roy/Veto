using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Appointments.UseCases;
using Veto.Application.Persistence;
using Veto.Application.Tests.Mocks;
using Veto.Domain.Entities;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Application.Tests.Appointments
{
    [TestFixture]
    internal class GetAvailableAppointmentSlotsUseCaseTests
    {
        private IApplicationStore _applicationStore;
        private AppointmentSlotConstraintsProviderMock _slotConstraintsProvider;
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
        public void GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_One_Appointment()
        {
            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("10:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new GetAvailableAppointmentSlotsUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            var slots = useCase.Execute(new GetAvailableAppointmentSlotsUseCaseRequest("1", appointmentDate.Date));
            Assert.IsTrue(!slots.Any(s => s == appointmentDate));
        }

        [Test]
        public void GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_One_Slot()
        {
            _dateProvider.MockValue = DateTime.Now.Date.Add(TimeSpan.Parse("16:44"));

            var appointmentDate = _dateProvider.Now().Date.Add(TimeSpan.Parse("17:00"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            client.AddPatient(Patient.New("2", "Sarah Connor"));
            client.TryBookAppointment("2", Appointment.New("3", appointmentDate), _dateProvider, _slotConstraintsProvider);
            _applicationStore.Save(client);

            var useCase = new GetAvailableAppointmentSlotsUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            var slots = useCase.Execute(new GetAvailableAppointmentSlotsUseCaseRequest("1", appointmentDate.Date));
            Assert.IsTrue(slots.Count == 1);
        }

        [Test]
        public void GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_Empty_Slots_TooLately()
        {
            _dateProvider.MockValue = DateTime.Now.Date.Add(TimeSpan.Parse("18:12"));

            var client = Client.New("1", "test@test.com", "123456789Test!");
            _applicationStore.Save(client);

            var useCase = new GetAvailableAppointmentSlotsUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            var slots = useCase.Execute(new GetAvailableAppointmentSlotsUseCaseRequest("1", _dateProvider.Now().Date));
            Assert.IsTrue(slots.Count == 0);
        }

        [TestCaseSource(typeof(GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_Specific_Empty_Slots_Data), nameof(GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_Specific_Empty_Slots_Data.TestCases))]
        public int GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_Specific_Empty_Slots(TimeSpan slotDuration)
        {
            _slotConstraintsProvider.SlotDurationMock = slotDuration;

            var client = Client.New("1", "test@test.com", "123456789Test!");
            _applicationStore.Save(client);

            var useCase = new GetAvailableAppointmentSlotsUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
            var slots = useCase.Execute(new GetAvailableAppointmentSlotsUseCaseRequest("1", _dateProvider.Now().Date));
            return slots.Count;
        }
    }

    internal class GetAvailableAppointmentSlotsUseCase_Should_Succeed_With_Specific_Empty_Slots_Data
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(TimeSpan.Parse("01:00")).Returns(9);
                yield return new TestCaseData(TimeSpan.Parse("00:30")).Returns(18);
                yield return new TestCaseData(TimeSpan.Parse("00:15")).Returns(36);
                yield return new TestCaseData(TimeSpan.Parse("00:23")).Returns(23);
            }
        }
    }
}
