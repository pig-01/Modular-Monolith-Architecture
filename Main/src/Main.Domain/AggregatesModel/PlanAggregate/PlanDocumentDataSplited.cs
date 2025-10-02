using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanDocumentDataSplited")]
public partial class PlanDocumentDataSplited : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanDocumentID")]
    public int PlanDocumentId { get; set; }

    [Column("DocumentID")]
    public int DocumentId { get; set; }

    [Column("CompanyID")]
    public long? CompanyId { get; set; }

    [StringLength(200)]
    public string? CompanyName { get; set; }

    [StringLength(200)]
    public string? AreaName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column("TenantID")]
    [StringLength(100)]
    public string TenantId { get; set; } = null!;

    public bool Archived { get; set; }

    [Required]
    [Column("FieldID")]
    [StringLength(50)]
    public string FieldId { get; set; } = null!;

    [StringLength(50)]
    public string? FieldName { get; set; }

    [StringLength(20)]
    public string? FieldType { get; set; }

    [StringLength(500)]
    public string? SplitValue { get; set; }

    public bool? Required { get; set; }

    public bool? ReadOnly { get; set; }

    public int? CycleNumber { get; set; }

    public string? CycleType { get; set; }

    public string? CustomName { get; set; }

    [ForeignKey("PlanDocumentId")]
    [InverseProperty("PlanDocumentDataSpliteds")]
    public virtual PlanDocument? PlanDocument { get; set; }

    public PlanDocumentDataSplited() { }

    /// <summary>
    /// 建立拆分資料
    /// </summary>
    /// <param name="cycleType">週期類型</param>
    /// <param name="cycleNumber">週期編號</param>
    /// <param name="planDocumentData">表單資料</param>
    /// <param name="createdUser">建立人員</param>
    public PlanDocumentDataSplited(string cycleType, int cycleNumber, PlanDocumentData planDocumentData, string createdUser)
    {
        CycleType = cycleType;
        CycleNumber = cycleNumber;
        FieldId = planDocumentData.FieldId ?? throw new InvalidOperationException("FieldId cannot be null when creating PlanDocumentDataSplited.");
        FieldName = planDocumentData.FieldName;
        FieldType = planDocumentData.FieldType;
        SplitValue = planDocumentData.FieldValue;
        CustomName = planDocumentData.CustomName;
        Required = planDocumentData.Required;
        ReadOnly = planDocumentData.ReadOnly;
        CreatedUser = createdUser;
        SetCreateMetadata(createdUser, createdUser);
    }

    /// <summary>
    /// 建立拆分資料
    /// </summary>
    /// <remarks>
    /// 建立拆分資料，欄位值使用傳入的 <paramref name="fieldValue"/> 參數
    /// </remarks>
    /// <param name="cycleType">週期類型</param>
    /// <param name="cycleNumber">週期編號</param>
    /// <param name="planDocumentData">表單資料</param>
    /// <param name="fieldValue">欄位值</param>
    /// <param name="createdUser">建立人員</param>
    public PlanDocumentDataSplited(string cycleType, int cycleNumber, PlanDocumentData planDocumentData, string fieldValue, string createdUser)
    {
        CycleType = cycleType;
        CycleNumber = cycleNumber;
        FieldId = planDocumentData.FieldId ?? throw new InvalidOperationException("FieldId cannot be null when creating PlanDocumentDataSplited.");
        FieldName = planDocumentData.FieldName;
        FieldType = planDocumentData.FieldType;
        SplitValue = fieldValue;
        CustomName = planDocumentData.CustomName;
        ReadOnly = planDocumentData.ReadOnly;
        Required = planDocumentData.Required;
        CreatedUser = createdUser;
        SetCreateMetadata(createdUser, createdUser);
    }

    /// <summary>
    /// 將當前資料封存
    /// </summary>
    /// <param name="archiveDate">封存日期</param>
    /// <param name="archiveUser">封存人員</param>
    /// <returns>已封存的資料</returns>
    public PlanDocumentDataSplited Archive(DateTime archiveDate, string archiveUser)
    {
        Archived = true;
        ModifiedDate = archiveDate;
        ModifiedUser = archiveUser;
        return this;
    }

    /// <summary>
    /// 綁定資料
    /// </summary>
    /// <param name="planDocumentData">指標計畫表單資料</param>
    /// <param name="createdUser">建立人員</param>
    /// <returns></returns>
    public void BindData(PlanDocumentData planDocumentData, string createdUser)
    {
        PlanDocumentId = planDocumentData.PlanDocumentId;
        DocumentId = planDocumentData.DocumentId;
        CompanyId = planDocumentData.CompanyId;
        CompanyName = planDocumentData.CompanyName;
        AreaName = planDocumentData.AreaName;
        StartDate = planDocumentData.StartDate;
        EndDate = planDocumentData.EndDate;
        TenantId = planDocumentData.TenantId;
        SetModifiedMetadata(createdUser);
    }
}
