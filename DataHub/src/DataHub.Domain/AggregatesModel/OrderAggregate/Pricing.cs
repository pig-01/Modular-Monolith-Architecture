using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.OrderAggregate;

[Table("Pricing")]
public partial class Pricing
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string PricingCode { get; set; } = null!;

    [StringLength(100)]
    public required string PricingName { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [NotMapped]
    public virtual ICollection<PricingDetail> PricingDetails { get; set; } = [];
}
