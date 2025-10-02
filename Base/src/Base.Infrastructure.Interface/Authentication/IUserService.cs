namespace Base.Infrastructure.Interface.Authentication;

public interface IUserService<TUser>
{
    Task<TUser> Now(CancellationToken cancellationToken = default);

    TUser CurrentNow(CancellationToken cancellationToken = default);
}
