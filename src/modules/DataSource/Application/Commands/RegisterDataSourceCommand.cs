using DataSource.Application.Abstractions;
using DataSource.Domain.Enums;
using MediatR;

namespace DataSource.Application.Commands;

public record RegisterDataSourceCommand(
    Guid UserId,
    string Name,
    ProviderType Provider,
    string ConnectionString) : IRequest<DataSourceDto>;
