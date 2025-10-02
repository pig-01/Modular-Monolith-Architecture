using Base.Domain.Models.Authentication;
using Microsoft.AspNetCore.Http;

namespace Base.Domain.Options.Authentication;

public partial class CASSetting()
{
    public int ProtocolVersion { get; set; } = 2;
    public string ServerUrlBase { get; set; } = "https://member.vikosmos.com/cas";
    public bool? ServerToken { get; set; }
    public string DefaultTimeZoneId { get; set; } = "UTC";
    public string DefaultCulture { get; set; } = "zh-CHT";
    public string ClientSecret { get; set; } = "";
    public string ClientId { get; set; } = "";
    public Func<HttpContext, User, Task<User>>? OnCreatingTicketConvertToSCUser { get; set; }
}
