using System.Globalization;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Utils;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Layouts.Commands.InlineCalendar;
using Botticelli.Framework.Controls.Layouts.Inlines;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Controls.Layouts.CommandProcessors.InlineCalendar;

/// <summary>
///     Calendar command processor
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TReplyMarkup"></typeparam>
public class ICCommandProcessor<TCommand, TReplyMarkup> : CommandProcessor<TCommand>
    where TCommand : BaseCalendarCommand where TReplyMarkup : class
{
    private readonly ILayoutSupplier<TReplyMarkup> _layoutSupplier;

    public ICCommandProcessor(ILogger<ICCommandProcessor<TCommand, TReplyMarkup>> logger,
        ICommandValidator<TCommand> commandValidator,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        MetricsProcessor metricsProcessor,
        IValidator<Message> messageValidator)
        : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
        _layoutSupplier = layoutSupplier;
    }

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        Inlines.InlineCalendar calendar;

        if (!DateTime.TryParse(message.Body?.GetArguments(), out var dt)) return;

        if (typeof(TCommand) == typeof(MonthBackwardCommand))
            calendar = CalendarFactory.GetMonthsForward(dt, CultureInfo.InvariantCulture.Name, -1);
        else if (typeof(TCommand) == typeof(MonthForwardCommand))
            calendar = CalendarFactory.GetMonthsForward(dt, CultureInfo.InvariantCulture.Name);
        else if (typeof(TCommand) == typeof(YearBackwardCommand))
            calendar = CalendarFactory.GetMonthsForward(dt, CultureInfo.InvariantCulture.Name, -12);
        else if (typeof(TCommand) == typeof(YearForwardCommand))
            calendar = CalendarFactory.GetMonthsForward(dt, CultureInfo.InvariantCulture.Name, 12);
        else
            calendar = CalendarFactory.Get(DateTime.Now, CultureInfo.InvariantCulture.Name);

        var responseMarkup = _layoutSupplier.GetMarkup(calendar);
        var options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);

        await Bot.UpdateMessageAsync(new SendMessageRequest
            {
                ExpectPartialResponse = false,
                Message = new Message
                {
                    Body = message.CallbackData,
                    Uid = message.Uid,
                    ChatIds = message.ChatIds,
                    ChatIdInnerIdLinks = message.ChatIdInnerIdLinks
                }
            },
            options,
            token);
    }
}