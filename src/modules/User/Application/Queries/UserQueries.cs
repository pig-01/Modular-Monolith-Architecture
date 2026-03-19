using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Infrastructure;

namespace User.Application.Queries;

public record UserDto(Guid Id, string Name, string Email);
public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;

internal sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly UserDbContext _dbContext;

    public GetUsersQueryHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Select(u => new UserDto(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);
    }
}

internal sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly UserDbContext _dbContext;

    public GetUserByIdQueryHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDto(u.Id, u.Name, u.Email))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
