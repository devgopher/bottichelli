using System.Reflection;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Monads.Commands.Processors;
using Botticelli.Framework.Monads.Commands.Result;
using Botticelli.Framework.SendOptions;
using Botticelli.Scheduler;
using Botticelli.Scheduler.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.Constants;
using Botticelli.Shared.ValueObjects;

namespace TelegramMonadsBasedBot.Commands.Processors;

public class StartCommandProcessor<TReplyMarkup> : ChainProcessor<StartCommand>
    where TReplyMarkup : class
{
    private readonly IJobManager _jobManager;
    private readonly SendOptionsBuilder<TReplyMarkup> _options;

    public StartCommandProcessor(ILogger<StartCommandProcessor<TReplyMarkup>> logger,
        IJobManager jobManager,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser)
        : base(logger)
    {
        _jobManager = jobManager;

        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);

        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }


    protected override async Task InnerProcessAsync(IResult<StartCommand> stepResult, CancellationToken token)
    {
        var message = GetMessage(stepResult.Command);

        var chatId = message.ChatIds.FirstOrDefault();
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                Uid = Guid.NewGuid().ToString(),
                ChatIds = message.ChatIds,
                Body = "Сhain step 1: Bot started..."
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
                    Variants = new[]
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

    protected override Task InnerErrorProcessAsync(IResult<StartCommand> command, CancellationToken token) =>
        throw new NotImplementedException();
}