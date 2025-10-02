using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Main.WebApi.Test.Integration.TestBase;

/// <summary>
/// 整合測試基底類別
/// 遵循 專案專案架構約定，提供共用的測試基礎設施與工具方法
/// 支援 TransactionScope 自動回滾機制
/// </summary>
public class IntegrationTestBase<TProgram>(WebApplicationFactory<TProgram> factory)
    : IClassFixture<WebApplicationFactory<TProgram>> where TProgram : class
{
    /// <summary>
    /// 建立測試服務範圍，包含 TransactionScope 自動回滾
    /// 遵循 專案專案的 DI 容器管理約定
    /// </summary>
    protected TestServiceScope CreateTestScope() => new(factory.Services.CreateScope());

    /// <summary>
    /// 建立帶有自訂服務的測試範圍
    /// 支援替換 DI 容器中的服務，適用於 Mock 測試場景
    /// </summary>
    protected TestServiceScope CreateTestScopeWithServices(Action<IServiceCollection> configureServices)
    {
        WebApplicationFactory<TProgram> customFactory = factory.WithWebHostBuilder(builder => builder.ConfigureServices(configureServices));

        return new TestServiceScope(customFactory.Services.CreateScope());
    }
}

/// <summary>
/// 測試服務範圍，自動處理 TransactionScope 與 ServiceScope
/// 確保測試資料的自動回滾與資源正確釋放
/// </summary>
public class TestServiceScope(IServiceScope serviceScope) : IDisposable
{
    private readonly TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);

    public IServiceProvider ServiceProvider => serviceScope.ServiceProvider;

    public void Dispose()
    {
        serviceScope?.Dispose();
        transactionScope.Dispose(); // 自動回滾所有資料庫變更
        GC.SuppressFinalize(this);
    }
}