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
        public TimeSpan GetClosingHour()
        {
            return TimeSpan.Parse("17:30");
        }

        public TimeSpan GetOpeningHour()
        {
            return TimeSpan.Parse("09:00");
        }

        public TimeSpan GetSlotDuration()
        {
            return TimeSpan.Parse("00:30");
        }
    }
}
