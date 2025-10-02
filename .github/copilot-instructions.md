# 後端系統 AI 開發指南

> Demo產品後端主要解決方案，採用 .NET 9.0 開發，支援 DDD 與 3-tier 架構模式

## 核心架構理解

### 解決方案層次結構
```
BackEnd.sln (主解決方案)
├── Main.sln (主要業務功能)
├── Base.sln (共享基礎設施)
├── DataHub.sln (資料中心)
└── Aspire (容器編排與開發環境)
```

### 關鍵架構模式

**三層架構 (3-Tier)**: 
- **WebApi** → **Service + Service.Interface** → **Repository (Dao)** → **Infrastructure (DbContext)**
- Services 使用構造函數注入，所有依賴注入透過 `Autofac` 管理
- DAO 模式: `ILoginDao` → `LoginDao` 位於 `Repository/Dao/{Domain}/Impl/`

**模組化架構**: 每個業務域有獨立的資料夾結構
- Domain 聚合根在 `Domain/AggregatesModel/{EntityName}Aggregate/`
- 實體配置使用 `DemoContext.{Domain}.cs` 部分類別

## 必須遵循的開發約定

### Dependency Injection 註冊模式
```csharp
// 位於 Main.WebApi/Registers/
ServiceRegisterModule    // Service 結尾類別，須符合 Namespace 格式
DaoRegisterModule       // Dao 結尾類別，須在 .Dao.{Domain}.Impl 命名空間
```

### 命名約定
- **Services**: `{Domain}Service : ServiceBase<{Domain}Service>, I{Domain}Service`
- **DAOs**: `{Domain}Dao : BaseDao, I{Domain}Dao`
- **Controllers**: `{Domain}Controller : ControllerBase`
- **測試**: `{ClassName}Tests` 配合 `MethodName_Condition_ExpectedResult()` 模式

### 日誌記錄標準
```csharp
// 使用 Serilog 與結構化日誌
logger.LogInformation("User {UserId} ({UserName}) has logged in.", userId, userName);
// 避免字串串接，使用插值參數
```

## 開發工作流程

### 建置任務 (使用 VS Code Tasks)
- `build-main-webapi`: 建置主要 WebAPI
- `build-datahub`: 建置 DataHub API  
- `build-all-solutions Debug/Release`: 完整解決方案建置
- `test-all`: 執行完整測試套件
- `watch-main-webapi`: 開發模式熱重載

### 啟動專案方式
1. **Aspire 整合環境**: 使用 `Aspire.AppHost` (包含前端 Yarn)
2. **單獨 API 開發**: 直接啟動 `Main.WebApi` 或 `DataHub`
3. **VS Code Launch**: 預設組態支援同時啟動多個服務

### 資料庫存取模式
- **Entity Framework Core**: 用於實體追蹤與複雜查詢
- **Dapper**: 用於高效能 SQL 查詢 (如 `LoginDao` 範例)
- **SqlBuilder**: 動態 SQL 建構 (位於 `Base.Infrastructure.Toolkits`)

## 測試策略

### 單元測試設定
- 專案命名: `{ProjectName}.Test`
- 測試框架: xUnit + NSubstitute (模擬)
- 組態檔: `appsettings.Test.json` 用於測試特定設定
- 位置: 各解決方案的 `test/` 資料夾

### 測試執行
```bash
dotnet test                           # 執行所有測試
dotnet test --filter "FullyQualifiedName~Login"  # 特定領域測試
```

## 重要整合點

### 認證與授權
- 支援 CAS、JWT Bearer、Cookie 多種認證方式
- 認證設定在 `AuthenticationExtension.cs`
- 使用者服務透過 `IUserService<Scuser>` 注入

### 跨專案通訊
- **Base** 提供共享基礎設施服務 (Mail, NPOI, Aspose, Security)
- **Main** 與 **DataHub** 透過介面契約溝通
- Mail 功能同時使用 `IMailRepository` (Base) 和 `MailAggregateDomain.IMailRepository` (Domain)

### 組態管理
系統使用多層組態結構，透過 `Extension.cs` 註冊:
```csharp
builder.Services.Configure<BizformOption>(bizformConfigurationSection);
builder.Services.Configure<CorsOption>(corsConfigurationSection);
```

重要指令: 
- 開發環境建置前須執行 `dotnet restore BackEnd.sln`
- 使用 `.editorconfig` 確保程式碼格式一致性
- 遵循 `CODING_RULES.md` 的 UTF-8-BOM 編碼與 4 空格縮排規則