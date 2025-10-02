using Main.Domain.SeedWork;

namespace Main.Domain.Enums;

public class BatchStatus(int id, string name) : Enumeration(id, name)
{
    public static readonly BatchStatus Pending = new(1, "Pending");
    public static readonly BatchStatus InProgress = new(2, "In Progress");
    public static readonly BatchStatus Completed = new(3, "Completed");
    public static readonly BatchStatus Failed = new(4, "Failed");
}
