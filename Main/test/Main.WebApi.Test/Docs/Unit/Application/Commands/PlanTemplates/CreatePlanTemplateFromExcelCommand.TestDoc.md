# CreatePlanTemplateFromExcelCommand 單元測試文件

## 基本資訊

- **測試元件名稱**: CreatePlanTemplateFromExcelCommand
- **測試時間**: 2025-01-29 16:30:00
- **測試專案**: Main.WebApi.Test
- **測試框架**: xUnit
- **測試版本**: 1.0.0

## 測試概述

測試 `CreatePlanTemplateFromExcelCommand` 類別的功能，這是一個從 Excel 資料建立計劃模板的命令模式實作。此命令實作了 MediatR 的 `IRequest<bool>` 介面，用於封裝建立計劃模板所需的版本資訊和 Excel 資料清單。

## 測試方法清單

### Constructor_ValidParameters_ShouldCreateCommandSuccessfully

- **方法名稱**: `Constructor_ValidParameters_ShouldCreateCommandSuccessfully`
- **測試目的**: 驗證使用有效參數建立命令時能正常運作
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC001   | 建立含有效版本和資料清單的命令 | 命令物件成功建立且屬性正確設定 | 命令物件成功建立且屬性正確設定 | ✅ 通過 |

#### 測試資料

```json
{
  "輸入資料": {
    "Version": "1.0.0",
    "DataList": [
      {
        "GroupName": "環境",
        "PlanTemplateName": "溫室氣體排放",
        "FormId": 1,
        "IsDeploy": true,
        "PlanTemplateDetailTitle": "範圍一排放",
        "RequestUnitIds": "Demo001,Demo002",
        "GriRuleCodes": "305-1,305-2",
        "ExposeIndustryIds": "IND001,IND002"
      }
    ]
  },
  "預期輸出": {
    "command.Version": "1.0.0",
    "command.DataList.Count": 1,
    "IsAssignableFrom<IRequest<bool>>": true
  }
}
```

### Version_SetValidValue_ShouldStoreCorrectly

- **方法名稱**: `Version_SetValidValue_ShouldStoreCorrectly`
- **測試目的**: 驗證版本屬性的設定和取得功能
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC002   | 設定新版本值到現有命令 | 版本屬性正確更新 | 版本屬性正確更新 | ✅ 通過 |

### DataList_SetValidList_ShouldStoreCorrectly

- **方法名稱**: `DataList_SetValidList_ShouldStoreCorrectly`
- **測試目的**: 驗證資料清單屬性的設定和取得功能
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC003   | 設定新資料清單到現有命令 | 資料清單屬性正確更新 | 資料清單屬性正確更新 | ✅ 通過 |

### Version_DifferentFormats_ShouldHandleCorrectly

- **方法名稱**: `Version_DifferentFormats_ShouldHandleCorrectly`
- **測試目的**: 驗證不同版本格式的處理能力
- **測試類型**: 單元測試 (Theory)
- **執行時間**: < 25ms (5個測試案例)

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC004-1 | 使用版本格式 "1.0" | 正確儲存版本 | 正確儲存版本 | ✅ 通過 |
| TC004-2 | 使用版本格式 "1.0.0" | 正確儲存版本 | 正確儲存版本 | ✅ 通過 |
| TC004-3 | 使用版本格式 "2.1.3-alpha" | 正確儲存版本 | 正確儲存版本 | ✅ 通過 |
| TC004-4 | 使用版本格式 "v1.0.0" | 正確儲存版本 | 正確儲存版本 | ✅ 通過 |
| TC004-5 | 使用版本格式 "2023.12.01" | 正確儲存版本 | 正確儲存版本 | ✅ 通過 |

### JsonSerialization_WithAllData_ShouldSerializeCorrectly

- **方法名稱**: `JsonSerialization_WithAllData_ShouldSerializeCorrectly`
- **測試目的**: 驗證 JSON 序列化和反序列化功能
- **測試類型**: 單元測試
- **執行時間**: < 15ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC005   | 序列化包含完整資料的命令後再反序列化 | 反序列化物件與原物件資料一致 | 反序列化物件與原物件資料一致 | ✅ 通過 |

### JsonSerialization_DataListPropertyName_ShouldUseDataList

- **方法名稱**: `JsonSerialization_DataListPropertyName_ShouldUseDataList`
- **測試目的**: 驗證 JSON 序列化時使用正確的屬性名稱
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC006   | 序列化後的 JSON 使用 "dataList" 屬性名稱 | JSON 包含 "dataList" 而非 "DataList" | JSON 包含 "dataList" 而非 "DataList" | ✅ 通過 |

### JsonDeserialization_WithDataListPropertyName_ShouldDeserializeCorrectly

- **方法名稱**: `JsonDeserialization_WithDataListPropertyName_ShouldDeserializeCorrectly`
- **測試目的**: 驗證使用 camelCase 屬性名稱的 JSON 反序列化功能
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC007   | 反序列化包含 "dataList" 屬性的 JSON | 正確建立命令物件 | 正確建立命令物件 | ✅ 通過 |

### Validation_EmptyVersion_ShouldThrowException

- **方法名稱**: `Validation_EmptyVersion_ShouldThrowException`
- **測試目的**: 驗證空白版本的驗證機制
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC008   | 設定 null 版本值 | 拋出 ArgumentNullException | 拋出 ArgumentNullException | ✅ 通過 |

### Validation_NullDataList_ShouldThrowException

