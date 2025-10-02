using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanSearchAggreate;

[Table("PlanSearchHistory")]
public class PlanSearchHistory : Entity, IAggregateRoot
{
    [Key]
    [Column("PlanSearchHistoryID")]
    public int PlanSearchHistoryId { get; set; }

    [Required]
    [Column("UserID")]
    public required string UserId { get; set; }

    [Required]
    [Column("TenantID")]
    public required string TenantId { get; set; }

    public string? KeyWord { get; set; }

    public static PlanSearchHistory Create(string keyWord, string userId, DateTime createdDate, string createdUser, DateTime modifiedDate, string modifiedUser, string tenantId)
    {
        return new PlanSearchHistory()
        {
            UserId = userId,
            KeyWord = keyWord,
            CreatedDate = createdDate,
            CreatedUser = createdUser,
            ModifiedDate = modifiedDate,
            ModifiedUser = modifiedUser,
            TenantId = tenantId
        };
    }
}