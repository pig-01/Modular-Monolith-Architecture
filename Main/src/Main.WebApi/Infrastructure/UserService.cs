using System.Globalization;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.Users;

namespace Main.WebApi.Infrastructure;

public class UserService(
    IHttpContextAccessor httpContextAccessor,
    IUserQuery userQuery) : IUserService<Scuser>
{
    public Scuser CurrentNow(CancellationToken cancellationToken = default) => Now(cancellationToken).GetAwaiter().GetResult();

    /// <summary>
    /// 取得當前使用者
    /// </summary>
    /// <remarks>
    /// 這個方法會從 HttpContext 中取得當前使用者的 ID、時區和文化資訊，並返回對應的 Scuser 實例。
    /// 如果 HttpContext 或使用者資訊不存在，則會拋出 <see cref="UnauthorizedAccessException"/>。
    /// 如果使用者不存在，則會拋出 <see cref="NotFoundException"/>.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Scuser> Now(CancellationToken cancellationToken = default)
    {
        string userId = httpContextAccessor.HttpContext?.User.Claims.GetNameId() ?? throw new UnauthorizedAccessException("無法取得當前登入使用者");
        string userName = httpContextAccessor.HttpContext?.User.Claims.GetSubject() ?? throw new UnauthorizedAccessException("無法取得當前登入使用者名稱");
        string timeZoneId = httpContextAccessor.HttpContext?.User.Claims.GetUserTimeZoneInfo() ?? "Asia/Taipei";
        string culture = httpContextAccessor.HttpContext?.User.Claims.GetUserCulture() ?? "zh-CHT";

        Scuser user = await userQuery.GetByIdWithCurrentTenantAsync(userId!, cancellationToken: cancellationToken) ?? throw new NotFoundException("使用者不存在");

        if (user.UserName != userName)
        {
            // 如果使用者名稱與當前上下文中的使用者名稱不一致，則更新使用者名稱
            user.UserName = userName;
        }

        // 設置使用者的時區和文化資訊
        user.CurrentCultureInfo = new CultureInfo(culture);
        user.CurrentTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return user;
    }
}
