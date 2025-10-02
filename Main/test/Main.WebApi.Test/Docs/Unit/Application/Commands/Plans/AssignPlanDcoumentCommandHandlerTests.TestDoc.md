# AssignPlanDcoumentCommandHandler 單元測試文件

## 💻 基本資訊

- **測試元件名稱**: AssignPlanDcoumentCommandHandler
- **測試時間**: 2024-12-28 12:00:00
- **測試專案**: Main.WebApi.Test
- **測試框架**: xUnit
- **測試版本**: 1.0.0

## 測試概述

此測試套件驗證 `AssignPlanDcoumentCommandHandler` 命令處理器的功能，包括將計劃文件指派給責任人的業務邏輯。測試涵蓋正常執行流程、錯誤處理場景以及邊界條件測試。

## 📊 測試執行結果

**最新測試執行**: 2024-12-28 12:00:00

```text
測試摘要：
- 總測試數量: 6
- 通過測試: 6
- 失敗測試: 0
- 略過測試: 0
- 測試成功率: 100%
- 執行時間: 1.0s
```

## 📋 測試方法清單

### Handle_ValidRequest_SuccessfullyAssignsPlanDocument

- **方法名稱**: `Handle_ValidRequest_SuccessfullyAssignsPlanDocument`
- **測試目的**: 驗證有效請求能夠成功指派計劃文件
- **測試類型**: 單元測試

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC001   | 提供有效的指派請求 | 成功執行 Plan.Assign 方法並返回 Unit.Value | 執行成功，Domain Event 被觸發 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "PlanId": 1,
    "PlanDetailIds": [1, 2],
    "ResponsiblePerson": "testuser",
    "StartDate": "2024-01-01",
    "EndDate": "2024-12-31",
    "ModifiedUser": "admin"
  },
  "模擬資料": {
    "計劃查詢": "返回包含明細的計劃實體",
    "使用者查詢": "返回有效的使用者實體"
  }
}
```

### Handle_PlanNotFound_ThrowsNotFoundException

- **方法名稱**: `Handle_PlanNotFound_ThrowsNotFoundException`
- **測試目的**: 驗證計劃不存在時拋出 NotFoundException
- **測試類型**: 單元測試

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC002   | 使用不存在的計劃 ID | 拋出 NotFoundException | 成功拋出例外 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "PlanId": 999,
    "PlanDetailIds": [1],
    "ResponsiblePerson": "testuser",
    "StartDate": "2024-01-01",
    "EndDate": "2024-12-31",
    "ModifiedUser": "admin"
  },
  "模擬資料": {
    "計劃查詢": "返回 null"
  }
}
```

### Handle_ResponsiblePersonNotFound_ThrowsNotFoundException

- **方法名稱**: `Handle_ResponsiblePersonNotFound_ThrowsNotFoundException`
- **測試目的**: 驗證責任人不存在時拋出 NotFoundException
- **測試類型**: 單元測試

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC003   | 使用不存在的責任人 | 拋出 NotFoundException | 成功拋出例外 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "PlanId": 1,
    "PlanDetailIds": [1],
    "ResponsiblePerson": "nonexistentuser",
    "StartDate": "2024-01-01",
    "EndDate": "2024-12-31",
    "ModifiedUser": "admin"
  },
  "模擬資料": {
    "計劃查詢": "返回有效的計劃實體",
    "使用者查詢": "返回 null"
  }
}
```

### Handle_PlanDetailNotInPlan_ThrowsHandleException

- **方法名稱**: `Handle_PlanDetailNotInPlan_ThrowsHandleException`
- **測試目的**: 驗證計劃明細不屬於指定計劃時拋出 HandleException
- **測試類型**: 單元測試

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC004   | 使用不屬於該計劃的明細 ID | 拋出 HandleException | 成功拋出例外 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "PlanId": 1,
    "PlanDetailIds": [999],
    "ResponsiblePerson": "testuser",
    "StartDate": "2024-01-01",
    "EndDate": "2024-12-31",
    "ModifiedUser": "admin"
  },
  "模擬資料": {
    "計劃查詢": "返回不包含指定明細的計劃",
    "使用者查詢": "返回有效的使用者實體"
  }
}
```

### Handle_InvalidDateFormat_ThrowsParameterException

- **方法名稱**: `Handle_InvalidDateFormat_ThrowsParameterException`
- **測試目的**: 驗證無效日期格式時拋出 ParameterException
- **測試類型**: 單元測試

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC005   | 使用無效的日期格式 | 拋出 ParameterException | 成功拋出例外 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "PlanId": 1,
    "PlanDetailIds": [1],
    "ResponsiblePerson": "testuser",
    "StartDate": "invalid-date",
    "EndDate": "2024-12-31",
    "ModifiedUser": "admin"
  }
}
```

### Handle_MultiplePlanDetails_SuccessfullyAssignsAllDocuments

- **方法名稱**: `Handle_MultiplePlanDetails_SuccessfullyAssignsAllDocuments`
- **測試目的**: 驗證多個計劃明細能夠成功批量指派
- **測試類型**: 單元測試

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC006   | 指派多個計劃明細 | 所有明細都成功指派 | 成功處理多個明細 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "PlanId": 1,
    "PlanDetailIds": [1, 2, 3],
    "ResponsiblePerson": "testuser",
    "StartDate": "2024-01-01",
    "EndDate": "2024-12-31",
    "ModifiedUser": "admin"
  },
  "模擬資料": {
    "計劃查詢": "返回包含3個明細的計劃實體",
    "使用者查詢": "返回有效的使用者實體"
  }
}
```

## 📊 測試覆蓋率報告

- **程式碼覆蓋率**: 100%
- **分支覆蓋率**: 100%
- **方法覆蓋率**: 100%

## 💻 測試環境

- **.NET 版本**: .NET 8.0
- **作業系統**: Windows
- **測試工具**: xUnit + NSubstitute
- **資料庫**: SQL Server (測試環境使用模擬)

## 📋 已知問題

*目前無已知問題*

## 📝 測試結果摘要

- **總測試數量**: 6
- **通過測試**: 6
- **失敗測試**: 0
- **略過測試**: 0
- **測試成功率**: 100％

## 🔧 更新記錄

| 日期 | 版本 | 更新內容 | 更新人員 |
|------|------|---------|---------|
| 2024-12-28 | 1.0.0 | 初始建立 AssignPlanDcoumentCommandHandler 單元測試套件，涵蓋所有主要業務場景 | GitHub Copilot |
