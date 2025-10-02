using System.ComponentModel;
using Main.Domain.SeedWork;

namespace Main.Domain.Enums;

public class DocumentState(int id, string name) : Enumeration(id, name)
{
    public static readonly DocumentState UnWritten = new(0, nameof(UnWritten));
    public static readonly DocumentState Written = new(1, nameof(Written));
    public static readonly DocumentState Approving = new(2, nameof(Approving));
    public static readonly DocumentState SendBack = new(3, nameof(SendBack));
    public static readonly DocumentState Canceled = new(4, nameof(Canceled));
    public static readonly DocumentState Approved = new(5, nameof(Approved));
    public static readonly DocumentState Rejected = new(6, nameof(Rejected));
}
