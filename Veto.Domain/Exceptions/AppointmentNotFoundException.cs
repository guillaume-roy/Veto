using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Domain.Exceptions
{
    public class AppointmentNotFoundException : Exception
    {
        public AppointmentNotFoundException() : base("appointment not found")
        {

        }
    }
}
