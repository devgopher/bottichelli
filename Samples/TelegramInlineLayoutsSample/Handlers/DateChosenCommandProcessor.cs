using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Utils;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Layouts.Commands.InlineCalendar;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace TelegramInlineLayoutsSample.Handlers;

public class DateChosenCommandProcessor(
        IBot bot,
        ICommandValidator<DateChosenCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        ILogger<GetCalendarCommandProcessor> logger,
        IValidator<Message> messageValidator)
        : CommandProcessor<DateChosenCommand>(logger,
                                              commandValidator,
                                              metricsProcessor,
                                              messageValidator)
{
    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var request = new SendMessageRequest
        {
            ExpectPartialResponse = false,
            Message = message
        };

        request.Message.Body = message.CallbackData?.GetArguments();
        await bot.SendMessageAsync(request, token);
    }
}