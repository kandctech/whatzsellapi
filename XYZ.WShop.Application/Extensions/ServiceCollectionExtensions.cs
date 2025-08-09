using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Extensions
{
    /// <summary>
    /// Extensions for configuring application services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds application services to the service collection.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services.AddFluentValidation();
        }
    }
}
