using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Veto.Application.Appointments.UseCases;
using Veto.Application.Clients.UseCases;
using Veto.Application.Entities;
using Veto.Application.Exceptions;
using Veto.Application.Patients.UseCases;
using Veto.Application.Persistence;
using Veto.Application.Security;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;
using Veto.Web.Models;

namespace Veto.Web.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly IApplicationStore _applicationStore;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEntityIdGenerator _entityIdGenerator;
        private readonly IMapper _mapper;
        private readonly IDateProvider _dateProvider;
        private readonly IAppointmentSlotConstraintsProvider _slotConstraintsProvider;

        public ClientController(IApplicationStore applicationStore, IPasswordHasher passwordHasher, IEntityIdGenerator entityIdGenerator, IMapper mapper,
            IDateProvider dateProvider, IAppointmentSlotConstraintsProvider slotConstraintsProvider)
        {
            _applicationStore = applicationStore;
            _passwordHasher = passwordHasher;
            _entityIdGenerator = entityIdGenerator;
            _mapper = mapper;
            _dateProvider = dateProvider;
            _slotConstraintsProvider = slotConstraintsProvider;
        }

        [HttpPost]
        public IActionResult CreateClient([FromBody] CreateClientDto dto)
        {
            try
            {
                var useCase = new CreateClientUseCase(_applicationStore, _passwordHasher, _entityIdGenerator);
                var clientId = useCase.Execute(_mapper.Map<CreateClientUseCaseRequest>(dto));

                return StatusCode((int)HttpStatusCode.Created, new { clientId });
            }
            catch (Exception e) when (
                e is UseCaseValidationException
                || e is ClientAlreadyExistsException
            )
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost("{clientId}/patients")]
        public IActionResult CreatePatient([FromRoute] string clientId, [FromBody] CreatePatientDto dto)
        {
            try
            {
                var useCase = new CreatePatientUseCase(_applicationStore, _entityIdGenerator);
                var patientId = useCase.Execute(new CreatePatientUseCaseRequest(clientId, dto.PatientName));

                return StatusCode((int)HttpStatusCode.Created, new { patientId });
            }
            catch (Exception e) when (
                e is UseCaseValidationException
                || e is ClientNotFoundException
                || e is PatientAlreadyExistsException
            )
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet("{clientId}/slots")]
        public IActionResult GetAvailableAppointmentSlots([FromRoute] string clientId, [FromQuery] GetAvailableAppointmentSlotsDto dto)
        {
            try
            {
                var useCase = new GetAvailableAppointmentSlotsUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
                var availableSlots = useCase.Execute(new GetAvailableAppointmentSlotsUseCaseRequest(clientId, dto.RequestDay));

                return Ok(availableSlots);
            }
            catch (Exception e) when (
                e is UseCaseValidationException
                || e is ClientNotFoundException
            )
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost("{clientId}/appointments")]
        public IActionResult BookAppointment([FromRoute] string clientId, [FromBody] BookAppointmentDto dto)
        {
            try
            {
                var useCase = new BookAppointmentUseCase(_applicationStore, _entityIdGenerator, _dateProvider, _slotConstraintsProvider);
                var appointmentId = useCase.Execute(new BookAppointmentUseCaseRequest(clientId, dto.PatientId, dto.AppointmentDate));

                return StatusCode((int)HttpStatusCode.Created, new { appointmentId });
            }
            catch (Exception e) when (
                e is UseCaseValidationException
                || e is ClientNotFoundException
                || e is PatientNotFoundException
                || e is AppointmentDateIsInvalidException
                || e is AppointmentDateIsAlreadyBookedException
            )
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPut("{clientId}/appointments/{appointmentId}")]
        public IActionResult RescheduleAppointment([FromRoute] string clientId, [FromRoute] string appointmentId, [FromBody] RescheduleAppointmentDto dto)
        {
            try
            {
                var useCase = new RescheduleAppointmentUseCase(_applicationStore, _dateProvider, _slotConstraintsProvider);
                useCase.Execute(new RescheduleAppointmentUseCaseRequest(clientId, appointmentId, dto.NewAppointmentDate));

                return Ok();
            }
            catch (Exception e) when (
                e is UseCaseValidationException
                || e is ClientNotFoundException
                || e is AppointmentNotFoundException
                || e is AppointmentDateIsInvalidException
                || e is AppointmentDateIsAlreadyBookedException
            )
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpDelete("{clientId}/appointments/{appointmentId}")]
        public IActionResult CancelAppointment([FromRoute] string clientId, [FromRoute] string appointmentId)
        {
            try
            {
                var useCase = new CancelAppointmentUseCase(_applicationStore);
                useCase.Execute(new CancelAppointmentUseCaseRequest(clientId, appointmentId));

                return Ok();
            }
            catch (Exception e) when (
                e is UseCaseValidationException
                || e is ClientNotFoundException
                || e is AppointmentNotFoundException
            )
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
