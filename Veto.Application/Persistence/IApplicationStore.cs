using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Domain.Entities;

namespace Veto.Application.Persistence
{
    public interface IApplicationStore
    {
        IEnumerable<Client> Clients { get; }

        void Save(Client client);
    }
}
