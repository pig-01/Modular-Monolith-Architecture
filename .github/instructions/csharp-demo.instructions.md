---
description: 'Guidelines for building projects in C#'
applyTo: '**/*.cs'
---

# 主要解決方案

> Demo產品後端主要解決方案，定義產品所需的全部功能，依照產品定義開發

## 更新記錄

| 日期 | 作者 | 調整內容 | 版本 |
| ---------- | -------- | --------------------------------- | -------- |
| 2025.02.11 | Jason Tsai | 首版 | 1.0.0 |
| 2025.02.25 | Jason Tsai | 新增UserSecert | 1.0.1 |
| 2025.04.29 | Jason Tsai | 新增Aspire說明，並新增架構簡述說明 | 1.0.2 |

## 方案資訊

| 項目 | 資訊規格 |
| ---- | ------- |
| Solution Name | BackEnd |
| Dotnet Version | `.NET 8.0` `.NET 9.0` |

## 專案

```text
BackEnd
├── Main
│   ├── src
│   │   ├── Main.WebApi
│   │   ├── Main.Service
│   │   ├── Main.Service.Interface
│   │   ├── Main.Infrastructure
│   │   ├── Main.Repository
│   │   ├── Main.Dto
│   │   └── Main.Domain
│   └── test
│       ├── Main.WebApi.Test
│       ├── Main.Service.Test
│       ├── Main.Infrastructure.Test
│       ├── Main.Repository.Test
│       └── Main.Domain.Test
├── Base
│   ├── src
│   │   ├── Base.Infrastructure
│   │   ├── Base.Infrastructure.Interface
│   │   ├── Base.Infrastructure.Toolkits
│   │   ├── Base.Domain
│   │   └── service
│   │       ├── Base.Aspose
│   │       ├── Base.Authentication
│   │       ├── Base.Files
│   │       ├── Base.Mail
│   │       ├── Base.NPOI
│   │       └── Base.Security
│   └── test
│       ├── Base.Aspose.Test
│       ├── Base.Authentication.Test
│       ├── Base.Files.Test
│       ├── Base.Mail.Test
│       ├── Base.NPOI.Test
│       └── Base.Security.Test
└── DataHub
    ├── src
    │   ├── DataHub
    │   ├── Base.DataHub.Bizform
    │   ├── Base.DataHub.Cloud
    │   ├── Base.DataHub.Domain
    │   └── Base.DataHub.Infrastructure
    └── test
        ├── Base.DataHub.Bizform.Test
        └── Base.DataHub.Cloud.Test
```

### 專案清單

* **解決方案** `BackEnd`

* **解決方案** `Main`
  * **WebApi專案**: `Main.WebApi`
  * **Classlib專案**:
    * `Main.Service`
    * `Main.Service.Interface`
    * `Main.Infrastructure`
    * `Main.Repository`
    * `Main.Dto`
    * `Main.Domain`
  * **Test專案**: `Main.Test`

* **解決方案** `Base`
  * **Classlib專案**:
    * `Base.Infrastructure`
    * `Base.Infrastructure.Interface`
    * `Base.Infrastructure.Toolkits`
    * `Base.Domain`
    * `Base.Aspose`
    * `Base.Authentication`
    * `Base.Files`
    * `Base.Mail`
    * `Base.NPOI`
    * `Base.Security`
  * **Test專案**:
    * `Base.Aspose.Test`
    * `Base.Authentication.Test`
    * `Base.Files.Test`
    * `Base.Mail.Test`
    * `Base.NPOI.Test`
    * `Base.Security.Test`

* **解決方案** `DataHub`
  * **WebApi專案**: `DataHub`
  * **Classlib專案**:
    * `Base.DataHub.Bizform`
    * `Base.DataHub.Cloud`
    * `Base.DataHub.Domain`
    * `Base.DataHub.Infrastructure`
  * **Test專案**:
    * `Base.DataHub.Bizform.Test`
    * `Base.DataHub.Cloud.Test`

---

### 系統架構簡述

本專案主要分為三個大項目：

1. **Main**: 主要提供與前端使用者互動的 API，包含帳號中心認證、JWT 認證（可選）及資料處理（CRUD）。
2. **Base**: 提供基礎功能模組，包含 Aspose、Authentication、Files、Mail、NPOI、Security 等服務，這些模組可供其他專案使用。
3. **DataHub**: 提供資料中心功能，包含 Bizform 與 Cloud 服務，這些服務可供其他專案使用。

### 架構說明

本專案採用兩種主要的開發架構：**3-Tier Architecture** 與 **Modular Monolith Architecture**。

**3-Tier Architecture** 主要用於大型專案，將應用程式分為三個層次：啟動專案、商業邏輯層與資料存取層。這種架構有助於清晰地分離關注點，並提高可維護性。
**Modular Monolith Architecture** 則適用於中小型專案，將應用程式分為模組化的專案，每個模組獨立開發與測試，但仍在同一個解決方案中進行整合。

### 專案架構

本專案的架構設計旨在提供一個清晰且易於維護的後端系統。以下是主要的架構說明：

主要提供與前端使用者互動的 API，功能包括帳號中心認證、JWT 認證（可選）及資料處理（CRUD）。應用程式層以 Controller 作為路由處理，且在以下兩種開發架構中共通。

主要解決方案可以使用 `BackEnd` 其中包含所有模組化的專案，這些模組化的專案會依照功能進行分組，並且可以獨立於主要解決方案進行開發與測試。這些模組化的專案會使用 `Base` 專案中的基礎功能，並將這些模組化的專案獨立成為一個解決方案。這些模組化的專案會在 `Main` `DataHub` 專案中進行整合。

在單獨開發模組化專案時，可以使用 `Main` `Base` `DataHub` 個別解決方案進行開發測試。

專案整合工具 .NET Aspire 在 `BackEnd` `Main` 都有提供。

#### **3-Tier Architecture**

* **啟動專案**: `Main.WebApi`
  * 負責乘載路由與基礎功能註冊。
* **商業邏輯層**: `Main.Service` 與 `Main.Service.Interface`
  * `Main.Service`: 負責商業邏輯處理。
  * `Main.Service.Interface`: 負責方法介面，實現應用程式層與邏輯層的介面隔離。
* **資料存取層**: `Main.Repository`
  * 負責資料處理及與資料庫溝通。
* **資料模型層**: `Main.Dto`
  * 存放資料表模型與資料傳輸模型 (DTO)。

---

#### **Modular Monolith Architecture**

* **啟動專案**: `Main.WebApi`
  * 負責乘載路由與基礎功能註冊，並處理部分商業邏輯及資料查詢。
* **基礎層**: `Main.Infrastructure`
  * 提供網站基礎方法及資料處理 (Database Context)。
* **領域模型層**: `Base.Domain`
  * 定義領域模型與相關商業邏輯。

## 其他文件

* 初次安裝手冊 [連結](https://gitlab.Demo.com.tw/GIT/Demo_Demo2412/-/blob/main/BackEnd/SETUP.md)
* 程式碼撰寫規範 [連結](https://gitlab.Demo.com.tw/GIT/Demo_Demo2412/-/blob/main/BackEnd/CODING_RULES.md)
* 開發撰寫範例 [連結](https://gitlab.Demo.com.tw/GIT/Demo_Demo2412/-/blob/main/BackEnd/BEST_PRACTICES.md)
