using MediatR;

namespace DataSource.Application.Commands;

public record DeleteDataSourceCommand(Guid DataSourceId, Guid UserId) : IRequest<bool>;
