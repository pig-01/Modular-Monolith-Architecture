---
description: 'Guidelines for developing projects in C#'
applyTo: '**/*.cs'
---

# 開發最佳實踐

> Demo產品後端開發最佳實踐，包含程式碼撰寫規範、開發範例等

## 更新記錄

| 日期 | 作者 | 調整內容 | 版本 |
| ---------- | -------- | --------------------------------- | -------- |
| 2025.09.30 | Jason Tsai | 首版 | 1.0.0 |

## 開發通則

### 基本原則

- 使用 C# 12 語法特性
- 遵循 SOLID 原則
- 遵循 Clean Code 原則
- 遵循 DDD (Domain-Driven Design) 原則

### 程式碼品質

- 使用依賴注入 (Dependency Injection) 來管理服務和資源
- 使用異步編程 (async/await) 來提升應用程式的響應性
- 使用日誌記錄 (Logging) 來追蹤應用程式的行為和錯誤
- 使用單元測試 (Unit Testing) 來確保程式碼的正確性和穩定性
- 使用版本控制 (Version Control) 來管理程式碼的變更和協作
- 使用代碼審查 (Code Review) 來提升程式碼的品質和一致性
- 使用持續整合 (CI) 和持續部署 (CD) 來自動化測試和部署流程
- 使用安全最佳實踐 (Security Best Practices) 來保護應用程式和資料的安全
- 使用效能優化 (Performance Optimization) 來提升應用程式的效能和可擴展性
- 使用文件化 (Documentation) 來記錄程式碼的設計和使用

## :point_up: 基本開發指令範例

主要定義在開發中常用的設計模式、架構範例等，以便於團隊成員參考和使用。

### :point_right: 建立命令(Command)和命令處理器(Command Handler)

依照提供的商業情境開發商業邏輯命令，並使用 MediatR 庫來實現命令和命令處理器的模式。

#### 使用情境

- 處理單一交易
- 處理資料存取或異動(Insert or Update or Delete)

#### :file_folder: 檔案內容

- 命令類別：`Application\Commands\`
- 命令處理器類別：`Application\Commands\`

#### :abc: 命名空間

- 命令類別：`Main.Application.Commands`
- 命令處理器類別：`Main.Application.Commands`

#### :1234: 執行步驟

1. 建立命令類別和命令處理器類別
2. 定義命令類別，包含必要的屬性和驗證
3. 定義命令處理器類別，實現命令的處理邏輯

####  範例程式碼

* *命令類別：`CreatePlanCommand.cs`*

```csharp
// 定義命令類別 CreatePlanCommand.cs
public class CreatePlanCommand : IRequest<int>
{
    /// <summary>
    /// 計劃名稱
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [JsonPropertyName("planName")]
    [DefaultValue("2025 凡奈斯金融保險永續指標")]
    public required string PlanName { get; set; }

    /// <summary>
    /// 計劃年度
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("year")]
    [DefaultValue(2025)]
    public int? Year { get; set; }
}
```

* *命令處理器類別：`CreatePlanCommandHandler.cs`*

```csharp
// 定義命令處理器類別 CreatePlanCommandHandler.cs
public class CreatePlanCommandHandler(ILogger<CreatePlanCommandHandler> logger) : IRequestHandler<CreatePlanCommand, int>
{
    [Authorize(Policy = "User")]
    public async Task<int> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        // Implementation logic here ...
        return plan.PlanId;
    }
}
```

#### 備註

- 使用 `required` 關鍵字來強制要求屬性在物件初始化時被設定
- 使用 `JsonIgnore` 和 `JsonPropertyName` 屬性來控制 JSON 序列化和反序列化的行為
- 使用 `DefaultValue` 屬性來指定屬性的預設值
- 使用 `ILogger` 來記錄日誌
- 使用 `IRequest` 和 `IRequestHandler` 介面來定義命令和命令處理器
- 使用 `Authorize` 屬性來保護命令處理器，確保只有授權的使用者可以執行命令
- 使用 `CancellationToken` 來支援取消操作
- 使用 XML 註解來描述類別和屬性的用途


### :point_right: 建立查詢(Query)

使用介面和實作類別來實現查詢的模式。

#### 檔案位置
#### 命名空間
#### 執行步驟
#### 範例程式碼


## 參考資源

- [C# 官方文件](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [ASP.NET Core 官方文件](https://learn.microsoft.com/en-us/aspnet/core/)
- [MediatR 官方文件](https://learn.microsoft.com/en-us/aspnet/core/)
