namespace Main.Dto.ViewModel.PlanTemplate;

public class ViewPlanTemplateDetail
{
    public int PlanTemplateDetailId { get; set; }

    public int PlanTemplateId { get; set; }

    public string? ExposeIndustryIds { get; set; }

    public string? Title { get; set; }

    public int? SortSequence { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? CreatedUser { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedUser { get; set; }

    public string? ChTitle { get; set; }

    public string? EnTitle { get; set; }

    public string? JpTitle { get; set; }

    public string? I18nTitle { get; set; }

    public ICollection<ViewGriRule>? Rules { get; set; } = [];
}
