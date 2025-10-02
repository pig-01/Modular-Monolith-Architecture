using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Main.WebApi.Registers;

public class SwaggerOptionsRegister(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"專案WebAPI {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "此API版本已被棄用"
                    : "「專案WebAPI」(以下簡稱本系統)WebAPI服務提供安全的身份驗證機制，以確保只有授權用戶能夠訪問敏感數據和功能。使用JWT（JSON Web Token）的Bearer方案，用戶需要在標頭中傳遞包含其授權令牌的 Authorization 標頭，格式為 \"Bearer [空格] 令牌\"。這有效確保了API請求的合法性，保障了報表平台的安全性，同時為開發者提供了方便的身份認證機制。",
                Contact = new OpenApiContact
                {
                    Name = "專案WebAPI",
                    Email = ""
                }
            });
        }

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
