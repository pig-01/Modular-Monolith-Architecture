using DataSource.Application.Abstractions;
using MediatR;

namespace DataSource.Application.Queries;

public record GetUserDataSourcesQuery(Guid UserId) : IRequest<IReadOnlyList<DataSourceDto>>;
