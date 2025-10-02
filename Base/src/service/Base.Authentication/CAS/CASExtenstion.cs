using System.Security.Claims;
using Demo.Authentication.CAS;
using Demo.Authentication.CAS.AspNetCore;
using Demo.Authentication.CAS.Security;
using Base.Domain.Models.Authentication;
using Base.Domain.Options.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Base.Authentication.CAS;

public static class CASExtenstion
{
    public static AuthenticationBuilder AddCookie(this IServiceCollection services, CASSetting setting) => services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie()
            .AddCAS("CAS", options =>
            {
                options.CasServerUrlBase = setting.ServerUrlBase ?? throw new ArgumentNullException(nameof(setting.ServerUrlBase));
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;
                options.Events.OnCreatingTicket = async context =>
                {
                    ILogger<CasAuthenticationHandler> logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<CasAuthenticationHandler>>();
                    logger.LogDebug("建立Ticket");

                    if (context.Identity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                    {
                        return;
                    }

                    Assertion assertion = context.Assertion;
                    User customerDto = new() { UserId = assertion.PrincipalName };
                    if (assertion.Attributes.TryGetValue("display_name", out Microsoft.Extensions.Primitives.StringValues displayName) && !string.IsNullOrWhiteSpace(displayName))
                    {
                        customerDto.UserName = displayName;
                    }

                    if (assertion.Attributes.TryGetValue("cn", out Microsoft.Extensions.Primitives.StringValues fullName) && !string.IsNullOrWhiteSpace(fullName))
                    {
                        customerDto.UserName = fullName;
                    }

                    if (assertion.Attributes.TryGetValue("email", out Microsoft.Extensions.Primitives.StringValues email) && !string.IsNullOrWhiteSpace(email))
                    {
                        customerDto.UserEmail = email;
                    }

                    User dto = await setting.OnCreatingTicketConvertToSCUser!(context.HttpContext, customerDto);

                    context.Identity.AddClaim(new Claim("auth_scheme", CasDefaults.AuthenticationType));
                    context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, dto.UserId));
                    context.Identity.AddClaim(new Claim(ClaimTypes.Name, dto.UserName!));
                    context.Identity.AddClaim(new Claim(ClaimTypes.Email, dto.UserEmail!));
                };
                options.Events.OnRemoteFailure = context =>
                {
                    Exception? failure = context.Failure;
                    if (!string.IsNullOrWhiteSpace(failure?.Message))
                    {
                        ILogger<CasAuthenticationHandler> logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<CasAuthenticationHandler>>();
                        logger.LogError(failure, "CAS認證失敗{Exception}", failure.Message);
                    }

                    context.Response.Redirect("/#/noRight");
                    context.HandleResponse();
                    return Task.CompletedTask;
                };
            });
}
