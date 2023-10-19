using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veto.Application.Entities;
using Veto.Application.Persistence;
using Veto.Application.Security;
using Veto.Domain.Providers;
using Veto.Infrastructure.Entities;
using Veto.Infrastructure.Persistence;
using Veto.Infrastructure.Providers;
using Veto.Infrastructure.Security;

namespace Veto.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddScoped<IEntityIdGenerator, EntityIdGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAppointmentSlotConstraintsProvider, AppointmentSlotConstraintsProvider>();
            services.AddScoped<IDateProvider, DateProvider>();
            services.AddSingleton<IApplicationStore, InMemoryApplicationStore>();

            return services;
        }
    }
}
