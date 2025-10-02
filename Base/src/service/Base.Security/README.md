# Security服務專案

> Demo產品Security服務專案，負責產品加解密相關功能

## 更新記錄

| 日期 | 作者 | 調整內容 | 版本 |
| ---------- | -------- | --------------------------------- | -------- |
| 2025.02.11 | Jason Tsai | 首版 | 1.0.0 |

## 專案資訊

| 項目 | 資訊規格 |
| ---- | ------- |
| Application Name | Base.NPOI |
| Application Type | Classlib |
| Framework | net8.0 |

## 檔案相依性

* Base.Infrastructure.Interface
* Base.Infrastructure.Toolkits


## 類別(Class)

| 類別名稱 | 類別描述 |
| ------- | -------- |
| AsposeFactory | Aspose工廠 |
| AsposeCore | Aspose服務核心 |

### AsposeFactory

#### 方法

| 方法名稱 | 回傳 | 方法描述 |
| ------- | -------- | -------- |
| Create(Stream document) | IAsposeCore | 建立Aspose服務，預設傳入欲操作文件 |
| Create(string licensePath, Stream document) | IAsposeCore | 建立Aspose服務，使用時會註冊 Aspose License，預設傳入欲操作文件 |

### AsposeCore

#### 方法

| 方法名稱 | 回傳 | 方法描述 |
| ------- | -------- | -------- |
| Save(Stream output, SaveFormat format) | SaveOutputParameters | 儲存文件，為 Aspose.Save 封裝方法 |
| ShowFiledInWord() | string |  |
| ShowFiledInWord(string filePath) | string |  |
| ShowFiledInWord(Document document) | string |  |
| InsertImageIntoWord(byte[] image, int left, int top, int width, int height) | void |  |
| InsertImageIntoWord(string licensePath, Stream input, Stream output, byte[] image, int left, int top, int width, int height) | void |  |
| ReplaceParameterIntoWord(Dictionary<string, string> parameters) | void |  |
| ReplaceParameterIntoWord(string licensePath, Stream input, Stream output, Dictionary<string, string> parameters) | void |  |