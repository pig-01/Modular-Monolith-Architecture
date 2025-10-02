using System.Globalization;
using Base.Infrastructure.Extension;

namespace Scheduler.Domain.AggregateModel.UserAggregate;

[Table("SCUser")]
public partial class Scuser : Entity, IAggregateRoot
{
    /// <summary>
    /// 使用者代號
    /// </summary>
    [Key]
    [Column("UserID")]
    [StringLength(50)]
    public required string UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Required]
    [StringLength(50)]
    public required string UserName { get; set; }

    /// <summary>
    /// 使用者密碼
    /// </summary>
    [Required]
    [StringLength(1024)]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// 使用者類別
    /// </summary>
    [Required]
    [StringLength(1)]
    [Unicode(false)]
    public required string UserType { get; set; }

    /// <summary>
    /// 使用者說明
    /// </summary>
    [StringLength(255)]
    public string? UserDesc { get; set; }

    /// <summary>
    /// 使用者電子郵件
    /// </summary>
    [StringLength(100)]
    public string? UserEmail { get; set; }

    /// <summary>
    /// 購買 Tenant 上限數
    /// </summary>
    public int MaxTenantNo { get; set; }

    /// <summary>
    /// 最近一次成功登入日期時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastSuccessfulLoginDate { get; set; }

    /// <summary>
    /// 最近一次失敗登入日期時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? LastFailedLoginDate { get; set; }

    /// <summary>
    /// 連續登入失敗次數
    /// </summary>
    public short RepeatedFailloginTimes { get; set; }

    /// <summary>
    /// 預設語系
    /// </summary>
    [StringLength(20)]
    [Unicode(false)]
    public string? Culture { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? UserTel { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string? UserTelArea { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? UserTelExt { get; set; }

    [InverseProperty("User")]
    public virtual required CurrentTenant CurrentTenant { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ScuserTenant> ScuserTenants { get; set; } = [];

    /// <summary>
    /// 是否為超級使用者
    /// </summary>
    public bool IsSuperUser => UserType == "S";

    public bool IsOverMaxTenantNo => MaxTenantNo > 0 && ScuserTenants.Count >= MaxTenantNo;

    [NotMapped]
    public CultureInfo? CurrentCultureInfo { get; set; }

    [NotMapped]
    public TimeZoneInfo? CurrentTimeZoneInfo { get; set; }

    /// <summary>
    /// 當前使用者語系
    /// </summary>
    public string CurrentCulture => Culture ?? "zh-CHT";

    /// <summary>
    /// 建立一個新的使用者實例
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    public static Scuser Create(string userName, string userEmail, string defaultTenantId, DateTime modifiedDate) => new()
    {
        UserId = userEmail,
        UserName = userName,
        UserEmail = userEmail,
        UserType = "1",
        PasswordHash = $"{userEmail}:{userEmail}".EncryptString(),
        Culture = "zh-CHT", //初始成員語系
        CreatedUser = "CASSystem",
        ModifiedUser = "CASSystem",
        CurrentTenant = CurrentTenant.Create(userEmail, defaultTenantId, modifiedDate)
    };
}
