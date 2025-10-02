using Microsoft.Extensions.Hosting;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class EnvironmentExtension
{
    public static bool IsDevelopment(string variable) => Environment.GetEnvironmentVariable(variable) == Environments.Development;
    public static bool IsStaging(string variable) => Environment.GetEnvironmentVariable(variable) == Environments.Staging;
    public static bool IsProduction(string variable) => Environment.GetEnvironmentVariable(variable) == Environments.Production;
}
