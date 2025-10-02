# 主要解決方案

> Demo產品主要解決方案，定義產品所需的全部功能，依照產品定義開發

## 更新記錄

| 日期 | 作者 | 調整內容 | 版本 |
| ---------- | -------- | --------------------------------- | -------- |
| 2025.02.11 | Jason Tsai | 首版 | 1.0.0 |
| 2025.02.25 | Jason Tsai | 新增UserSecert | 1.0.1 |

## 方案資訊

| 項目 | 資訊規格 |
| ---- | ------- |
| Solution Name | Main |

## 檔案相依性

Base.Infrastructure.Interface
Base.Infrastructure.Toolkits
Base.Aspose
Base.Authentication
Base.File
Base.Hangfire
Base.Mail
Base.Security

## 服務專案(Service Project)

| 類別名稱 | 類別描述 | 版本 |
| ------- | -------- | ---- |
| Base.Aspose | Demo產品服務，主要處理 Aspose 服務 | 版本 |
| Base.Authentication | Demo產品服務，主要處理 Authentication 服務 | 版本 |
| Base.File | Demo產品服務，主要處理 File 服務 | 版本 |
| Base.Hangfire | Demo產品服務，主要處理 Hangfire 服務 | 版本 |
| Base.Mail | Demo產品服務，主要處理 Mail 服務 | 版本 |
| Base.Security | Demo產品服務，主要處理 Security 服務 | 版本 |


## 初次安裝

### 前置作業

1. **確認已安裝 Visual Studio 2022**
    * 前往 `\\Demofile01\Software\VisualStudio\` 下載
    * 安裝 Visual Studio 2022_Enterprise or Professional
    
1. **確認已安裝 .NET8.0**
    * 安裝時確認已勾選 .NET 8.0
    
1. **安裝擴充套件**
    - 開啟 Visual Studio 2022
    - 前往 `擴充功能` > `管理擴充功能`
    - 搜尋並安裝以下擴充套件：
        - `EF Core Power Tools`
        - `Unit Test Boilerplate Generator`
        - `Format document on Save`

### 下載專案

1. **安裝 Git**
    - 前往 `\\Demofile01\Software\Git\` 下載
    - 下載並安裝 Git

2. **安裝 Github Desktop**
    - 前往 [Github Desktop 下載頁面](https://desktop.github.com/)
    - 下載並安裝 Github Desktop

3. **Clone 專案**
    - 開啟 Github Desktop
    - 前往 `File` > `Clone Repository`
    - 選擇 `URL`，並輸入 `https://gitlab.Demo.com.tw/GIT/Demo_Demo2412.git`
    - 選擇儲存位置後按下 `Clone`
    - 驗證身分後即可開始 Clone 專案

### 安裝步驟

1. **還原 NuGet 套件**
    - 開啟 Visual Studio 2022
    - 點選 `檔案` > `開啟` > `專案/方案`
    - 選擇 `Main.sln` 檔案
    - 在方案總管中，右鍵點選 `Main.sln`，選擇 `還原 NuGet 套件`

2. **設定檔案**
    - 複製 `secrets.template.json` 檔案
    - 在方案總管中，右鍵點選 `Main.WebAPI`，選擇 `管理使用者密碼`
    - 這會開啟 `secrets.json` 檔案，將 `secrets.template.json` 的內容貼上到 `secrets.json` 中

3. **啟動服務**
    - 在 Visual Studio 2022 中，點選 `偵錯` > `啟動但不偵錯` 或按 `Ctrl + F5`
    - 這會啟動專案並在瀏覽器中開啟

4. **測試服務**
    - 在瀏覽器中，前往 `https://localhost:7163/swagger/index.html`
    - 這會開啟 Swagger 介面，可以透過 Swagger 介面測試 API

### 更新步驟

1. **更新專案**
    - 開啟 Github Desktop
    - 選擇 `Current Repository` > `Demo_Demo2412`
    - 點選 `Fetch origin` 更新專案
    - 點選 `Pull origin` 下載最新版本

2. **還原 NuGet 套件**
    - 開啟 Visual Studio 2022
    - 點選 `檔案` > `開啟` > `專案/方案`
    - 選擇 `Main.sln` 檔案
    - 在方案總管中，右鍵點選 `Main.sln`，選擇 `還原 NuGet 套件`

3. **設定檔案**
    - 複製 `secrets.template.json` 檔案
    - 在方案總管中，右鍵點選 `Main.WebAPI`，選擇 `管理使用者密碼`
    - 這會開啟 `secrets.json` 檔案，將 `secrets.template.json` 的內容貼上到 `secrets.json` 中
 
4. **測試服務**
    - 在瀏覽器中，前往 `https://localhost:7163/swagger/index.html`
    - 這會開啟 Swagger 介面，可以透過 Swagger 介面測試 API

### 常見問題

1. **無法還原 NuGet 套件**
    - 確認網路連線正常
    - 確認 NuGet 套件庫正常

2. **無法啟動服務**
    - 確認專案是否有誤
    - 確認服務是否有誤

 3. **無法測試服務**
    - 確認瀏覽器是否有誤
    - 確認服務是否有誤

4. **無法更新專案**
    - 確認網路連線正常

## 資料庫資訊

### 開發軟體安裝

1. **安裝 SQL Server Management Studio (SSMS)**
    - 前往 `\\Demofile01\Software\SQL\SSMS` 下載
    - 下載並安裝 SQL Server Management Studio (SSMS)

### 更新ORM

1. **更新資料庫**
    - 開啟 Visual Studio 2022
    - 點選 `檔案` > `開啟` > `專案/方案`
    - 選擇 `Main.sln` 檔案
    - 在專案總管中，右鍵點選 `Main.Infrastructure`，選擇 `EF Core Power Tools` > `Reverse Engineer`
    - 輸入資料庫連線字串，並按下 `Connect`
    - 選擇要更新的資料表，並按下 `Finish`
    - 等待更新完成

### 資料庫連線字串

* 本地資料庫連線字串範例
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Demo_Demo;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```


    