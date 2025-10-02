using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Models.Mail;

public class UserInviteModel(string startUri, Uri serverUri, DateTime expirationDate, Scuser inviteUser)
{
    public string StartUri { get; set; } = startUri;

    public Uri ServerUri { get; set; } = serverUri;

    public DateTime ExpirationDate { get; set; } = expirationDate;

    public Scuser InviteUser { get; set; } = inviteUser;
}
