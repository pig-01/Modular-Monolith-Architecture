using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanFactory")]
public partial class PlanFactory : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanID")]
    public int PlanId { get; set; }

    [Column("FactoryID")]
    public string? FactoryId { get; set; }

    [StringLength(10)]
    [Column("TenantID")]
    public string? TenantId { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("PlanFactories")]
    public virtual Plan Plan { get; set; } = null!;

    /// <summary>
    /// 建立 PlanFactory 實例
    /// </summary>
    /// <param name="factoryId"></param>
    /// <param name="tenantId"></param>
    /// <param name="createdUser"></param>
    /// <param name="modifiedUser"></param>
    /// <returns></returns>
    public static PlanFactory Create(string factoryId, string tenantId, string createdUser, string modifiedUser) => new()
    {
        FactoryId = factoryId,
        TenantId = tenantId,
        CreatedUser = createdUser,
        ModifiedUser = modifiedUser,
    };
}