using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Schedule.Quartz.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMonadsBasedBot;
using InfoCommand = TelegramMonadsBasedBot.Commands.InfoCommand;
using StartCommand = TelegramMonadsBasedBot.Commands.StartCommand;
using StopCommand = TelegramMonadsBasedBot.Commands.StopCommand;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTelegramBot(builder.Configuration)
    .AddLogging(cfg => cfg.AddNLog())
    .AddQuartzScheduler(builder.Configuration)
    .AddHostedService<TestBotHostedService>()
    .AddScoped<ILayoutParser, JsonLayoutParser>();

builder.Services.AddBotCommand<InfoCommand>()
    .AddProcessor<TelegramMonadsBasedBot.Commands.Processors.InfoCommandProcessor<ReplyMarkupBase>>()
    .AddValidator<PassValidator<InfoCommand>>();

builder.Services.AddBotCommand<StartCommand>()
    .AddProcessor<TelegramMonadsBasedBot.Commands.Processors.StartCommandProcessor<ReplyMarkupBase>>()
    .AddValidator<PassValidator<StartCommand>>();

builder.Services.AddBotCommand<StopCommand>()
    .AddProcessor<TelegramMonadsBasedBot.Commands.Processors.StopCommandProcessor<ReplyMarkupBase>>()
    .AddValidator<PassValidator<StopCommand>>();

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();
app.Services.RegisterBotCommand<StartCommand, TelegramMonadsBasedBot.Commands.Processors.StartCommandProcessor<ReplyMarkupBase>, TelegramBot>()
    .RegisterProcessor<TelegramMonadsBasedBot.Commands.Processors.StopCommandProcessor<ReplyMarkupBase>>()
    .RegisterProcessor<TelegramMonadsBasedBot.Commands.Processors.InfoCommandProcessor<ReplyMarkupBase>>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();