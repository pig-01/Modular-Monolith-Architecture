using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Main.WebApi.Conventions;

public class CustomControllerNameConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        // 自訂控制器名稱解析邏輯
        string originalName = controller.ControllerType.Name; // 預設名稱
        if (originalName.EndsWith("Controller"))
        {
            controller.ControllerName = originalName.Replace("Controller", "");
        }
    }
}
