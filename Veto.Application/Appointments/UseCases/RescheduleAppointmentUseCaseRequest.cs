using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Application.Appointments.UseCases
{
    public record RescheduleAppointmentUseCaseRequest(string ClientId, string AppointmentId, DateTime NewAppointmentDate);
}
