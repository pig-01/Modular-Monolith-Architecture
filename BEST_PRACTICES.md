# 最佳實踐案例

> 產品後端主要解決方案，定義產品所需的全部功能，依照產品定義開發

[TOC]

## 更新記錄

| 日期       | 作者        | 調整內容                     | 版本   |
| ---------- | ----------- | ---------------------------- | ------ |
| 2025.05.02 | Jason Tsai  | 首版                         | 1.0.0  |

## 日誌紀錄(Logging)

使用 Serilog 進行日誌紀錄，地端部署時使用 Console 和 File查詢與儲存，雲端部署時使用 Seq 作為日誌儲存與查詢的工具。

### 日誌記錄規則

1. 日誌紀錄的等級應該根據事件的嚴重性來選擇，通常使用 `Debug`、`Information`、`Warning` 和 `Error` 等級。
1. 日誌紀錄的內容應該簡潔明瞭，並包含足夠的上下文資訊，以便於後續的查詢與分析。
1. 日誌記錄統一使用插值字串，不要使用字串拼接的方式來記錄日誌。
1. 如有需求:
    1. 日誌記錄提供i18n功能，應該使用 `IStringLocalizer` 來進行多語系的日誌紀錄。
    1. 日誌記錄傳入結構資料，應該使用 `ILogger` 的擴展方法來進行結構化的日誌紀錄。

### 日誌紀錄範例

```csharp
public class TestController(ILogger<TestController> logger, IStringLocalizer<TestController> localizer) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // 使用 Serilog 記錄日誌基本範例
        logger.LogInformation("This is a test log message.");

        // 使用 Serilog 記錄日誌範例，並包含額外的上下文資訊和差值字串
        int userId = 123;
        string userName = "John Doe";
        logger.LogInformation("User {UserId} ({UserName}) has logged in.", userId, userName);

        // 使用 Serilog 記錄日誌範例，並包含 i18n 功能
        string localizedMessage = localizer["User {UserId} ({UserName}) has logged in."];
        logger.LogInformation(localizedMessage, userId, userName);

        // 使用 Serilog 記錄日誌範例，並包含結構化資料
        var user = new { Id = userId, Name = userName };
        logger.LogInformation("User {@User} has logged in.", user);

        return Ok();
    }
}
```

## 例外處理(Exception Handling)

在開發時，應該使用 `try-catch` 來捕捉例外，並在 `catch` 區塊中進行例外處理。例外處理的方式可以使用 `ILogger` 來記錄例外資訊，並回傳適當的 HTTP 狀態碼與錯誤訊息。

雲端部署後，會使用 `UseExceptionHandler` 來進行全域的例外處理，並回傳適當的 HTTP 狀態碼與錯誤訊息。

### 例外處理規則

1. 在 `try` 區塊中，應該只包含可能會拋出例外的程式碼，並避免在 `try` 區塊中進行過多的邏輯處理。
1. 在 `catch` 區塊中，應該只包含例外處理的程式碼，並避免在 `catch` 區塊中進行過多的邏輯處理。
1. 除了套件中提供的 `Exception` 類別外，可以使用自訂的例外類別來進行例外處理，並在 `catch` 區塊中進行適當的處理。
    1. 自訂的例外類別在 `Base.Domain.Exceptions` 命名空間中，並繼承自 `BaseException` 類別。
1. 撰寫過程中應使用例外控制策略，例如:
    1. `throw` 例外時，應該使用 `throw new Exception("Error message")` 的方式來拋出例外，而不是使用 `throw;` 的方式來重新拋出例外。
    1. 當判斷例外必須中斷程序時，應優先使用 `throw new XXXException("Error message")` 來拋出例外，而不是使用 `return` 或 `break` 的方式來中斷程式的執行。
    1. 如有明確的例外類別可供使用，在 `catch` 區塊中應該使用具體的例外類別來捕捉例外，而不是使用 `catch (Exception ex)` 的方式來捕捉所有的例外。

### 例外處理範例

```csharp
public class TestController(ILogger<TestController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            // 可能會拋出例外的程式碼
            throw new Exception("Test exception");
        }
        catch (Exception ex)
        {
            // 使用 Serilog 記錄例外資訊
            logger.LogError(ex, "An error occurred while processing the request.");
            // 回傳適當的 HTTP 狀態碼與錯誤訊息
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }

    [HttpGet]
    public IActionResult GetWithCustomException()
    {
        try
        {
            // 可能會拋出例外的程式碼
            throw new CustomException("Test custom exception");
        }
        catch (CustomException ex)
        {
            // 使用 Serilog 記錄例外資訊
            logger.LogError(ex, "An error occurred while processing the request.");
            // 回傳適當的 HTTP 狀態碼與錯誤訊息
            return StatusCode(500, "An error occurred while processing the request.");
        }
    }
}
```

## 資料庫交易

在開發資料庫交易中使用 UnitOfWork 中的 `BeginTransactionAsync` `CommitTransactionAsync` `RollbackTransactionAsync` 三種方法進行控制

另一種是使用 UnitOfWork 中的 `ExecuteAsync` 和其多載進行控制

```text
注意!!! 兩種方法不可以混用
```

### 資料庫交易規則

1. 在使用 `BeginTransactionAsync` 時，必須在 `CommitTransactionAsync` 或 `RollbackTransactionAsync` 之後才能結束交易。
2. 在使用 `ExecuteAsync` 時，在 `ExecuteAsync` 回傳結果後才能結束交易。

### 資料庫交易範例

```csharp
// 使用 IUnitOfWork 中的 BeginTransactionAsync, CommitTransactionAsync, RollbackTransactionAsync 來實作交易
public class TestService(IUnitOfWork unitOfWork)
{
    public Task TestTransactionAsync()
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            // handle unit of work

            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw ex;
        }
    }
}

// 使用 IUnitOfWork 中的 ExecuteAsync 來打包必須交易的程式邏輯
public class TestService(IUnitOfWork unitOfWork)
{
    public Task TestTransactionAsync()
    {
        // 泛型中必須帶入當前使用的Service(做紀錄使用)
        await unitOfWork.ExecuteAsync<TestService>(async () =>
        {
            // handle unit of work
        }, nameof(CreateCompany), (exception) =>
        {
            // handle error
        });

        // 也可以使用回傳類別(如果有需要交易處理以外的其他邏輯處理)
        ReturnResponse response = await unitOfWork.ExecuteAsync<TestService, ReturnResponse>(async () =>
        {
            // handle unit of work
            return default;
        }, nameof(CreateCompany), (exception) =>
        {
            // handle error
        });
    }
}
```
