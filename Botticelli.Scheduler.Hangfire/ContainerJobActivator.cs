using System;
using System.Linq;
using Botticelli.Shared.Utils;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Scheduler.Hangfire;

public class ContainerJobActivator(IServiceCollection services) : JobActivator
{
    public override object ActivateJob(Type type)
    {
        var realTypeDescriptor = services
            .AsEnumerable()
            .FirstOrDefault(s => s.ServiceType is { FullName: not null, IsInterface: true } &&
                                 s.ServiceType
                                     .FullName
                                     .ToLowerInvariant()
                                     .ContainsStrings("ibot`", $"{type.Name.ToLowerInvariant()}"));


        realTypeDescriptor.NotNull();

        return services.BuildServiceProvider().GetRequiredService(realTypeDescriptor.ServiceType);
    }
}