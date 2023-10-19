using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Security;

namespace Veto.Application.Tests.Mocks
{
    internal class PasswordHasherMock : IPasswordHasher
    {
        public string Hash(string password)
        {
            return password;
        }
    }
}
