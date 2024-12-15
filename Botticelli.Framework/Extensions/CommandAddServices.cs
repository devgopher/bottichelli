using Botticelli.Bot.Interfaces.Processors;
using Botticelli.Framework.Commands;
using Botticelli.Framework.Commands.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Botticelli.Framework.Extensions;


public class CommandAddServices<TCommand>(IServiceCollection services)
    where TCommand : class, ICommand
{
    public CommandAddServices<TCommand> AddProcessor<TCommandProcessor, TConfiguration>(IConfiguration configuration)
        where TCommandProcessor : class, ICommandProcessor 
        where TConfiguration : class
    {
        services.Configure<TConfiguration>(configuration.GetSection(typeof(TConfiguration).Name));
        AddProcessor<TCommandProcessor>();
        
        return this;
    }
    
    public CommandAddServices<TCommand> AddProcessor<TCommandProcessor>()
            where TCommandProcessor : class, ICommandProcessor
    {
        services.AddScoped<TCommandProcessor>();
        
        return this;
    }

    public CommandAddServices<TCommand> AddValidator<TCommandValidator, TConfiguration>(IConfiguration configuration)
        where TCommandValidator : class, ICommandValidator<TCommand> 
        where TConfiguration : class
    {
        services.Configure<TConfiguration>(configuration.GetSection(typeof(TConfiguration).Name));
        
        // validator chain needs to be implemented!
        AddValidator<TCommandValidator>();
        
        return this;
    }
    
    public CommandAddServices<TCommand> AddValidator<TCommandValidator>() 
            where TCommandValidator : class, ICommandValidator<TCommand>
    {
        // validator chain needs to be implemented!
        services.AddScoped<TCommandValidator>()
                .AddScoped<ICommandValidator<TCommand>, TCommandValidator>();

        return this;
    }
}