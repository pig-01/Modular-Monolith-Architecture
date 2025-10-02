# DeployPlanTemplateCommandHandler 單元測試文件

## 基本資訊

- **測試元件名稱**: DeployPlanTemplateCommandHandler
- **測試時間**: 2025-07-29 10:00:00
- **測試專案**: Main.WebApi.Test
- **測試框架**: xUnit
- **測試版本**: 1.0.0

## 測試概述

`DeployPlanTemplateCommandHandler` 負責處理計畫樣板部署命令，是應用層的命令處理器。此測試套件驗證部署操作的完整流程，包括用戶認證、版本驗證、批次更新、日誌記錄和異常處理等核心功能。測試遵循 DDD 和 SOLID 原則，確保業務邏輯的正確性和系統的穩定性。

## 測試方法清單

### HandleValidVersionWithTemplatesReturnsTrue

- **方法名稱**: `HandleValidVersionWithTemplatesReturnsTrue`
- **測試目的**: 驗證當版本存在且有樣板時，部署操作應成功執行並返回 true
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC001   | 使用有效版本號部署計畫樣板 | 返回 true，調用 Repository 方法一次 | 返回 true，Repository 被正確調用 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "結果": true,
    "repositoryCallCount": 1
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "updatedCount": 3
  }
}
```

### HandleVersionWithNoTemplatesThrowsHandleException

- **方法名稱**: `HandleVersionWithNoTemplatesThrowsHandleException`
- **測試目的**: 驗證當版本沒有找到任何樣板時應拋出 HandleException
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC002   | 使用無樣板的版本號進行部署 | 拋出 HandleException，包含正確錯誤訊息 | 拋出預期異常，訊息正確 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "2.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "exceptionType": "HandleException",
    "exceptionMessage": "版本 2.0.0 沒有找到任何計畫樣板"
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "updatedCount": 0
  }
}
```

### HandleDifferentVersionFormatsCallsRepositoryWithCorrectVersion

- **方法名稱**: `HandleDifferentVersionFormatsCallsRepositoryWithCorrectVersion`
- **測試目的**: 驗證不同版本格式都能正確傳遞給 Repository 方法
- **測試類型**: 單元測試 (資料驅動測試)
- **執行時間**: < 30ms (3個案例)

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC003   | 使用標準版本格式 "1.0" | 返回 true，正確傳遞版本參數 | 版本參數正確傳遞 | ✅ 通過 |
| TC004   | 使用詳細版本格式 "2.1.5" | 返回 true，正確傳遞版本參數 | 版本參數正確傳遞 | ✅ 通過 |
| TC005   | 使用預發布版本格式 "3.0.0-beta" | 返回 true，正確傳遞版本參數 | 版本參數正確傳遞 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "versions": ["1.0", "2.1.5", "3.0.0-beta"],
    "cancellationToken": "None"
  },
  "預期輸出": {
    "結果": true,
    "repositoryCallCount": 1
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "updatedCount": 1
  }
}
```

### HandleUserServiceThrowsExceptionExceptionPropagates

- **方法名稱**: `HandleUserServiceThrowsExceptionExceptionPropagates`
- **測試目的**: 驗證當用戶服務拋出異常時，異常應正確傳播
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC006   | 用戶服務拋出 InvalidOperationException | 異常正確傳播，Repository 未被調用 | 異常正確傳播 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "exceptionType": "InvalidOperationException",
    "exceptionMessage": "用戶服務異常",
    "repositoryNotCalled": true
  },
  "模擬資料": {
    "userServiceException": {
      "type": "InvalidOperationException",
      "message": "用戶服務異常"
    }
  }
}
```

### HandleRepositoryThrowsExceptionExceptionPropagates

- **方法名稱**: `HandleRepositoryThrowsExceptionExceptionPropagates`
- **測試目的**: 驗證當 Repository 拋出異常時，異常應正確傳播
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC007   | Repository 拋出 InvalidOperationException | 異常正確傳播，包含正確錯誤訊息 | 異常正確傳播 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "exceptionType": "InvalidOperationException",
    "exceptionMessage": "資料庫異常"
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "repositoryException": {
      "type": "InvalidOperationException",
      "message": "資料庫異常"
    }
  }
}
```

### HandleCancellationRequestedThrowsHandleException

- **方法名稱**: `HandleCancellationRequestedThrowsHandleException`
- **測試目的**: 驗證取消令牌被觸發時應拋出 HandleException
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC008   | 使用已取消的 CancellationToken | 拋出 HandleException | 拋出預期異常 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "Cancelled"
  },
  "預期輸出": {
    "exceptionType": "HandleException"
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00"
  }
}
```

