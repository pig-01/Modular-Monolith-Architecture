using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Main.Infrastructure.Demo.Context;
using Main.WebApi.Application.Queries.Plans.Impl;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.WebApi.Test.Integration.TestBase;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Test.Integration.Application.Queries.Plans;

/// <summary>
/// PlanQuery 整合測試
/// 測試計畫查詢服務的資料存取與商業邏輯整合
/// 遵循 DDD 原則，測試聚合邊界內的查詢操作
/// </summary>
public class PlanQueryIntegrationTests(WebApplicationFactory<Program> factory) : IntegrationTestBase<Program>(factory)
{
    [Fact(DisplayName = "根據ID取得計畫 - 應返回正確的計畫資料")]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPlan()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        IPlanQuery planQuery = new PlanQuery(context);

        // 建立測試資料 - 不指定 PlanId，讓資料庫自動生成
        Plan testPlan = new()
        {
            PlanName = "測試計畫",
            Year = "2024",
            CompanyId = 123456,
            TenantId = "TEST001", // 修正為 10 字元以內
            Show = true,
            CreatedDate = DateTime.Now,
            CreatedUser = "testuser",
            ModifiedDate = DateTime.Now,
            ModifiedUser = "testuser"
        };

        context.Plans.Add(testPlan);
        await context.SaveChangesAsync();

        int generatedPlanId = testPlan.PlanId; // 取得資料庫自動生成的 ID

        // Act
        Plan? result = await planQuery.GetByIdAsync(generatedPlanId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(generatedPlanId, result.PlanId);
        Assert.Equal("測試計畫", result.PlanName);
        Assert.Equal("2024", result.Year);
        Assert.Equal(123456, result.CompanyId);
        Assert.Equal("TEST001", result.TenantId);
        Assert.True(result.Show);
    }

    [Fact(DisplayName = "根據ID取得計畫 - 不存在的ID應返回null")]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        IPlanQuery planQuery = new PlanQuery(context);

        // Act
        Plan? result = await planQuery.GetByIdAsync(99999);

