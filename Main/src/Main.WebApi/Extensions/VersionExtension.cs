using Asp.Versioning;

namespace Main.WebApi.Extensions;

public static class VersionExtension
{
    private const int majorVersion = 1;

    public static void AddApiVersion(this WebApplicationBuilder builder) => builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(majorVersion);
        options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
}
