using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Persistence;
using Veto.Domain.Entities;

namespace Veto.Application.Tests.Mocks
{
    internal class ApplicationStoreMock : IApplicationStore
    {
        private readonly Dictionary<string, Client> _dataSource = new();

        public IEnumerable<Client> Clients => _dataSource.Values.ToList();

        public void Save(Client client)
        {
            if (_dataSource.ContainsKey(client.Id))
            {
                _dataSource[client.Id] = client;
            }
            else
            {
                _dataSource.Add(client.Id, client);
            }
        }
    }
}
