using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.SendOptions;
using Botticelli.Scheduler;
using Botticelli.Scheduler.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.Constants;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

namespace MessagingSample.Common.Commands.Processors;

public class StartCommandProcessor<TReplyMarkup> : CommandProcessor<StartCommand> where TReplyMarkup : class
{
    private readonly IJobManager _jobManager;
    private readonly SendOptionsBuilder<TReplyMarkup>? _options;
    
    public StartCommandProcessor(ILogger<StartCommandProcessor<TReplyMarkup>> logger,
                                 ICommandValidator<StartCommand> commandValidator,
                                 MetricsProcessor metricsProcessor,
                                 IJobManager jobManager,
                                 ILayoutSupplier<TReplyMarkup> layoutSupplier,
                                 ILayoutParser layoutParser, 
                                 IValidator<Message> messageValidator)
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
        _jobManager = jobManager;

        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);
        
        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }

    protected override Task InnerProcessContact(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessPoll(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessLocation(Message message, CancellationToken token) => Task.CompletedTask;

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var chatId = message.ChatIds.FirstOrDefault();
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "Bot started..."
            }
        };

        await Bot.SendMessageAsync(greetingMessageRequest, _options, token);

        var assemblyPath = Path.GetDirectoryName(typeof(StartCommandProcessor<TReplyMarkup>).Assembly.Location);
        _jobManager.AddJob(Bot,
            new Reliability
            {
                IsEnabled = false,
                Delay = TimeSpan.FromSeconds(3),
                IsExponential = true,
                MaxTries = 5
            },
            new Message
            {
                Body = "Now you see me!",
                ChatIds = [chatId],
                Contact = new Contact
                {
                    Phone = "+9003289384923842343243243",
                    Name = "Test",
                    Surname = "Botticelli"
                },
                Poll = new Poll
                {
                    Question = "To be or not to be?",
                    Variants = new []
                    {
                        "To be!",
                        "Not to be!"
                    },
                    CorrectAnswerId = 0,
                    IsAnonymous = false,
                    Type = Poll.PollType.Quiz
                },
                Attachments =
                [
                    new BinaryBaseAttachment(Guid.NewGuid().ToString(),
                        "testpic.png",
                        MediaType.Image,
                        string.Empty,
                        await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/testpic.png"), token)),

                    new BinaryBaseAttachment(Guid.NewGuid().ToString(),
                        "voice.mp3",
                        MediaType.Voice,
                        string.Empty,
                        await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/voice.mp3"), token)),

                    new BinaryBaseAttachment(Guid.NewGuid().ToString(),
                        "video.mp4",
                        MediaType.Video,
                        string.Empty,
                        await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/video.mp4"), token)),

                    new BinaryBaseAttachment(Guid.NewGuid().ToString(),
                        "document.odt",
                        MediaType.Document,
                        string.Empty,
                        await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/document.odt"), token))
                ]
            },
            new Schedule
            {
                Cron = "*/30 * * ? * * *"
            });
    }
}