using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.CustomerAggregate;

[Table("CustomerOrdersRelation")]
[Index("CustomerId", "OrderId", Name = "UQ_CustomerOrdersRelation_CustomerId", IsUnique = true)]
public partial class CustomerOrdersRelation
{
    [Key]
    public long CustomerOrdersRelationId { get; set; }

    [StringLength(36)]
    [Unicode(false)]
    public string CustomerId { get; set; } = null!;

    public long OrderId { get; set; }
}
