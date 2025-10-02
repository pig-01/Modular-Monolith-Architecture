namespace Scheduler.Domain.Enums;

public class MailQueueStatus(int id, string name) : Enumeration(id, name)
{
    public static readonly MailQueueStatus Pending = new(1, "Pending");
    public static readonly MailQueueStatus Sent = new(2, "Sent");
    public static readonly MailQueueStatus Failed = new(3, "Failed");
}
