using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Infrastructure;

namespace Product.Application.Queries;

public record ProductDto(Guid Id, string Name, decimal Price);
public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

internal sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly ProductDbContext _dbContext;

    public GetProductsQueryHandler(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .ToListAsync(cancellationToken);
    }
}

internal sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly ProductDbContext _dbContext;

    public GetProductByIdQueryHandler(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
