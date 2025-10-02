using System.ComponentModel;
using System.Reflection;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field == null) return value.ToString();
        DescriptionAttribute? attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute == null ? value.ToString() : attribute.Description;
    }
}
