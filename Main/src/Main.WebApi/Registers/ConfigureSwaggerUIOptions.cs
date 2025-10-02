using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Main.WebApi.Registers;

public class ConfigureSwaggerUIOptions() : IConfigureNamedOptions<SwaggerUIOptions>
{
    public void Configure(string? name, SwaggerUIOptions options) => Configure(options);
    public void Configure(SwaggerUIOptions options)
    {
        // UI進入點
        options.RoutePrefix = "swagger";

        // 顯示請求時間
        options.DisplayRequestDuration();

        // 啟動深度連結
        options.EnableDeepLinking();

        // 啟動篩選框
        options.EnableFilter();

        // 顯示模型而非範例
        //options.DefaultModelRendering(ModelRendering.Model);

        // 預設模型展開深度，0為不展開，預設為1
        options.DefaultModelsExpandDepth(-1);

        // 文件展開設定
        options.DocExpansion(DocExpansion.List);
    }
}
