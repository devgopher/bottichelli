using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Schedule.Quartz.Extensions;
using MessagingSample.Common.Commands;
using MessagingSample.Common.Commands.Processors;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTelegramBot(builder.Configuration)
    .AddLogging(cfg => cfg.AddNLog())
    .AddQuartzScheduler(builder.Configuration)
    .AddScoped<ILayoutParser, JsonLayoutParser>();

builder.Services.AddBotCommand<InfoCommand>()
    .AddProcessor<InfoCommandProcessor<ReplyKeyboardMarkup>>()
    .AddValidator<PassValidator<InfoCommand>>();

builder.Services.AddBotCommand<StartCommand>()
    .AddProcessor<StartCommandProcessor<ReplyKeyboardMarkup>>()
    .AddValidator<PassValidator<StartCommand>>();

builder.Services.AddBotCommand<StopCommand>()
    .AddProcessor<StopCommandProcessor<ReplyKeyboardMarkup>>()
    .AddValidator<PassValidator<StopCommand>>();

var app = builder.Build();
app.Services.RegisterBotCommand<StartCommand, StartCommandProcessor<ReplyKeyboardMarkup>, TelegramBot>()
    .RegisterProcessor<StopCommandProcessor<ReplyKeyboardMarkup>>()
    .RegisterProcessor<InfoCommandProcessor<ReplyKeyboardMarkup>>();

app.Run();