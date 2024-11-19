using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Schedule.Quartz.Extensions;
using Botticelli.Talks.Extensions;
using MessagingSample.Common.Commands;
using MessagingSample.Common.Commands.Processors;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramCommandChainSample;
using TelegramCommandChainSample.Commands;
using TelegramCommandChainSample.Commands.CommandProcessors;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddTelegramBot(builder.Configuration)
       .AddLogging(cfg => cfg.AddNLog())
       .AddQuartzScheduler(builder.Configuration)
       .AddScoped<StartCommandProcessor<ReplyMarkupBase>>()
       .AddScoped<StopCommandProcessor<ReplyMarkupBase>>()
       .AddScoped<InfoCommandProcessor<ReplyMarkupBase>>()
       .AddOpenTtsTalks(builder.Configuration)
       .AddScoped<ILayoutParser, JsonLayoutParser>()
       .AddBotCommand<InfoCommand, InfoCommandProcessor<ReplyMarkupBase>, PassValidator<InfoCommand>>()
       .AddBotCommand<StartCommand, StartCommandProcessor<ReplyMarkupBase>, PassValidator<StartCommand>>()
       .AddBotCommand<StopCommand, StopCommandProcessor<ReplyMarkupBase>, PassValidator<StopCommand>>();


// Command processing chain is being initialized here...
builder.Services.AddBotChainProcessedCommand<GetNameCommand, PassValidator<GetNameCommand>>()
       .AddNext<GetNameCommandProcessor>()
       .AddNext<SayHelloFinalCommandProcessor>();

var app = builder.Build();

app.Services.RegisterBotChainedCommand<GetNameCommand, TelegramBot>();

app.Run();