using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Main.WebApi.Registers;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration) : IConfigureNamedOptions<SwaggerGenOptions>
{

    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);

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
                    if (description.IsDeprecated)
                    {
                        versionApiInfo.Description = "此API版本已被棄用";
                    }
                    options.SwaggerDoc(description.GroupName, openApiInfoSetting);
                }

                // 設定為 OpenAPI 3.1
                options.UseInlineDefinitionsForEnums();
                options.CustomSchemaIds(type => type.FullName);  // 避免衝突


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "JWT（JSON Web Token）使用 Bearer 方式的 Authorization Header。\r\n在下面的文字輸入框中輸入 'Bearer' [空格] 然後是您的 Token。\r\n範例：\"Bearer af249d7a2e1f\"",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // using System.Reflection;
                string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            }
        }

    }

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
                Url = new Uri(openApi.GetValue("Document:Contact:Url", defaultString))
            },
            License = new OpenApiLicense
            {
                Name = openApi.GetValue("Document:License:Name", defaultString),
                Url = new Uri(openApi.GetValue("Document:License:Url", defaultString))
            },
            Version = openApi.GetValue("Document:Version", defaultString)
        };

        return openApiInfo;
    }
}
