using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.AggregatesModel.TenantAggregate;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.OrderAggregate;

[Table("OrderTenant")]
public partial class OrderTenant(string tenantId, long orderId) : Entity
{
    [Key]
    [Column("OrderTenantID")]
    public long OrderTenantId { get; set; }

    [Column("OrderID")]
    public long OrderId { get; set; } = orderId;

    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = tenantId;

    [ForeignKey("OrderId")]
    [InverseProperty("OrderTenants")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("TenantId")]
    [InverseProperty("OrderTenants")]
    public virtual Tenant Tenant { get; set; } = null!;
}
