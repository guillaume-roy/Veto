using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Application.Appointments.UseCases
{
    public record CancelAppointmentUseCaseRequest(string ClientId, string AppointmentId);
}
