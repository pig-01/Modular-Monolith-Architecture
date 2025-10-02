using System.Text.Json.Serialization;
using MediatR;

namespace Scheduler.Domain.SeedWork;

public abstract class Entity
{
    private readonly List<INotification> domainEvents = [];

    [JsonIgnore]
    public IReadOnlyCollection<INotification> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem) => domainEvents.Add(eventItem);

    public void RemoveDomainEvent(INotification eventItem) => domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => domainEvents.Clear();

    private DateTimeOffset createdDate = DateTimeOffset.UtcNow;

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("createdDate")]
    [Column(TypeName = "datetime")]
    public virtual DateTime CreatedDate
    {
        get => createdDate.DateTime;
        set => createdDate = value;
    }

    private string? createdUser = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("createdUser")]
    [StringLength(50)]
    public virtual string? CreatedUser
    {
        get => createdUser;
        set => createdUser = value;
    }

    private DateTimeOffset? modifiedDate = DateTimeOffset.UtcNow;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiedDate")]
    [Column(TypeName = "datetime")]
    public virtual DateTime? ModifiedDate
    {
        get => modifiedDate?.DateTime;
        set => modifiedDate = value;
    }

    private string? modifiedUser = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("modifiedUser")]
    [StringLength(50)]
    public virtual string? ModifiedUser
    {
        get => modifiedUser;
        set => modifiedUser = value;
    }

    /// <summary>
    /// Set the creation and modification metadata.
    /// </summary>
    /// <param name="createdUser"></param>
    /// <param name="modifiedUser"></param>
    public virtual void SetCreateMetadata(string createdUser, string modifiedUser)
    {
        this.createdUser = createdUser;
        this.modifiedUser = modifiedUser;
    }

    /// <summary>
    /// Set the modification metadata.
    /// </summary>
    /// <param name="modifiedUser"></param>
    public virtual void SetModifiedMetadata(string modifiedUser) => this.modifiedUser = modifiedUser;
}