using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Domain.Exceptions
{
    public class PatientAlreadyExistsException : Exception
    {
        public PatientAlreadyExistsException() : base("patient already exists")
        {

        }
    }
}
