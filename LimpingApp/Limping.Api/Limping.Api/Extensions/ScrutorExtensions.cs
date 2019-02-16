using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Limping.Api.Extensions
{
    public static class ScrutorExtensions
    {
        public static ILifetimeSelector AsSelfWithAliases(this IServiceTypeSelector selector, IServiceCollection services, ServiceLifetime lifetime, Func<Type, IEnumerable<Type>> aliasSelector)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (aliasSelector == null)
            {
                throw new ArgumentNullException(nameof(aliasSelector));
            }

            return selector.As(type =>
            {
                var aliases = aliasSelector(type);
                foreach (var alias in aliases)
                {
                    services.Add(new ServiceDescriptor(alias, serviceProvider => serviceProvider.GetRequiredService(type), lifetime));
                }

                return new[] { type };
            });
        }
    }
}
