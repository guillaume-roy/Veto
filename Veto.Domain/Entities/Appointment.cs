using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Domain.Exceptions;
using Veto.Domain.Providers;

namespace Veto.Domain.Entities
{
    public class Appointment
    {
        public string Id { get; private set; } = string.Empty;
        public DateTime AppointmentDate { get; private set; }

        public static Appointment New(string id, DateTime appointmentDate)
        {
            return new Appointment
            {
                Id = id,
                AppointmentDate = SanitizeDate(appointmentDate)
            };
        }

        public void UpdateAppointmentDate(DateTime newAppointmentDate)
        {
            AppointmentDate = SanitizeDate(newAppointmentDate);
        }

        public static DateTime SanitizeDate(DateTime appointmentDate)
        {
            return new DateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day, appointmentDate.Hour, appointmentDate.Minute, 0, appointmentDate.Kind);
        }

        public static void CheckIfAppointmentDateIsValid(DateTime appointmentDate, IDateProvider dateProvider, IAppointmentSlotConstraintsProvider appointmentSlotConstraintsProvider)
        {
            var sanitizedDate = SanitizeDate(appointmentDate);

            if (sanitizedDate < dateProvider.Now())
            {
                throw new AppointmentDateIsInvalidException();
            }

            if (sanitizedDate.TimeOfDay < appointmentSlotConstraintsProvider.GetOpeningHour())
            {
                throw new AppointmentDateIsInvalidException();
            }

            if (sanitizedDate.TimeOfDay.Add(appointmentSlotConstraintsProvider.GetSlotDuration()) > appointmentSlotConstraintsProvider.GetClosingHour())
            {
                throw new AppointmentDateIsInvalidException();
            }

            if (sanitizedDate.TimeOfDay.Ticks % appointmentSlotConstraintsProvider.GetSlotDuration().Ticks != 0)
            {
                throw new AppointmentDateIsInvalidException();
            }
        }
    }
}
