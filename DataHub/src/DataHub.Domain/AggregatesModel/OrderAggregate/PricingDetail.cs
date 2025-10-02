using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.OrderAggregate;

[Table("PricingDetail")]
[Index("PricingCode", "PricingDetailCode", Name = "UQ_PricingDetail", IsUnique = true)]
public partial class PricingDetail
{
    [Key]
    public long PricingDetailId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PricingCode { get; set; }

    [StringLength(50)]
    public string? PricingDetailCode { get; set; }

    [StringLength(50)]
    public string? PricingDetailValue { get; set; }
}
