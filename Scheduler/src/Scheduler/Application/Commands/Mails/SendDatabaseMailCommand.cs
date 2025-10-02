using MediatR;

namespace Scheduler.Application.Commands.Mails;

/// <summary>
/// Command to send database emails.
/// </summary>
public class SendDatabaseMailCommand : IRequest<bool>
{

}
