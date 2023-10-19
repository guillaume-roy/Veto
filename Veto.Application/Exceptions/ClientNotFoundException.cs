using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Application.Exceptions
{
    public class ClientNotFoundException : Exception
    {
        public ClientNotFoundException() : base("client not found")
        {

        }
    }
}
