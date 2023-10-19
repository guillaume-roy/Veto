using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Application.Entities
{
    public interface IEntityIdGenerator
    {
        string Generate();
    }
}
