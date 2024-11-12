using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
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

builder.Services.AddBotCommand<MathCommand>()
    .AddMonadsChain<MathCommand, PassValidator<MathCommand>, ReplyMarkupBase, ReplyTelegramLayoutSupplier>(
        builder.Services,
        cb => cb.Next<InputCommandProcessor<MathCommand>>()
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Sqrt)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Cos)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Log10)
            .Next<OutputCommandProcessor<ReplyMarkupBase, MathCommand>>());

var app = builder.Build();
app.Services.UseMonadsChain<MathCommand, TelegramBot>();

app.Run();