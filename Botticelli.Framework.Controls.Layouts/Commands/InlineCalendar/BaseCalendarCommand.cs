using Botticelli.Framework.Commands;

namespace Botticelli.Framework.Controls.Layouts.Commands.InlineCalendar;

public abstract class BaseCalendarCommand : ICommand
{
    public DateTime CurrentDt { get; set; }
    public Guid Id { get; }
}