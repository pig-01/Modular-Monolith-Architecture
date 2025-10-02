using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DataHub.Extensions;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration) : IConfigureNamedOptions<SwaggerGenOptions>
{

    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);

    /// <summary>
    /// Configures the Swagger generation options for the application.
    /// </summary>
    /// <param name="options">The <see cref="SwaggerGenOptions"/> to configure.</param>
    /// <remarks>
    /// This method retrieves OpenAPI configuration settings and dynamically adds Swagger documents
    /// for each API version discovered in the application. It also customizes schema generation
    /// by using inline definitions for enums and setting custom schema IDs.
    /// </remarks>
    public void Configure(SwaggerGenOptions options)
    {
        // Get swagger document information from configuration
        IConfigurationSection openApi = configuration.GetSection("OpenApi");

        if (!openApi.Exists())
        {
            return;
        }
        else
        {
            if (provider.ApiVersionDescriptions is not null && provider.ApiVersionDescriptions.Count is not 0)
            {
                // Get the OpenApi information from configuration
                OpenApiInfo openApiInfoSetting = GetOpenApiInfo(openApi);

                // Add a swagger document for each discovered API version
                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                {
                    OpenApiInfo versionApiInfo = openApiInfoSetting;
                    versionApiInfo.Version = $"{description.ApiVersion}";

                    options.SwaggerDoc(description.GroupName, openApiInfoSetting);
                }

                options.UseInlineDefinitionsForEnums();
                options.CustomSchemaIds(type => type.FullName);
            }
        }

    }

    /// <summary>
    /// Gets the OpenApi information from the configuration section.
    /// </summary>
    /// <param name="openApi"></param>
    /// <returns></returns>
    private static OpenApiInfo GetOpenApiInfo(IConfigurationSection openApi)
    {
        string defaultString = "Default String";

        OpenApiInfo openApiInfo = new()
        {
            Title = openApi.GetValue("Document:Title", defaultString),
            Description = openApi.GetValue("Document:Description", defaultString),
            Contact = new OpenApiContact
            {
                Name = openApi.GetValue("Document:Contact:Name", defaultString),
                Email = openApi.GetValue("Document:Contact:Email", defaultString),
                Url = new Uri(openApi.GetValue("Document:Contact:Url", defaultString) ?? defaultString)
            },
            License = new OpenApiLicense
            {
                Name = openApi.GetValue("Document:License:Name", defaultString),
                Url = new Uri(openApi.GetValue("Document:License:Url", defaultString) ?? defaultString)
            },
            Version = openApi.GetValue("Document:Version", defaultString)
        };

        return openApiInfo;
    }
}
