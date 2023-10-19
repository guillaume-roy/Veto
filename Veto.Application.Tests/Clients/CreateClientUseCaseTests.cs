using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Clients.UseCases;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Persistence;
using Veto.Application.Security;
using Veto.Application.Tests.Mocks;
using Veto.Domain.Entities;

namespace Veto.Application.Tests.Clients
{
    internal class CreateClientUseCaseTests
    {
        private IApplicationStore _applicationStore;
        private IPasswordHasher _passwordHasher;
        private IEntityIdGenerator _entityIdGenerator;

        [SetUp]
        public void Setup()
        {
            _applicationStore = new ApplicationStoreMock();
            _passwordHasher = new PasswordHasherMock();
            _entityIdGenerator = new EntityIdGeneratorMock();
        }

        [Test]
        public void CreateClientUseCase_Should_Succeed()
        {
            var useCase = new CreateClientUseCase(_applicationStore, _passwordHasher, _entityIdGenerator);
            var clientId = useCase.Execute(new CreateClientUseCaseRequest("test@test.com", "123456789Test!"));
            Assert.IsNotNull(clientId);
        }

        [Test]
        public void CreateClientUseCase_Should_Fail_With_Existing_Email()
        {
            _applicationStore.Save(Client.New("1", "test@test.com", "123456789Test!"));

            var useCase = new CreateClientUseCase(_applicationStore, _passwordHasher, _entityIdGenerator);
            Assert.Throws<ClientAlreadyExistsException>(() => useCase.Execute(new CreateClientUseCaseRequest("test@test.com", "123456789Test!")));
        }

        [Test]
        public void CreateClientUseCase_Should_Fail_With_Invalid_Email()
        {
            var useCase = new CreateClientUseCase(_applicationStore, _passwordHasher, _entityIdGenerator);
            Assert.Throws<UseCaseValidationException>(() => useCase.Execute(new CreateClientUseCaseRequest("test", "123456789Test!")));
        }

        [Test]
        public void CreateClientUseCase_Should_Fail_With_Invalid_Password()
        {
            var useCase = new CreateClientUseCase(_applicationStore, _passwordHasher, _entityIdGenerator);
            Assert.Throws<UseCaseValidationException>(() => useCase.Execute(new CreateClientUseCaseRequest("test@test.com", "test")));
        }
    }
}
