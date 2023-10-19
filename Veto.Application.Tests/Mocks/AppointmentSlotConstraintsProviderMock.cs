using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Domain.Providers;

namespace Veto.Application.Tests.Mocks
{
    internal class AppointmentSlotConstraintsProviderMock : IAppointmentSlotConstraintsProvider
    {
        public TimeSpan ClosingHourMock { get; set; } = TimeSpan.Parse("18:00");
        public TimeSpan OpeningHourMock { get; set; } = TimeSpan.Parse("09:00");
        public TimeSpan SlotDurationMock { get; set; } = TimeSpan.Parse("00:30");

        public TimeSpan GetClosingHour()
        {
            return ClosingHourMock;
        }

        public TimeSpan GetOpeningHour()
        {
            return OpeningHourMock;
        }

        public TimeSpan GetSlotDuration()
        {
            return SlotDurationMock;
        }
    }
}
