using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Monads.Commands;
using Botticelli.Framework.Monads.Commands.Context;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Framework.Monads.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Framework.Telegram.Layout;
using Botticelli.Schedule.Quartz.Extensions;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMonadsBasedBot;
using TelegramMonadsBasedBot.Commands;
using TelegramMonadsBasedBot.Commands.Processors;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTelegramBot(builder.Configuration)
    .AddLogging(cfg => cfg.AddNLog())
    .AddQuartzScheduler(builder.Configuration)
    .AddHostedService<TestBotHostedService>()
    .AddScoped<ILayoutParser, JsonLayoutParser>()
    .AddTelegramLayoutsSupport();

builder.Services.AddBotCommand<StartCommand>()
    .AddMonadsChain<StartCommand, PassValidator<StartCommand>, ReplyMarkupBase, ReplyTelegramLayoutSupplier>(
        builder.Services,
        cb => cb.Next<StartCommandProcessor<ReplyMarkupBase>>()
            .Next<StartCommandPromptProcessor<ReplyMarkupBase>>());

builder.Services.AddBotCommand<MathCommand>()
    .AddMonadsChain<MathCommand, PassValidator<MathCommand>, ReplyMarkupBase, ReplyTelegramLayoutSupplier>(
        builder.Services,
        cb => cb.Next<InputCommandProcessor<MathCommand>>()
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Sqrt)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Cos)
            .Next<OutputCommandProcessor<ReplyMarkupBase, MathCommand>>());


// builder.Services.AddBotCommand<InfoCommand>()
//     .AddProcessor<TelegramMonadsBasedBot.Commands.Processors.InfoCommandProcessor<ReplyMarkupBase>>()
//     .AddValidator<PassValidator<InfoCommand>>();
//
//
// builder.Services.AddBotCommand<StopCommand>()
//     .AddProcessor<TelegramMonadsBasedBot.Commands.Processors.StopCommandProcessor<ReplyMarkupBase>>()
//     .AddValidator<PassValidator<StopCommand>>();

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();
app.Services.UseMonadsChain<StartCommand, TelegramBot>()
    .UseMonadsChain<MathCommand, TelegramBot>();
// .RegisterBotCommand<StartCommand, TelegramMonadsBasedBot.Commands.Processors.StartCommandProcessor<ReplyMarkupBase>, TelegramBot>()
// .RegisterProcessor<TelegramMonadsBasedBot.Commands.Processors.StopCommandProcessor<ReplyMarkupBase>>()
// .RegisterProcessor<TelegramMonadsBasedBot.Commands.Processors.InfoCommandProcessor<ReplyMarkupBase>>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();