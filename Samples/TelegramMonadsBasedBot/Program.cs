using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Framework.Monads.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Framework.Telegram.Layout;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramMonadsBasedBot.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTelegramBot(builder.Configuration)
    .AddLogging(cfg => cfg.AddNLog())
    .AddTelegramLayoutsSupport();

builder.Services.AddBotCommand<MathCommand>()
    .AddMonadsChain<MathCommand, PassValidator<MathCommand>, ReplyKeyboardMarkup, ReplyTelegramLayoutSupplier>(
        builder.Services,
        cb => cb.Next<InputCommandProcessor<MathCommand>>()
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Sqrt)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Sqrt)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Cos)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Abs)
            .Next<TransformArgumentsProcessor<MathCommand, double>>(tp => tp.SuccessFunc = Math.Sqrt)
            .Next<OutputCommandProcessor<ReplyKeyboardMarkup, MathCommand>>());

var app = builder.Build();
app.Services.UseMonadsChain<MathCommand, TelegramBot>();

app.Run();