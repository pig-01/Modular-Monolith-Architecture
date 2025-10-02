using Main.WebApi.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Main.WebApi.Filters;

public class NotImplementedOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo
            .GetCustomAttributes(typeof(NotImplementedApiAttribute), false)
            .FirstOrDefault() is NotImplementedApiAttribute notImplementedAttribute)
        {
            operation.Summary = $"🚧 {notImplementedAttribute.Message} 🚧";
        }
    }
}
