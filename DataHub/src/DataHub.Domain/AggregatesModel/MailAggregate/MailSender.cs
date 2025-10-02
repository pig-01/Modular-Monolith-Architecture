using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.MailAggregate;

[Table("MailSender")]
public partial class MailSender
{
    [Key]
    [Column("MailSenderID")]
    public int MailSenderId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? SenderType { get; set; }

    [Column("SenderID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? SenderId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SenderName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MailAddress { get; set; }

    [Column("TenantID")]
    [StringLength(30)]
    [Unicode(false)]
    public string? TenantId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [StringLength(50)]
    public string CreatedUser { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(50)]
    public string? ModifiedUser { get; set; }
}
