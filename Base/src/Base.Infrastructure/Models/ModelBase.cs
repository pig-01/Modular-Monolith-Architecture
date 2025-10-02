using System.Reflection;
using System.Text.Json;
using Base.Infrastructure.Toolkits.Converter.Json;

namespace Base.Infrastructure.Models;

public class ModelBase<T>
{
    /// <summary>
    /// 實例方法，將物件轉換為 JSON 字串
    /// </summary>
    /// <returns></returns>
    public string ToJson() => JsonSerializer.Serialize(this, ConverterBase.Settings);
    //public string ToJson() => ModelBase<T>.ToJson((T)this);

    /// <summary>
    /// 反序列化：從 JSON 字串建立物件
    /// </summary>
    /// <param name="json">json字串</param>
    /// <returns></returns>
    public static T FromJson(string json) => JsonSerializer.Deserialize<T>(json, ConverterBase.Settings)!;
    /// <summary>
    /// 靜態方法：將物件轉換為 JSON 字串
    /// </summary>
    /// <param name="model">物件</param>
    /// <returns></returns>
    public static string ToJson(T model) => JsonSerializer.Serialize(model, ConverterBase.Settings);

    /// <summary>
    /// 複製物件
    /// </summary>
    /// <returns></returns>
    public T Clone() => ModelBase<T>.FromJson(ToJson());


    /// <summary>
    /// 將物件轉換為指定型別的物件
    /// </summary>
    /// <typeparam name="T1">起始模型</typeparam>
    /// <typeparam name="T2">目標模型</typeparam>
    /// <param name="model">起始模型資料</param>
    /// <returns></returns>
    public static T2 Converter<T1, T2>(T1 model) where T2 : class, new()
    {
        T2 result = new();
        PropertyInfo[] t1Properties = typeof(T1).GetProperties();
        PropertyInfo[] t2Properties = typeof(T2).GetProperties();

        foreach (PropertyInfo t1Property in t1Properties)
        {
            foreach (PropertyInfo t2Property in t2Properties)
            {
                if (t1Property.Name == t2Property.Name)
                {
                    if (t1Property.PropertyType == t2Property.PropertyType)
                    {
                        t2Property.SetValue(result, t1Property.GetValue(model));
                    }

                    // TODO: 進階型別轉換
                }
            }
        }

        return result;
    }
}
