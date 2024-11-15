using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedValidation(this IServiceCollection services) 
        => services.AddValidatorsFromAssemblyContaining<Message>(ServiceLifetime.Singleton);
}