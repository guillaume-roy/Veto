using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Domain.Providers;

namespace Veto.Infrastructure.Providers
{
    public class DateProvider : IDateProvider
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
