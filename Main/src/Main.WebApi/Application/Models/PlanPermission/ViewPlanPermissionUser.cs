namespace Main.Dto.ViewModel.PlanPermission;

public class ViewPlanPermissionRelatedItem
{
    public int PlanPermissionRelatedItemId { get; set; }

    public int PlanPermissionId { get; set; }
    public required string RelatedType { get; set; }
    public required string RelatedId { get; set; }

    public required string DisplayRelatedName { get; set; }
}
