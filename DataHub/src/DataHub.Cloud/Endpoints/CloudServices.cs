using DataHub.Cloud.Application.Queries;
using DataHub.Infrastructure.Options;
using MediatR;

namespace DataHub.Cloud.Endpoints;

public readonly struct CloudServices(
    ILogger<CloudServices> logger,
    HttpRequest httpRequest,
    IMediator mediator,
    IOptions<DataHubOptions> dataHubOptions,
    IOrderQuery orderQuery,
    HttpContext http)
{
    public ILogger<CloudServices> Logger { get; } = logger;
    public HttpRequest HttpRequest { get; } = httpRequest;
    public IOptions<DataHubOptions> DataHubOptions { get; } = dataHubOptions;
    public IOrderQuery OrderQuery { get; } = orderQuery;
    public IMediator Mediator { get; } = mediator;
    public HttpContext Http { get; } = http;
}
