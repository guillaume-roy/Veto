using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Entities;

namespace Veto.Application.Tests.Mocks
{
    internal class EntityIdGeneratorMock : IEntityIdGenerator
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
