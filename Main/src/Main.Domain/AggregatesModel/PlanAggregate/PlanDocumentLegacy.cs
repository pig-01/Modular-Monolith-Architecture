using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanDocumentLegacy")]
public partial class PlanDocumentLegacy : Entity
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanDocumentID")]
    public int PlanDocumentId { get; set; }

    [Column("PlanDetailID")]
    public int PlanDetailId { get; set; }

    [Column("DocumentID")]
    public int? DocumentId { get; set; }

    [StringLength(50)]
    public string? Responsible { get; set; }

    [StringLength(50)]
    public string? Approve { get; set; }

    [StringLength(3)]
    public string? FormStatus { get; set; }

    [Column("PlanFactoryID")]
    public int PlanFactoryId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public int? Year { get; set; }

    public int? Quarter { get; set; }

    public int? Month { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ArchivedDate { get; set; }

    [StringLength(50)]
    public string? ArchivedUser { get; set; }

    public static PlanDocumentLegacy FromPlanDocument(PlanDocument planDocument) => new()
    {
        PlanDocumentId = planDocument.PlanDocumentId,
        PlanDetailId = planDocument.PlanDetailId,
        DocumentId = planDocument.DocumentId,
        Responsible = planDocument.Responsible,
        Approve = planDocument.Approve,
        FormStatus = planDocument.FormStatus,
        StartDate = planDocument.StartDate,
        EndDate = planDocument.EndDate,
        Year = planDocument.Year,
        Quarter = planDocument.Quarter,
        Month = planDocument.Month,
        PlanFactoryId = planDocument.PlanFactoryId
    };
}