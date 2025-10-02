using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Base.Domain.Exceptions;
using Main.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[PrimaryKey("Id", "PlanDocumentId", "DocumentId")]
public partial class PlanDocumentData : Entity, IAggregateRoot
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Key]
    [Column("PlanDocumentID")]
    public int PlanDocumentId { get; set; }

    [Key]
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
    public string? FieldId { get; set; }

    [StringLength(50)]
    public string? FieldName { get; set; }

    [StringLength(20)]
    public string? FieldType { get; set; }

    [StringLength(500)]
    public string? FieldValue { get; set; }

    public bool? Required { get; set; }

    public bool? ReadOnly { get; set; }

    public string? CustomName { get; set; }

    [ForeignKey("PlanDocumentId")]
    [InverseProperty("PlanDocumentDatas")]
    public virtual PlanDocument? PlanDocument { get; set; }

    /// <summary>
    /// 是否為拆分資料
    /// </summary>
    /// <value></value>
    [NotMapped]
    public bool IsSplited { get; set; }

    public PlanDocumentData() { }

    /// <summary>
    /// 建立表單資料
    /// </summary>
    /// <remarks>
    /// 只建立資料部分，其他欄位需另外使用 <see cref="BindData(PlanDocument, string, string)"/> 綁定
    /// </remarks>
    /// <param name="fieldId"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldType"></param>
    /// <param name="fieldValue"></param>
    /// <param name="customName"></param>
    /// <param name="companyName"></param>
    /// <param name="areaName"></param>
    /// <param name="required"></param>
    /// <param name="readOnly"></param>
    /// <param name="isSplited"></param>
    public PlanDocumentData(string fieldId, string fieldName, string fieldType, string fieldValue, string customName, string companyName, string areaName, bool? required, bool? readOnly, bool isSplited)
    {
        FieldId = fieldId;
        FieldName = fieldName;
        FieldType = fieldType;
        FieldValue = fieldValue;
        IsSplited = isSplited;
        Required = required;
        ReadOnly = readOnly;
        CustomName = customName;
        CompanyName = companyName;
        AreaName = areaName;
    }

    /// <summary>
    /// 綁定資料
    /// </summary>
    /// <remarks>
    /// 綁定表單資料到 <see cref="PlanDocument"/> 物件，表單資料需透過 <see cref="PlanDocumentData(string, string, string, string, string, bool?, bool?, bool)"/> 建構
    /// </remarks>
    /// <param name="planDocument"></param>
    /// <param name="createdUser"></param>
    /// <param name="modifiedUser"></param>
    public void BindData(PlanDocument planDocument, string createdUser, string modifiedUser)
    {
        DocumentId = planDocument.DocumentId ?? throw new InvalidOperationException("DocumentId cannot be null when creating PlanDocumentData.");
        StartDate = planDocument.StartDate;
        EndDate = planDocument.EndDate;
        TenantId = planDocument.PlanDetail?.Plan?.TenantId ?? throw new InvalidOperationException("TenantId cannot be null when creating PlanDocumentData.");
        CompanyId = planDocument.PlanDetail?.Plan?.CompanyId;
        CreatedUser = createdUser;
        ModifiedUser = modifiedUser;
    }

    public PlanDocumentData Archive(DateTime archiveDate, string archiveUser)
    {
        Archived = true;
        ModifiedDate = archiveDate;
        ModifiedUser = archiveUser;
        return this;
    }

    /// <summary>
    /// 拆分資料
    /// </summary>
    /// <param name="year"></param>
    /// <param name="createdUser"></param>
    /// <returns></returns>
    public List<PlanDocumentDataSplited> Split(PlanDocument planDocument, int year, string createdUser) => true switch
    {
        var _ when planDocument.IsSingleQuarter => SplitQuarterData(year, planDocument.Quarter!.Value, createdUser),
        var _ when planDocument.IsSingleYear => SplitYearData(year, createdUser),
        _ => throw new ParameterException("Invalid AssignPlanDetail"),
    };

    /// <summary>
    /// 拆分年度資料
    /// </summary>
    /// <param name="year"></param>
    /// <param name="createdUser"></param>
    /// <returns></returns>
    private List<PlanDocumentDataSplited> SplitYearData(int year, string createdUser)
    {
        List<PlanDocumentDataSplited> splitedDataList = [];
        decimal yearValue = decimal.TryParse(FieldValue, out decimal v) ? v : 0m;
        int totalDays = DateTime.IsLeapYear(year) ? 366 : 365;



        // 計算每個月的比例值，並確保加總等於原值
        decimal[] monthlyValues = new decimal[12];
        decimal remainingValue = yearValue;

        // 先計算前11個月的值（四捨五入到小數點4位）
        for (int month = 0; month < 11; month++)
        {
            int daysInCurrentMonth = DateTime.DaysInMonth(year, month + 1);
            decimal monthlyValue = yearValue * daysInCurrentMonth / totalDays;
            monthlyValues[month] = Math.Round(monthlyValue, 4, MidpointRounding.AwayFromZero);
            remainingValue -= monthlyValues[month];
            PlanDocumentDataSplited splitedData = new(
          "month", month + 1, this, monthlyValues[month].ToString("0.####", CultureInfo.InvariantCulture), createdUser);
            splitedData.BindData(this, createdUser);
            splitedDataList.Add(splitedData);
        }

        // 最後一個月使用剩餘值，確保加總等於原值
        monthlyValues[11] = remainingValue;
        PlanDocumentDataSplited lastSplitedData = new(
       "month", 12, this, monthlyValues[11].ToString("0.####", CultureInfo.InvariantCulture), createdUser);
        lastSplitedData.BindData(this, createdUser);
        splitedDataList.Add(lastSplitedData);

        return splitedDataList;
    }

    /// <summary>
    /// 拆分季度資料
    /// </summary>
    /// <param name="year"></param>
    /// <param name="quarter"></param>
    /// <param name="createdUser"></param>
    /// <returns></returns>
    private List<PlanDocumentDataSplited> SplitQuarterData(int year, int quarter, string createdUser)
    {
        List<PlanDocumentDataSplited> splitedDataList = [];
        decimal quarterValue = decimal.TryParse(FieldValue, out decimal v) ? v : 0m;

        // 取得該季的月份
        int startMonth = (quarter - 1) * 3 + 1;
        int[] quarterMonths = [startMonth, startMonth + 1, startMonth + 2];
        int totalDaysInQuarter = quarterMonths.Sum(month => DateTime.DaysInMonth(year, month));

        // 計算每個月的比例值，並確保加總等於原值
        decimal[] monthlyValues = new decimal[3];
        decimal remainingValue = quarterValue;

        // 先計算前11個月的值（四捨五入到小數點4位）
        for (int i = 0; i < 2; i++)
        {
            int daysInCurrentMonth = DateTime.DaysInMonth(year, quarterMonths[i]);
            decimal monthlyValue = quarterValue * daysInCurrentMonth / totalDaysInQuarter;
            monthlyValues[i] = Math.Round(monthlyValue, 4, MidpointRounding.AwayFromZero);
            remainingValue -= monthlyValues[i];
            PlanDocumentDataSplited splitedData = new(
              "month", quarterMonths[i], this, monthlyValue.ToString("0.####", CultureInfo.InvariantCulture), createdUser);
            splitedData.BindData(this, createdUser);
            splitedDataList.Add(splitedData);
        }

        // 最後一個月使用剩餘值，確保加總等於原值
        monthlyValues[2] = remainingValue;
        PlanDocumentDataSplited lastSplitedData = new(
               "month", quarterMonths[2], this, monthlyValues[2].ToString("0.####", CultureInfo.InvariantCulture), createdUser);
        lastSplitedData.BindData(this, createdUser);
        splitedDataList.Add(lastSplitedData);


        return splitedDataList;
    }
}
