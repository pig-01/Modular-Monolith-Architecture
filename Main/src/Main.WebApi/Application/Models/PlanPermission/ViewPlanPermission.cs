namespace Main.Dto.ViewModel.PlanPermission;

public class ViewPlanPermission
{
    public int PlanPermissionId { get; set; }

    public int? PlanId { get; set; }

    public required string Type { get; set; }

    public bool IsAll { get; set; }

    public bool OnlyCreated { get; set; }

    public bool OnlyResponseible { get; set; }

    public bool OnlyApprove { get; set; }

    public bool IsManager { get; set; }

    public ViewPlanPermissionRelatedItem[]? PlanPermissionRelated { get; set; }
}
