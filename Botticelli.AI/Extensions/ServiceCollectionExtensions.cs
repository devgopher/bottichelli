using Botticelli.AI.Message;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.AI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiValidation(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<Shared.ValueObjects.Message>(ServiceLifetime.Singleton)
            .AddValidatorsFromAssemblyContaining<AiMessage>(ServiceLifetime.Singleton);
}