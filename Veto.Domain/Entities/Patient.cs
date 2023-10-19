using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Domain.Entities
{
    public class Patient
    {
        public string Id { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;

        public static Patient New(string id, string name)
        {
            return new Patient
            {
                Id = id,
                Name = name,
            };
        }
    }
}
