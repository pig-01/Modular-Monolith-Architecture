using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Base.Domain.Exceptions;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Domain.Events.PlanAggregate;
using Main.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("Plan")]
public partial class Plan : Entity, IAggregateRoot
{
    [Key]
    [Column("PlanID")]
    public int PlanId { get; set; }

    [Required]
    [StringLength(200)]
    public required string PlanName { get; set; }

    [Required]
    [Column("CompanyID")]
    public required long CompanyId { get; set; }

    [Required]
    [Column("TenantID")]
    [StringLength(10)]
    public required string TenantId { get; set; }

    [StringLength(4)]
    [Unicode(false)]
    public required string Year { get; set; }

    public bool Show { get; set; } = true;

    public bool Archived { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ArchivedDate { get; set; }

    [StringLength(50)]
    public string? ArchivedUser { get; set; }

    [StringLength(30)]
    public string? PlanTemplateVersion { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<PlanDetail> PlanDetails { get; set; } = [];

    [InverseProperty("Plan")]
    public virtual ICollection<PlanFactory> PlanFactories { get; set; } = [];

    [InverseProperty("Plan")]
    public virtual ICollection<PlanIndicator> PlanIndicators { get; set; } = [];

    [InverseProperty("Plan")]
    public virtual ICollection<PlanIndustry> PlanIndustries { get; set; } = [];

    /// <summary>
    /// 建立新的指標計畫
    /// </summary>
    /// <param name="planName"></param>
    /// <param name="tenantId"></param>
    /// <param name="companyId"></param>
    /// <param name="year"></param>
    /// <param name="createdDate"></param>
    /// <param name="createdUser"></param>
    /// <returns></returns>
    public static Plan Create(string planName, string tenantId, long companyId, int year, string? version, DateTime createdDate, string createdUser) => new()
    {
        PlanName = planName,
        CompanyId = companyId,
        TenantId = tenantId,
        Year = year.ToString(CultureInfo.InvariantCulture),
        Show = true,
        PlanTemplateVersion = version,
        CreatedDate = createdDate,
        CreatedUser = createdUser
    };

    /// <summary>
    /// 隱藏指標計畫
    /// </summary>
    /// <param name="modifiedDate"></param>
    /// <param name="modifiedUser"></param>
    public void HideToggle(bool isShow, DateTime modifiedDate, string modifiedUser)
    {
        Show = !isShow;
        ModifiedDate = modifiedDate;
        ModifiedUser = modifiedUser;
    }

    /// <summary>
    /// 封存指標計畫
    /// </summary>
    /// <param name="archiveDate">封存時間</param>
    /// <param name="archiveUser">封存人員</param>
    /// <returns></returns>
    public void Archive(DateTime archiveDate, string archiveUser)
    {
        AddDomainEvent(new PlanArchivedDomainEvent(this, archiveDate, archiveUser));
        Archived = true;
        ArchivedDate = archiveDate;
        ArchivedUser = archiveUser;
    }

    /// <summary>
    /// 指標計畫表單指派事件
    /// </summary>
    /// <param name="planDetails"></param>
    /// <param name="responsible"></param>
    /// <param name="modifiedUser"></param>
    public void Assign(string planName, Scuser responsible, Scuser modifiedUser, IEnumerable<PlanDocument> planDocuments)
    {
        List<PlanDetail> planDetails = [];

        IEnumerable<PlanDetail> existPlanDetails = PlanDetails.Select(x => new PlanDetail
        {
            PlanDetailId = x.PlanDetailId,
            PlanDetailName = x.PlanDetailName,
            PlanId = x.PlanId,
            Plan = new Plan()
            {
                PlanName = x.Plan.PlanName,
                PlanId = x.Plan.PlanId,
                CompanyId = x.Plan.CompanyId,
                Year = x.Plan.Year,
                TenantId = x.Plan.TenantId
            },
            RowNumber = x.RowNumber,
            GroupId = x.GroupId,
            CycleType = x.CycleType,
            CycleMonth = x.CycleMonth,
            CycleDay = x.CycleDay,
            CycleMonthLast = x.CycleMonthLast,
            EndDate = x.EndDate,
            PlanDocuments = new HashSet<PlanDocument>()
        });

        foreach (PlanDocument document in planDocuments)
        {
            PlanDetail planDetail = existPlanDetails.FirstOrDefault(x => x.PlanDetailId == document.PlanDetailId) ?? throw new NotFoundException("PlanDetail not found for the given PlanDocument.");

            PlanDetail? exist = planDetails.FirstOrDefault(x => x.PlanDetailId == planDetail.PlanDetailId);
            if (exist is null)
            {
                planDetail.PlanDocuments.Add(document);
                planDetails.Add(planDetail);
            }
            else
            {
                if (planDetails.Remove(exist))
                {
                    // 如果已經存在，則更新該明細的文件
                    exist.PlanDocuments.Add(document);
                    planDetails.Add(exist);
                }
            }
        }

        AddDomainEvent(new PlanDocumentAssignedDomainEvent(planName, responsible, modifiedUser, planDetails));
    }
}
