using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Domain.Providers;

namespace Veto.Application.Tests.Mocks
{
    internal class DateProviderMock : IDateProvider
    {
        public DateTime MockValue { get; set; } = DateTime.Now;

        public DateTime Now()
        {
            return MockValue;
        }
    }
}
