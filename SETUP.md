# WebApi 安裝手冊

> 本專案使用C# .Net Core進行開發

## 環境安裝注意事項

1. 從公司網路資料夾 `\\file01\Software\VisualStudio` 中安裝 `2022_Professional`
2. 安裝時**必須**勾選 `ASP.NET與網頁程式開發` `Azure開發` `資料儲存處理(選填)`，特別注意 `.NET8.0執行階段(runtime)` `.NET9.0執行階段(runtime)` `.NET SDK` 必須勾到
3. **必須**安裝擴充套件開啟 Visual Studio 2022前往 `擴充功能` > `管理擴充功能`，搜尋並安裝以下擴充套件
    1. `EF Core Power Tools`
    2. `Unit Test Boilerplate Generator`
    3. `Format document on Save`
4. 如有舊專案啟動需求請安裝 `.NET Framework 4.7.2 目標套件(Target Package)` `.NET Framework 4.8 SDK` `.NET Framework 4.8 目標套件(Target Package)`
5. 如有VisualCode開發需求請從公司網路資料夾 `\\file01\Software\VisualStudio` 中安裝 `VSCodeUserSetup-x64-1.95.3.exe`
6. 安裝擴充套件 `.NET Core EditorConfig Generator`[連結](https://marketplace.visualstudio.com/items/?itemName=doggy8088.netcore-editorconfiggenerator) `.NET Core Extension Pack`[連結](https://marketplace.visualstudio.com/items/?itemName=doggy8088.netcore-extension-pack)

### 環境安裝詳細步驟

1. **確認已安裝 Visual Studio 2022**
    * 前往 `\\file01\Software\VisualStudio\` 下載
    * 安裝 Visual Studio 2022_Enterprise or Professional  

2. **確認已安裝 .NET8.0**
    * 安裝時確認已勾選 .NET 8.0 .NET 9.0

3. **安裝擴充套件**
    * 開啟 Visual Studio 2022
    * 前往 `擴充功能` > `管理擴充功能`
    * 搜尋並安裝以下擴充套件：
        * `EF Core Power Tools`
        * `Unit Test Boilerplate Generator`
        * `Format document on Save`

## 下載專案注意事項

1. Git clone from gitlab (有綁雙因子驗證請注意)
2. 使用VS/VSCode開啟方案 `BackEnd.sln` 並還原nuget套件

### 下載專案

1. **安裝 Git**
    * 前往 `\\file01\Software\Git\` 下載
    * 下載並安裝 Git

2. **安裝 Github Desktop**
    * 前往 [Github Desktop 下載頁面](https://desktop.github.com/)
    * 下載並安裝 Github Desktop

3. **Clone 專案**
    * 開啟 Github Desktop
    * 前往 `File` > `Clone Repository`
    * 選擇 `URL`，並輸入 `https://XXX2412.git`
    * 選擇儲存位置後按下 `Clone`
    * 驗證身分後即可開始 Clone 專案

## 更新專案注意事項

### 更新步驟

1. **更新專案**
    * 開啟 Github Desktop
    * 選擇 `Current Repository` > `_2412`
    * 點選 `Fetch origin` 更新專案
    * 點選 `Pull origin` 下載最新版本

2. **還原 NuGet 套件**
    * 開啟 Visual Studio 2022
    * 點選 `檔案` > `開啟` > `專案/方案`
    * 選擇 `Main.sln` 檔案
    * 在方案總管中，右鍵點選 `Main.sln`，選擇 `還原 NuGet 套件`

3. **設定檔案**
    * 複製 `secrets.template.json` 檔案
    * 在方案總管中，右鍵點選 `Main.WebAPI`，選擇 `管理使用者密碼`
    * 這會開啟 `secrets.json` 檔案，將 `secrets.template.json` 的內容貼上到 `secrets.json` 中

4. **測試服務**
    * 在瀏覽器中，前往 `https://localhost:7163/swagger/index.html`
    * 這會開啟 Swagger 介面，可以透過 Swagger 介面測試 API

### 常見問題

1. **無法還原 NuGet 套件**
    * 確認網路連線正常
    * 確認 NuGet 套件庫正常

2. **無法啟動服務**
    * 確認專案是否有誤
    * 確認服務是否有誤

3. **無法測試服務**
    * 確認瀏覽器是否有誤
    * 確認服務是否有誤

4. **無法更新專案**
    * 確認網路連線正常

## 資料庫資訊注意事項

### 開發軟體安裝

1. **安裝 SQL Server Management Studio (SSMS)**
    * 前往 `\\file01\Software\SQL\SSMS` 下載
    * 下載並安裝 SQL Server Management Studio (SSMS)

### 更新ORM(初使不用)

1. **更新資料庫**
    * 開啟 Visual Studio 2022
    * 點選 `檔案` > `開啟` > `專案/方案`
    * 選擇 `Main.sln` 檔案
    * 在專案總管中，右鍵點選 `Main.Infrastructure`，選擇 `EF Core Power Tools` > `Reverse Engineer`
    * 輸入資料庫連線字串，並按下 `Connect`
    * 選擇要更新的資料表，並按下 `Finish`
    * 等待更新完成

### 資料庫連線字串

* 本地資料庫連線字串範例

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```