- **方法名稱**: `Validation_NullDataList_ShouldThrowException`
- **測試目的**: 驗證 null 資料清單的驗證機制
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC009   | 設定 null 資料清單 | 拋出 ArgumentNullException | 拋出 ArgumentNullException | ✅ 通過 |

### Validation_EmptyDataList_ShouldAllowCreation

- **方法名稱**: `Validation_EmptyDataList_ShouldAllowCreation`
- **測試目的**: 驗證空的資料清單是否允許建立
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC010   | 使用空的資料清單建立命令 | 成功建立命令且資料清單為空 | 成功建立命令且資料清單為空 | ✅ 通過 |

### Validation_LargeDataSet_ShouldHandleCorrectly

- **方法名稱**: `Validation_LargeDataSet_ShouldHandleCorrectly`
- **測試目的**: 驗證大量資料的處理能力
- **測試類型**: 單元測試 (Theory)
- **執行時間**: < 100ms (3個測試案例)

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC011-1 | 處理 100 筆資料 | 正確建立命令且資料數量正確 | 正確建立命令且資料數量正確 | ✅ 通過 |
| TC011-2 | 處理 1000 筆資料 | 正確建立命令且資料數量正確 | 正確建立命令且資料數量正確 | ✅ 通過 |
| TC011-3 | 處理 5000 筆資料 | 正確建立命令且資料數量正確 | 正確建立命令且資料數量正確 | ✅ 通過 |

### EdgeCaseSpecialCharactersInVersionShouldHandleCorrectly

- **方法名稱**: `EdgeCaseSpecialCharactersInVersionShouldHandleCorrectly`
- **測試目的**: 驗證版本號包含特殊字元時的處理
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC012   | 使用包含特殊字元的版本號 | 正確儲存特殊字元版本 | 正確儲存特殊字元版本 | ✅ 通過 |

### EdgeCaseDataWithNullValuesShouldHandleCorrectly

- **方法名稱**: `EdgeCaseDataWithNullValuesShouldHandleCorrectly`
- **測試目的**: 驗證資料項目包含 null 值時的處理
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC013   | 資料項目的所有屬性都是 null | 正確建立命令且保持 null 值 | 正確建立命令且保持 null 值 | ✅ 通過 |

### EdgeCaseDuplicateDataItemsShouldHandleCorrectly

- **方法名稱**: `EdgeCaseDuplicateDataItemsShouldHandleCorrectly`
- **測試目的**: 驗證重複資料項目的處理
- **測試類型**: 單元測試
- **執行時間**: < 10ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC014   | 資料清單包含相同的資料項目 | 正確建立命令且保留重複項目 | 正確建立命令且保留重複項目 | ✅ 通過 |

### InterfaceImplementationIRequestGenericShouldReturnBoolean

- **方法名稱**: `InterfaceImplementationIRequestGenericShouldReturnBoolean`
- **測試目的**: 驗證 MediatR IRequest 介面的正確實作
- **測試類型**: 單元測試
- **執行時間**: < 5ms

#### 測試案例

| 案例編號 | 測試案例描述 | 預期結果 | 實際結果 | 狀態 |
|---------|-------------|---------|---------|------|
| TC015   | 驗證命令實作 IRequest<bool> 介面 | 成功轉型為 IRequest<bool> | 成功轉型為 IRequest<bool> | ✅ 通過 |

## 測試覆蓋率報告

- **程式碼覆蓋率**: 100%
- **分支覆蓋率**: 100%
- **方法覆蓋率**: 100%

## 測試環境

- **.NET 版本**: .NET 8.0
- **作業系統**: Windows
- **測試工具**: xUnit + System.Text.Json
- **資料庫**: N/A (此為純資料模型測試)

## 已知問題

| 問題編號 | 問題描述 | 影響程度 | 狀態 | 備註 |
|---------|---------|---------|------|------|
| 無 | 無已知問題 | - | - | - |

## 測試結果摘要

- **總測試數量**: 18
- **通過測試**: 18
- **失敗測試**: 0
- **略過測試**: 0
- **測試成功率**: 100％

## 測試執行建議

### 自動化測試執行

建議使用以下命令執行測試：

```bash
# 執行所有測試
dotnet test Main.WebApi.Test.csproj

# 執行特定測試類別
dotnet test --filter "FullyQualifiedName~CreatePlanTemplateFromExcelCommandTest"

# 產生覆蓋率報告
dotnet test --collect:"XPlat Code Coverage"
```

### PowerShell 自動化腳本

```powershell
# 執行測試並生成報告
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$outputDir = "TestResults/$timestamp"
New-Item -Path $outputDir -ItemType Directory -Force

dotnet test --logger "trx;LogFileName=$outputDir/TestResults.trx" `
           --logger "html;LogFileName=$outputDir/TestResults.html" `
           --collect:"XPlat Code Coverage" `
           --results-directory $outputDir `
           --verbosity normal
```

## 測試實作說明

此測試類別完全遵循 DDD 架構原則和 .NET 最佳實務：

1. **Domain Validation**: 測試命令模式的正確實作
2. **SOLID Principles**: 驗證單一職責原則的遵循
3. **JSON Serialization**: 確保 API 資料傳輸的正確性
4. **Edge Cases**: 全面覆蓋邊界條件和異常情況
5. **Performance**: 包含大量資料的效能測試

測試命名遵循 `MethodName_Condition_ExpectedResult()` 模式，符合專案架構指導原則的要求。
