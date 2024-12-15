using AiSample.Common;
using AiSample.Common.Commands;
using AiSample.Common.Handlers;
using AiSample.Common.Settings;
using Botticelli.AI.YaGpt.Extensions;
using Botticelli.Bus.None.Extensions;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Options;
using Botticelli.Framework.Vk.Messages;
using Botticelli.Framework.Vk.Messages.API.Markups;
using Botticelli.Framework.Vk.Messages.Extensions;
using Botticelli.Framework.Vk.Messages.Options;
using Botticelli.Interfaces;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration
                      .GetSection(nameof(SampleSettings))
                      .Get<SampleSettings>();

builder.Services.AddVkBot(builder.Configuration)
       .AddLogging(cfg => cfg.AddNLog())
       .AddYaGptProvider(builder.Configuration)
       .AddScoped<ICommandValidator<AiCommand>, PassValidator<AiCommand>>()
       .AddSingleton<AiHandler>()
       .UsePassBusAgent<IBot<VkBot>, AiHandler>()
       .UsePassBusClient<IBot<VkBot>>()
       .AddBotCommand<AiCommand, AiCommandProcessor<VkKeyboardMarkup>, PassValidator<AiCommand>>();

var app = builder.Build();
app.Services.RegisterBotCommand<AiCommandProcessor<VkKeyboardMarkup>, VkBot>();

app.Run();