        // Assert
        Assert.Null(result);
    }

    [Theory(DisplayName = "根據多個ID取得計畫列表")]
    [InlineData(2, 2)]
    [InlineData(1, 1)]
    public async Task GetByIdsAsync_WithValidIds_ShouldReturnMatchingPlans(int planCount, int expectedCount)
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        IPlanQuery planQuery = new PlanQuery(context);

        // 建立測試資料
        Plan[] testPlans = [];
        for (int i = 0; i < planCount; i++)
        {
            testPlans = [.. testPlans, new Plan
            {
                PlanName = $"測試計畫{i + 1}",
                Year = "2024",
                CompanyId = 123456,
                TenantId = "TEST002",
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            }];
        }

        context.Plans.AddRange(testPlans);
        await context.SaveChangesAsync();

        // 取得自動生成的 IDs
        int[] planIds = [.. testPlans.Take(expectedCount).Select(p => p.PlanId)];

        // Act
        IEnumerable<Plan> result = await planQuery.GetByIdsAsync(planIds);

        // Assert
        Assert.Equal(expectedCount, result.Count());
        if (expectedCount > 0)
        {
            Assert.All(result, plan => Assert.Contains(plan.PlanId, planIds));
        }
    }

    [Fact(DisplayName = "取得年份清單 - 應返回指定租戶的不重複年份列表")]
    public async Task GetYearListAsync_WithTenantId_ShouldReturnDistinctYears()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        IPlanQuery planQuery = new PlanQuery(context);
        string testTenantId = "TEST003";

        // 建立測試資料
        List<Plan> testPlans = new()
        {
            new Plan
            {
                PlanName = "2023年計畫",
                Year = "2023",
                CompanyId = 123456,
                TenantId = testTenantId,
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            },
            new Plan
            {
                PlanName = "2024年計畫1",
                Year = "2024",
                CompanyId = 123456,
                TenantId = testTenantId,
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            },
            new Plan
            {
                PlanName = "2024年計畫2",
                Year = "2024", // 重複年份
                CompanyId = 123456,
                TenantId = testTenantId,
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            },
            new Plan
            {
                PlanName = "其他租戶計畫",
                Year = "2025",
                CompanyId = 654321,
                TenantId = "OTHER001", // 不同租戶
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            }
        };

        context.Plans.AddRange(testPlans);
        await context.SaveChangesAsync();

        // Act
        IEnumerable<string> result = await planQuery.GetYearListAsync(testTenantId);

        // Assert
        Assert.Equal(2, result.Count()); // 只有 2023, 2024 兩個年份
        Assert.Contains("2023", result);
        Assert.Contains("2024", result);
        Assert.DoesNotContain("2025", result); // 不包含其他租戶的年份
        Assert.Equal(result.OrderBy(x => x), result); // 確保已排序
    }

    [Fact(DisplayName = "取得計畫列表 - 無條件查詢應返回所有計畫")]
    public async Task ListAsync_WithoutPredicate_ShouldReturnAllPlans()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        PlanQuery planQuery = new(context);

        // 建立測試資料
        List<Plan> testPlans = [
            new Plan
            {
                PlanName = "計畫A",
                Year = "2024",
                CompanyId = 123456,
                TenantId = "TEST004",
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            },
            new Plan
            {
                PlanName = "計畫B",
                Year = "2024",
                CompanyId = 123456,
                TenantId = "TEST004",
                Show = false,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            }
        ];

        context.Plans.AddRange(testPlans);
        await context.SaveChangesAsync();

        // Act
        IEnumerable<Plan> result = await planQuery.ListAsync();

        // Assert
        Assert.True(result.Count() >= 2); // 至少包含我們建立的兩筆資料
        Assert.Contains(result, p => p.PlanName == "計畫A");
        Assert.Contains(result, p => p.PlanName == "計畫B");
    }

    [Fact(DisplayName = "取得計畫列表 - 使用條件查詢應返回符合條件的計畫")]
    public async Task ListAsync_WithPredicate_ShouldReturnFilteredPlans()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        PlanQuery planQuery = new(context);

        // 建立測試資料
        List<Plan> testPlans = [
            new Plan
            {
                PlanName = "顯示計畫",
                Year = "2024",
                CompanyId = 123456,
                TenantId = "TEST005",
                Show = true,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            },
            new Plan
            {
                PlanName = "隱藏計畫",
                Year = "2024",
                CompanyId = 123456,
                TenantId = "TEST005",
                Show = false,
                CreatedDate = DateTime.Now,
                CreatedUser = "testuser",
                ModifiedDate = DateTime.Now,
                ModifiedUser = "testuser"
            }
        ];

        context.Plans.AddRange(testPlans);
        await context.SaveChangesAsync();

        // Act
        IEnumerable<Plan> result = await planQuery.ListAsync(p => p.Show && p.TenantId == "TEST005");

        // Assert
        Assert.Single(result); // 只有一筆符合條件
        Plan visiblePlan = result.First();
        Assert.Equal("顯示計畫", visiblePlan.PlanName);
        Assert.True(visiblePlan.Show);
    }

    [Fact(DisplayName = "記憶體使用效能測試 - 大量查詢操作不應造成記憶體洩漏")]
    public async Task MemoryUsageTestBulkQueriesShouldNotCauseMemoryLeak()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        PlanQuery planQuery = new(context);

        // 建立測試資料
        Plan[] testPlans = [.. Enumerable.Range(1, 10).Select(i => new Plan
        {
            PlanName = $"效能測試計畫{i}",
            Year = "2024",
            CompanyId = 123456,
            TenantId = "TEST006",
            Show = true,
            CreatedDate = DateTime.Now,
            CreatedUser = "testuser",
            ModifiedDate = DateTime.Now,
            ModifiedUser = "testuser"
        })];

        context.Plans.AddRange(testPlans);
        await context.SaveChangesAsync();

        // 取得生成的 IDs 進行效能測試
        int[] planIds = [.. testPlans.Select(p => p.PlanId)];

        // Act & Assert
        long initialMemory = GC.GetTotalMemory(true);

        // 執行多次查詢操作
        for (int i = 0; i < 10; i++)
        {
            Plan? _ = await planQuery.GetByIdAsync(planIds[i % planIds.Length]);
            IEnumerable<Plan> __ = await planQuery.ListAsync(p => p.TenantId == "TEST006");
        }

        long finalMemory = GC.GetTotalMemory(true);
        long memoryIncrease = finalMemory - initialMemory;

        // 記憶體增長應在合理範圍內 (小於 10MB)
        Assert.True(memoryIncrease < 10 * 1024 * 1024,
            $"記憶體使用量異常增長: {memoryIncrease / 1024 / 1024}MB");
    }

    [Fact(DisplayName = "併發存取測試 - 查詢操作的執行緒安全性")]
    public async Task ConcurrentAccessMultipleThreadsQueryingShouldMaintainConsistency()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        PlanQuery planQuery = new(context);

        // 建立測試資料
        Plan testPlan = new()
        {
            PlanName = "併發測試計畫",
            Year = "2024",
            CompanyId = 123456,
            TenantId = "TEST007",
            Show = true,
            CreatedDate = DateTime.Now,
            CreatedUser = "testuser",
            ModifiedDate = DateTime.Now,
            ModifiedUser = "testuser"
        };

        context.Plans.Add(testPlan);
        await context.SaveChangesAsync();

        int planId = testPlan.PlanId;

        // Act - 順序執行多個查詢以測試查詢邏輯的一致性
        List<Plan?> results = [];
        for (int i = 0; i < 5; i++)
        {
            Plan? result = await planQuery.GetByIdAsync(planId);
            results.Add(result);
        }

        // Assert
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.Equal(planId, result.PlanId);
            Assert.Equal("併發測試計畫", result.PlanName);
            Assert.Equal("TEST007", result.TenantId);
        });
    }

    [Fact(DisplayName = "取消令牌支援測試 - 查詢操作應正確回應取消請求")]
    public async Task CancellationTokenSupportQueryOperationShouldRespectCancellation()
    {
        // Arrange
        using TestServiceScope scope = CreateTestScope();
        DemoContext context = scope.ServiceProvider.GetRequiredService<DemoContext>();
        PlanQuery planQuery = new(context);

        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMilliseconds(1)); // 立即取消

        // Act & Assert - TaskCanceledException 是 OperationCanceledException 的子類
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await Task.Delay(10, cts.Token); // 確保取消令牌已觸發
            await planQuery.GetByIdAsync(999, cts.Token);
        });
    }
}