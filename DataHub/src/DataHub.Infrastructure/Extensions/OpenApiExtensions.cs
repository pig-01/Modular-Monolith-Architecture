using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace DataHub.Infrastructure.Extensions;

public static partial class OpenApiExtensions
{
    public static void AddEnpointParameter(this OpenApiOperation operation, ParameterLocation parameterLocation, string headerName, string headerValue, string description = "", bool required = true, string? schemaType = null) => operation.Parameters.Add(new()
    {
        Name = headerName,// Header 名稱
        In = parameterLocation,// 指定 Header
        Required = required,// 是否為必填
        Description = description,// Header 描述
        Schema = new OpenApiSchema
        {
            Type = schemaType// 資料型別
        },
        Example = new OpenApiString(headerValue)// Header 範例
    });
}
