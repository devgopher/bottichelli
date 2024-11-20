using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Vk.Messages;
using Botticelli.Framework.Vk.Messages.API.Markups;
using Botticelli.Framework.Vk.Messages.Extensions;
using Botticelli.Schedule.Quartz.Extensions;
using Botticelli.Talks.Extensions;
using MessagingSample.Common.Commands;
using MessagingSample.Common.Commands.Processors;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddVkBot(builder.Configuration)
    .AddLogging(cfg => cfg.AddNLog())
    .AddQuartzScheduler(builder.Configuration)
    .AddScoped<StartCommandProcessor<VkKeyboardMarkup>>()
    .AddScoped<StopCommandProcessor<VkKeyboardMarkup>>()
    .AddOpenTtsTalks(builder.Configuration)
    .AddBotCommand<StartCommand, StartCommandProcessor<VkKeyboardMarkup>, PassValidator<StartCommand>>()
    .AddBotCommand<StopCommand, StopCommandProcessor<VkKeyboardMarkup>, PassValidator<StopCommand>>();


var app = builder.Build();
app.Services.RegisterBotCommand<StartCommandProcessor<VkKeyboardMarkup>, VkBot>();
app.Services.RegisterBotCommand<StopCommandProcessor<VkKeyboardMarkup>, VkBot>();

app.Run();