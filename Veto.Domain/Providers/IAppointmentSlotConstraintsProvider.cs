using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Domain.Providers
{
    public interface IAppointmentSlotConstraintsProvider
    {
        TimeSpan GetOpeningHour();
        TimeSpan GetClosingHour();
        TimeSpan GetSlotDuration();
    }
}
