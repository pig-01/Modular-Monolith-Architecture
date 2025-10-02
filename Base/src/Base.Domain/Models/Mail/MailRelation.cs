namespace Base.Domain.Models.Mail;

public class MailRelation
{
    /// <summary>
    /// 租戶編號
    /// </summary>
    /// <value></value>
    public string? TenantId { get; set; }

    /// <summary>
    /// 產品編號
    /// </summary>
    /// <value></value>
    public string? PartyId { get; set; }
}

