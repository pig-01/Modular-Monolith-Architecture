using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Main.Domain.AggregatesModel.PlanAggregate;

[Table("PlanDocumentDataSplitConfig")]
[Index("PlanTemplateId", "FieldId", "FieldName", Name = "UK_PlanDocumentDataSplitConfig_PlanTemplateID_FieldID_FieldName", IsUnique = true)]
public partial class PlanDocumentDataSplitConfig
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PlanTemplateID")]
    public int PlanTemplateId { get; set; }

    [Column("FieldID")]
    [StringLength(50)]
    [Unicode(false)]
    public string FieldId { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }

    public bool? IsEnabled { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CreatedUser { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ModifiedUser { get; set; }

    [StringLength(255)]
    public string? FieldName { get; set; }
}