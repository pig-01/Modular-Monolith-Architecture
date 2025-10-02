using System.Net.Mail;

namespace Base.Domain.Models.Mail;

/// <summary>
/// 發信資訊
/// </summary>
public partial class MailInfomation
{
    /// <summary>
    /// 寄件者
    /// </summary>
    /// <value></value>
    public MailAddress? Sender { get; set; }

    /// <summary>
    /// 收件者清單
    /// </summary>
    /// <value></value>
    public List<MailAddress> ReceiverList { get; set; } = [];

    /// <summary>
    /// 副本清單
    /// </summary>
    /// <value></value>
    public List<MailAddress> CarbonCopyList { get; set; } = [];

    /// <summary>
    /// 密件副本清單
    /// </summary>
    /// <value></value>
    public List<MailAddress> BlindCarbonCopyList { get; set; } = [];

    /// <summary>
    /// 信件範本
    /// </summary>
    /// <value></value>
    public MailTemplate Template { get; set; } = new MailTemplate();

    /// <summary>
    /// 語系
    /// </summary>
    /// <value></value>
    public string Culture { get; set; } = "zh-CHT";

    /// <summary>
    /// 信件關聯
    /// </summary>
    /// <value></value>
    public MailRelation Relation { get; set; } = new MailRelation();

    /// <summary>
    /// 附件清單
    /// </summary>
    /// <value></value>
    public IList<MailAttachment> AttachmentList { get; set; } = [];


    /// <summary>
    /// 主旨
    /// </summary>
    /// <value></value>
    public string? Subject { get; set; }

    /// <summary>
    /// 內文
    /// </summary>
    /// <value></value>
    public string? Body { get; set; }


    /// <summary>
    /// 是否為HTML格式
    /// </summary>
    /// <value></value>
    public bool IsBodyHtml { get; set; } = true;

}
