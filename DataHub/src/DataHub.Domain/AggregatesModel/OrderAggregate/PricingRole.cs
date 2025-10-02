using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.OrderAggregate;

[PrimaryKey("PricingCode", "RoleCode")]
[Table("PricingRole")]
public partial class PricingRole
{
    [Key]
    [StringLength(50)]
    public string PricingCode { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string RoleCode { get; set; } = null!;

    [StringLength(50)]
    public string? RoleName { get; set; }
}
