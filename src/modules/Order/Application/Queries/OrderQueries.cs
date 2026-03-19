using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Infrastructure;

namespace Order.Application.Queries;

public record OrderItemDto(Guid ProductId, int Quantity, decimal Price);
public record OrderDto(Guid Id, Guid UserId, decimal Total, IReadOnlyList<OrderItemDto> Items);
public record GetOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;
public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;

internal sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly OrderDbContext _dbContext;

    public GetOrdersQueryHandler(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.Total,
                o.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.Price)).ToList()))
            .ToListAsync(cancellationToken);
    }
}

internal sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly OrderDbContext _dbContext;

    public GetOrderByIdQueryHandler(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.Id == request.Id)
            .Select(o => new OrderDto(
                o.Id,
                o.UserId,
                o.Total,
                o.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.Price)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
