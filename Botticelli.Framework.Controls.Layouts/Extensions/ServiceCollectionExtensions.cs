using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Controls.Layouts.CommandProcessors.InlineCalendar;
using Botticelli.Framework.Controls.Layouts.Commands.InlineCalendar;
using Botticelli.Framework.Controls.Parsers;
using Botticelli.Framework.Extensions;
using Botticelli.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types.ReplyMarkups;

namespace Botticelli.Framework.Controls.Layouts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInlineCalendar<TReplyMarkup, TLayoutSupplier, TDateChosenCommandProcessor>(this IServiceCollection services)
            where TLayoutSupplier : class, ILayoutSupplier<TReplyMarkup>
            where TDateChosenCommandProcessor : CommandProcessor<DateChosenCommand>
        => services.AddScoped<ILayoutSupplier<TReplyMarkup>, TLayoutSupplier>()
                   .AddScoped<ICCommandProcessor<MonthForwardCommand, InlineKeyboardMarkup>>()
                   .AddScoped<ICCommandProcessor<MonthBackwardCommand, InlineKeyboardMarkup>>()
                   .AddScoped<ICCommandProcessor<YearForwardCommand, InlineKeyboardMarkup>>()
                   .AddScoped<ICCommandProcessor<YearBackwardCommand, InlineKeyboardMarkup>>()
                   .AddScoped<TDateChosenCommandProcessor>()
                   .AddScoped<ICommandValidator<YearForwardCommand>, PassValidator<YearForwardCommand>>()
                   .AddScoped<ICommandValidator<YearBackwardCommand>, PassValidator<YearBackwardCommand>>()
                   .AddScoped<ICommandValidator<MonthForwardCommand>, PassValidator<MonthForwardCommand>>()
                   .AddScoped<ICommandValidator<MonthBackwardCommand>, PassValidator<MonthBackwardCommand>>()
                   .AddScoped<ICommandValidator<DateChosenCommand>, PassValidator<DateChosenCommand>>();

    public static IServiceProvider UseInlineCalendar<TBot, TDateChosenCommandProcessor>(this IServiceProvider sp)
            where TDateChosenCommandProcessor : CommandProcessor<DateChosenCommand>
            where TBot : IBot<TBot> =>
            sp.RegisterBotCommand<ICCommandProcessor<MonthForwardCommand, InlineKeyboardMarkup>, TBot>()
              .RegisterBotCommand<ICCommandProcessor<MonthBackwardCommand, InlineKeyboardMarkup>, TBot>()
              .RegisterBotCommand<ICCommandProcessor<YearForwardCommand, InlineKeyboardMarkup>, TBot>()
              .RegisterBotCommand<ICCommandProcessor<YearBackwardCommand, InlineKeyboardMarkup>, TBot>()
              .RegisterBotCommand<TDateChosenCommandProcessor, TBot>();
}