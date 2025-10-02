using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Models.Mail;

public class DocumentAssignNotificationModel(string title, string planName, Scuser assignUser, Scuser responsibleUser, IEnumerable<AssignItem> assignItems)
{
    public string? Title { get; set; } = title;

    public string? PlanName { get; set; } = planName;

    public Scuser AssignUser { get; set; } = assignUser;

    public Scuser ResponsibleUser { get; set; } = responsibleUser;

    public IEnumerable<AssignItem> AssignItems { get; set; } = assignItems;
}
