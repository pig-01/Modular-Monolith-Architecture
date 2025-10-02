namespace Scheduler.Domain.AggregateModel.UserAggregate;

[PrimaryKey("UserId", "TenantId")]
[Table("CurrentTenant")]
[Index("UserId", Name = "UQ_CurrentTenant_UserID", IsUnique = true)]
public partial class CurrentTenant
{
    [Key]
    [Column("UserID")]
    [StringLength(50)]
    public string UserId { get; set; } = null!;

    [Key]
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    // [ForeignKey("TenantId")]
    // [InverseProperty("CurrentTenants")]
    // public virtual Tenant Tenant { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("CurrentTenant")]
    public virtual Scuser User { get; set; } = null!;

    /// <summary>
    /// 建立新的當前站台
    /// </summary>
    /// <param name="userId">使用者識別碼</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="modifiedDate">修改日期</param>
    /// <returns></returns>
    public static CurrentTenant Create(string userId, string tenantId, DateTime modifiedDate) => new()
    {
        UserId = userId,
        TenantId = tenantId,
        ModifiedDate = modifiedDate
    };
}
