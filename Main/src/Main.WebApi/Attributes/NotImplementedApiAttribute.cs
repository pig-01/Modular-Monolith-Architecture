namespace Main.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class NotImplementedApiAttribute(string message = "此 API 尚未開發") : Attribute
{
    public string Message { get; } = message;
}
