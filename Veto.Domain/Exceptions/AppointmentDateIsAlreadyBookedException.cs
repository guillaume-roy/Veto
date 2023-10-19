﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veto.Domain.Exceptions
{
    public class AppointmentDateIsAlreadyBookedException : Exception
    {
        public AppointmentDateIsAlreadyBookedException() : base("date not available")
        {

        }
    }
}
