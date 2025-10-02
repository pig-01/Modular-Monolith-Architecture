using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.CustomerAggregate;

[Table("CustomerSCUserRelation")]
[Index("UserId", Name = "UQ_CustomerSCUserRelation_UserID", IsUnique = true)]
public partial class CustomerScuserRelation
{
    [Key]
    [Column("CustomerSCUserRelationId")]
    public long CustomerScuserRelationId { get; set; }

    [StringLength(36)]
    [Unicode(false)]
    public string CustomerId { get; set; } = null!;

    [Column("UserID")]
    [StringLength(50)]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CustomerScuserRelation")]
    public virtual Scuser User { get; set; } = null!;
}
