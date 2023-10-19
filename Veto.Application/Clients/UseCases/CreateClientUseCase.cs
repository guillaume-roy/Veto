using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Application.Security;
using Veto.Domain.Entities;

namespace Veto.Application.Clients.UseCases
{
    public class CreateClientUseCase
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEntityIdGenerator _entityIdGenerator;

        public CreateClientUseCase(IApplicationStore applicationStore, IPasswordHasher passwordHasher, IEntityIdGenerator entityIdGenerator)
        {
            _applicationStore = applicationStore;
            _passwordHasher = passwordHasher;
            _entityIdGenerator = entityIdGenerator;
        }

        public string Execute(CreateClientUseCaseRequest request)
        {
            var validator = new CreateClientUseCaseValidator();
            validator.Validate(request);

            var existingClient = _applicationStore.Clients.FirstOrDefault(c => c.Email == Client.SanitizeEmail(request.Email));
            if (existingClient != null)
            {
                throw new ClientAlreadyExistsException();
            }

            var client = Client.New(
                _entityIdGenerator.Generate(),
                request.Email,
                _passwordHasher.Hash(request.Password));

            _applicationStore.Save(client);

            return client.Id;
        }
    }
}
