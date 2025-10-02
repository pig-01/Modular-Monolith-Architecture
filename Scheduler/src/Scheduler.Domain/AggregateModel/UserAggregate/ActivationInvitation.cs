namespace Scheduler.Domain.AggregateModel.UserAggregate;

/// <summary>
/// 站台邀請啟動用的資料表，記錄邀請簽章與狀態
/// </summary>
[Table("ActivationInvitation")]
public partial class ActivationInvitation
{
    /// <summary>
    /// 邀請記錄識別碼
    /// </summary>
    [Key]
    [Column("InvitationID")]
    public long InvitationId { get; set; }

    /// <summary>
    /// 站台識別碼
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// 站台使用者識別碼
    /// </summary>
    [Column("UserTenantID")]
    public long UserTenantId { get; set; }

    /// <summary>
    /// 啟動簽章 (GUID)
    /// </summary>
    public Guid ActivationToken { get; set; }

    /// <summary>
    /// 邀請人
    /// </summary>
    [StringLength(50)]
    public string Inviter { get; set; } = null!;

    /// <summary>
    /// 有效期限
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 是否已啟用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 啟用時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ActivatedAt { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    [StringLength(50)]
    public string CreatedUser { get; set; } = null!;

    /// <summary>
    /// 修改日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 修改人員
    /// </summary>
    [StringLength(50)]
    public string ModifiedUser { get; set; } = null!;

    [ForeignKey("UserTenantId")]
    [InverseProperty("ActivationInvitations")]

    public virtual ScuserTenant UserTenant { get; set; } = null!;

    /// <summary>
    /// 建立一個新的啟動邀請記錄
    /// </summary>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="userTenantId">站台使用者識別碼</param>
    /// <param name="inviter">邀請人</param>
    /// <param name="expiresAt">有效期限</param>
    /// <param name="createdUser">建立人員</param>
    /// <returns></returns>
    public static ActivationInvitation Create(string tenantId, long userTenantId, string inviter, DateTime expiresAt, string createdUser) => new()
    {
        TenantId = tenantId,
        UserTenantId = userTenantId,
        ActivationToken = Guid.NewGuid(),
        Inviter = inviter,
        ExpiresAt = expiresAt,
        IsEnabled = true, // 預設為啟用狀態
        CreatedUser = createdUser
    };

    /// <summary>
    /// 啟用邀請記錄
    /// </summary>
    /// <param name="activationToken">啟動簽章 (GUID)</param>
    /// <param name="activatedAt">啟用時間</param>
    /// <param name="modifiedUser">修改人員</param>
    /// <returns></returns>
    public bool Activate(Guid activationToken, DateTime activatedAt, string modifiedUser)
    {
        if (ActivationToken != activationToken || !IsEnabled)
        {
            return false; // 無效的簽章或已經啟用
        }

        ActivatedAt = activatedAt;
        IsEnabled = false; // 啟用後設為不可用
        ModifiedUser = modifiedUser;

        return UserTenant.Activate(activatedAt, modifiedUser);
    }
}