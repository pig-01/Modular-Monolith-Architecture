namespace Scheduler.Domain.AggregateModel.UserAggregate;

[Table("SCUserTenant")]
public partial class ScuserTenant : Entity
{
    /// <summary>
    /// 使用者ID
    /// </summary>
    [Key]
    [Column("UserTenantID")]
    public long UserTenantId { get; set; }

    /// <summary>
    /// 使用者代號
    /// </summary>
    [Column("UserID")]
    [StringLength(50)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 帳號是否有效
    /// null: 限制使用(等待確認中)，true: 有效，false: 無效
    /// </summary>
    public bool? IsEnable { get; set; }

    /// <summary>
    /// 密碼永遠有效
    /// </summary>
    public bool PasswordNeverExpire { get; set; }

    /// <summary>
    /// 需變更密碼
    /// </summary>
    public bool ChangingPassword { get; set; }

    /// <summary>
    /// Tenant代號
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    // [InverseProperty("UserTenant")]
    // public virtual ICollection<OrganizationScuser> OrganizationScusers { get; set; } = [];

    [InverseProperty("UserTenant")]
    public virtual ICollection<ScuserRole> ScuserRoles { get; set; } = [];

    [InverseProperty("UserTenant")]
    public virtual ICollection<ActivationInvitation> ActivationInvitations { get; set; } = [];

    // [ForeignKey("TenantId")]
    // [InverseProperty("ScuserTenants")]
    // public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ScuserTenants")]
    public virtual Scuser User { get; set; } = null!;


    /// <summary>
    /// 建立新的使用者站台關聯
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static ScuserTenant Create(string tenantId, Scuser user, DateTime? expiresAt = null)
    {
        ScuserTenant scuserTenant = new()
        {
            TenantId = tenantId,
            UserId = user.UserId,
            IsEnable = null, // 帳號是否有效，預設為null
            PasswordNeverExpire = false, // 密碼非永遠有效
            ChangingPassword = false // 需要變更密碼
        };

        // 如果有指定過期時間，則使用該時間，否則使用預設的過期時間
        if (expiresAt.HasValue)
        {
            // 建立啟動邀請
            // 注意：這裡的過期時間和邀請人需要根據實際情況進行設置
            // 這裡假設過期時間為 expiresAt，邀請人為 user.UserId
            ActivationInvitation activationInvitation = ActivationInvitation.Create(tenantId, scuserTenant.UserTenantId,
                user.UserId, expiresAt.Value, user.UserId);
            scuserTenant.ActivationInvitations.Add(activationInvitation);

            // 發出加入使用者事件
            // 注意：這裡的 TenantId 需要根據實際情況進行設置
            // scuserTenant.AddDomainEvent(new JoinUserDomainEvent(user, scuserTenant.TenantId, activationInvitation.ActivationToken));
        }
        else
        {
            // 如果沒有指定過期時間，則不建立啟動邀請
            scuserTenant.ActivationInvitations = [];
        }

        return scuserTenant;
    }

    public bool Activate(DateTime activatedAt, string modifiedUser)
    {
        if (IsEnable == true)
        {
            return false; // 已經啟用
        }

        IsEnable = true;
        ModifiedUser = modifiedUser;

        // 更新使用者的當前站台
        if (User is not null)
        {
            if (User.CurrentTenant is null)
            {
                User.CurrentTenant = CurrentTenant.Create(UserId, TenantId, activatedAt);
            }
            else
            {
                User.CurrentTenant = CurrentTenant.Create(UserId, TenantId, activatedAt);
            }
        }

        // 發出啟用事件
        // AddDomainEvent(new ActivateUserDomainEvent(this, activatedAt));

        return true;
    }
}
