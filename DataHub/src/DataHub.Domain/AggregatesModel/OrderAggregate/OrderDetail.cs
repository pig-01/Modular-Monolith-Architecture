using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.OrderAggregate;

[Table("OrderDetail")]
[Index("OrderId", "PricingDetailCode", Name = "UQ_OrderDetail_OrderId", IsUnique = true)]
public partial class OrderDetail
{
    [Key]
    public long OrderDetailId { get; set; }

    public long? OrderId { get; set; }

    [StringLength(50)]
    public string? PricingDetailCode { get; set; }

    [StringLength(50)]
    public string? PricingDetailValue { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public virtual Order? Order { get; set; }
}
