namespace Scheduler.Domain.AggregateModel.MailAggregate;

[Table("MailSender")]
public partial class MailSender : Entity
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
}