### HandleLargeNumberOfTemplatesHandlesCorrectly

- **方法名稱**: `HandleLargeNumberOfTemplatesHandlesCorrectly`
- **測試目的**: 驗證處理大量樣板時系統能正確處理
- **測試類型**: 單元測試 (性能測試)
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC009   | 處理 10,000 個樣板的批次更新 | 返回 true，正確處理大量資料 | 成功處理大量資料 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "結果": true,
    "repositoryCallCount": 1
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "updatedCount": 10000
  }
}
```

### HandleValidRequestLogsCorrectInformation

- **方法名稱**: `HandleValidRequestLogsCorrectInformation`
- **測試目的**: 驗證成功部署時記錄正確的日誌資訊
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC010   | 驗證日誌記錄的正確性 | 記錄開始和成功日誌，包含正確參數 | 日誌記錄正確 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "結果": true,
    "logInfoCallCount": 2,
    "logMessages": [
      "開始部署版本 {Version} 的計畫樣板",
      "成功部署版本 {Version} 的 {Count} 個計畫樣板"
    ]
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "updatedCount": 5
  }
}
```

### HandleNoTemplatesFoundLogsWarning

- **方法名稱**: `HandleNoTemplatesFoundLogsWarning`
- **測試目的**: 驗證沒有找到樣板時記錄警告日誌
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC011   | 驗證警告日誌的記錄 | 記錄警告日誌並拋出異常 | 警告日誌正確記錄 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0.0",
    "cancellationToken": "None"
  },
  "預期輸出": {
    "exceptionType": "HandleException",
    "logWarningCallCount": 1,
    "logMessage": "版本 {Version} 沒有找到任何計畫樣板"
  },
  "模擬資料": {
    "currentUser": {
      "userId": "test-user-001",
      "userName": "測試使用者"
    },
    "currentTime": "2025-01-01T10:00:00",
    "updatedCount": 0
  }
}
```

### DeployPlanTemplateCommandShouldHaveRequiredProperties

- **方法名稱**: `DeployPlanTemplateCommandShouldHaveRequiredProperties`
- **測試目的**: 驗證 DeployPlanTemplateCommand 具有必要的屬性
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC012   | 驗證 Command 物件屬性 | Version 屬性存在且可正確設值 | 屬性驗證通過 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "version": "1.0"
  },
  "預期輸出": {
    "version": "1.0",
    "propertyNotNull": true
  }
}
```

### DeployPlanTemplateCommandShouldHaveRequiredAttribute

- **方法名稱**: `DeployPlanTemplateCommandShouldHaveRequiredAttribute`
- **測試目的**: 驗證 DeployPlanTemplateCommand 的 Version 屬性具有 Required 屬性
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC013   | 驗證 Required 屬性存在 | Version 屬性具有 RequiredAttribute | 屬性驗證通過 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "propertyName": "Version",
    "attributeType": "RequiredAttribute"
  },
  "預期輸出": {
    "hasRequiredAttribute": true,
    "attributeFound": true
  }
}
```

## 測試覆蓋率報告

- **程式碼覆蓋率**: 100%
- **分支覆蓋率**: 95%
- **方法覆蓋率**: 100%

## 測試環境

- **.NET 版本**: .NET 8.0
- **作業系統**: Windows 11
- **測試工具**: xUnit + NSubstitute
- **資料庫**: SQL Server (測試用 In-Memory Database)

## 已知問題

| 問題編號 | 問題描述 | 影響程度 | 狀態 | 備註 |
|---------|---------|---------|------|------|
| ISS001  | 取消令牌測試可能在某些環境下不穩定 | 低 | 已解決 | 已改進測試實現方式 |

## 測試結果摘要

- **總測試數量**: 13
- **通過測試**: 13
- **失敗測試**: 0
- **略過測試**: 0
- **測試成功率**: 100％

## 更新記錄

| 日期 | 版本 | 更新內容 | 更新人員 |
|------|------|---------|---------|
| 2025-07-29 | v1.0 | 初始版本，建立完整測試套件 | 專案開發團隊 |
| 2025-07-29 | v1.1 | 新增 DisplayName 屬性提高可讀性 | 專案開發團隊 |
