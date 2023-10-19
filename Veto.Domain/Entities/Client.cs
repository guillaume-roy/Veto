using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Domain.Entities
{
    public class Client
    {
        private readonly List<Patient> _patients = new();
        private readonly List<Appointment> _appointments = new();

        public string Id { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;

        public static Client New(string id, string email, string hashedPassword)
        {
            return new Client
            {
                Id = id,
                Email = SanitizeEmail(email),
                HashedPassword = hashedPassword,
            };
        }

        public static string SanitizeEmail(string email)
        {
            return email.ToLowerInvariant().Trim();
        }

        public void AddPatient(Patient patient)
        {
            if (GetPatientById(patient.Id) != null)
            {
                throw new PatientAlreadyExistsException();
            }

            _patients.Add(patient);
        }

        public Patient? GetPatientById(string patientId)
        {
            return _patients.FirstOrDefault(p => p.Id == patientId);
        }

        public void TryBookAppointment(string patientId, Appointment appointment, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            if (GetPatientById(patientId) == null)
            {
                throw new PatientNotFoundException();
            }

            CheckIfAppointmentDateIsAvailable(appointment.AppointmentDate, dateProvider, appointmentSlotConstraintsProvider);

            _appointments.Add(appointment);
        }

        public void CancelAppointment(string appointmentId)
        {
            var appointment = GetAppointmentById(appointmentId);
            _appointments.Remove(appointment);
        }

        public void TryRescheduleAppointment(string appointmentId, DateTime newAppointmentDate, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            var appointment = GetAppointmentById(appointmentId);

            CheckIfAppointmentDateIsAvailable(newAppointmentDate, dateProvider, appointmentSlotConstraintsProvider);

            _appointments.Remove(appointment);

            appointment.UpdateAppointmentDate(newAppointmentDate);

            _appointments.Add(appointment);
        }

        private Appointment GetAppointmentById(string appointmentId)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null)
            {
                throw new AppointmentNotFoundException();
            }
            return appointment;
        }

        private void CheckIfAppointmentDateIsAvailable(DateTime appointmentDate, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            var sanitizedDate = Appointment.SanitizeDate(appointmentDate);

            Appointment.CheckIfAppointmentDateIsValid(sanitizedDate, dateProvider, appointmentSlotConstraintsProvider);

            var alreadyHasAppointment = _appointments.Any(a => a.AppointmentDate == sanitizedDate);
            if (alreadyHasAppointment)
            {
                throw new AppointmentDateIsAlreadyBookedException();
            }
        }

        public List<DateTime> GetAvailableAppointmentSlots(DateTime requestDay, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            var availableSlots = new List<DateTime>();

            var startTime = new DateTime(requestDay.Year, requestDay.Month, requestDay.Day).Add(appointmentSlotConstraintsProvider.GetOpeningHour());
            var endTime = new DateTime(requestDay.Year, requestDay.Month, requestDay.Day).Add(appointmentSlotConstraintsProvider.GetClosingHour());
            var currentTime = startTime;

            var appointmentsForToday = _appointments.Where(a => a.AppointmentDate.Date == startTime.Date);

            while (currentTime < endTime)
            {
                if (!appointmentsForToday.Any(a => a.AppointmentDate == currentTime)
                    && currentTime.Add(appointmentSlotConstraintsProvider.GetSlotDuration()) <= endTime)
                {
                    availableSlots.Add(currentTime);
                }

                currentTime = currentTime.Add(appointmentSlotConstraintsProvider.GetSlotDuration());
            }

            return requestDay.Date == dateProvider.Now().Date
                ? availableSlots.Where(a => a > dateProvider.Now()).ToList()
                : availableSlots;
        }
    }
}
