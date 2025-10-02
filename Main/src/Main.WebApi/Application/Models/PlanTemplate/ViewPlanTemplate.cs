namespace Main.Dto.ViewModel.PlanTemplate;

public class ViewPlanTemplate
{
    public int PlanTemplateId { get; set; }

    public string? IndicatorId { get; set; }

    public required string PlanTemplateName { get; set; }

    public string? PlanTemplateChName { get; set; }

    public string? PlanTemplateEnName { get; set; }

    public string? PlanTemplateJpName { get; set; }

    public string? I18nPlanTemplateName { get; set; }

    public int FormId { get; set; }

    public required string GroupId { get; set; }

    public string? I18nGroupName { get; set; }

    public int? RowNumber { get; set; }

    public int? SortSequence { get; set; }

    public string[] RequestUnits { get; set; } = [];

    public virtual ICollection<ViewPlanTemplateDetail> PlanTemplateDetails { get; set; } = [];
}
