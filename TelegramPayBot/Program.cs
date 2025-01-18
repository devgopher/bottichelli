using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Pay.Telegram.Extensions;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramPayBot.Commands;
using TelegramPayBot.Commands.Processors;
using TelegramPayBot.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTelegramPayBot<PayPreCheckoutHandler>(builder.Configuration)
    .AddLogging(cfg => cfg.AddNLog())
    .AddScoped<ILayoutParser, JsonLayoutParser>();

builder.Services.AddBotCommand<InfoCommand>()
    .AddProcessor<InfoCommandProcessor<ReplyKeyboardMarkup>>()
    .AddValidator<PassValidator<InfoCommand>>();

var app = builder.Build();
app.Services.RegisterBotCommand<InfoCommand, InfoCommandProcessor<ReplyKeyboardMarkup>, TelegramBot>();

app.Run();