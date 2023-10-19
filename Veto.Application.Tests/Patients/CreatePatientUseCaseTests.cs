using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Clients.UseCases;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Patients.UseCases;
using Veto.Application.Persistence;
using Veto.Application.Security;
using Veto.Application.Tests.Mocks;
using Veto.Domain.Entities;

namespace Veto.Application.Tests.Patients
{
    internal class CreatePatientUseCaseTests
    {
        private IApplicationStore _applicationStore;
        private IEntityIdGenerator _entityIdGenerator;

        [SetUp]
        public void Setup()
        {
            _applicationStore = new ApplicationStoreMock();
            _entityIdGenerator = new EntityIdGeneratorMock();
        }

        [Test]
        public void CreateClientUseCase_Should_Succeed()
        {
            _applicationStore.Save(Client.New("1", "test@test.com", "123456789Test!"));

            var useCase = new CreatePatientUseCase(_applicationStore, _entityIdGenerator);
            var patientId = useCase.Execute(new CreatePatientUseCaseRequest("1", "John Doe"));
            Assert.IsNotNull(patientId);
        }

        [Test]
        public void CreateClientUseCase_Should_Fail_With_NotFound_Client()
        {
            var useCase = new CreatePatientUseCase(_applicationStore, _entityIdGenerator);
            Assert.Throws<ClientNotFoundException>(() => useCase.Execute(new CreatePatientUseCaseRequest("1", "John Doe")));
        }

        [Test]
        public void CreateClientUseCase_Should_Fail_With_Invalid_ClientId()
        {
            _applicationStore.Save(Client.New("1", "test@test.com", "123456789Test!"));

            var useCase = new CreatePatientUseCase(_applicationStore, _entityIdGenerator);
            Assert.Throws<UseCaseValidationException>(() => useCase.Execute(new CreatePatientUseCaseRequest("", "John Doe")));
        }

        [Test]
        public void CreateClientUseCase_Should_Fail_With_Invalid_PatientName()
        {
            _applicationStore.Save(Client.New("1", "test@test.com", "123456789Test!"));

            var useCase = new CreatePatientUseCase(_applicationStore, _entityIdGenerator);
            Assert.Throws<UseCaseValidationException>(() => useCase.Execute(new CreatePatientUseCaseRequest("1", "")));
        }
    }
}
