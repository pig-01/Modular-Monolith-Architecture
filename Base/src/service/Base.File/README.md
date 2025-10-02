# File服務專案

> Demo產品Authentication服務專案，負責產品相關功能

## 更新記錄

| 日期 | 作者 | 調整內容 | 版本 |
| ---------- | -------- | --------------------------------- | -------- |
| 2025.02.11 | Jason Tsai | 首版 | 1.0.0 |

## 專案資訊

| 項目 | 資訊規格 |
| ---- | ------- |
| Application Name | Base.Aspose |
| Application Type | Classlib |
| Framework | net8.0 |

## 專案相依性

| 專案 |
| ---- |
| Base.Infrastructure.Interface |
| Base.Infrastructure.Toolkits |

## 套件相依性

| 項目 | 版本 |
| ---- | ------- |
| Demo.Authentication.CAS.AspNetCore | 5.5.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.10 |

## 類別(Class)

| 類別名稱 | 類別描述 |
| ------- | -------- |
| CASExtension | CAS登入擴充方法 |
| JWTExtension | JWT登入擴充方法 |
| JwtGenerateService | JWT建立服務 |

### CASExtension

#### 方法

| 方法名稱 | 回傳 | 方法描述 |
| ------- | -------- | -------- |
| static AddCookie(this IServiceCollection services, CASSetting setting) | AuthenticationBuilder | 將服務註冊於應用程式服務集合中 |

### JWTExtension

#### 方法

| 方法名稱 | 回傳 | 方法描述 |
| ------- | -------- | -------- |
| static AddJwtBearer(this IServiceCollection services, JWTSetting setting) | AuthenticationBuilder | 將服務註冊於應用程式服務集合中 |

### JwtGenerateService

#### 方法

| 方法名稱 | 回傳 | 方法描述 |
| ------- | -------- | -------- |
| AddRole(string role) | void |  |
| AddRoles(IEnumerable<string> roles) | void |  |
| GetRoles() | List<string> |  |
| GenerateToken(string userId, string userName, Guid jwtId, int expireMinutes = 30) | JwtToken |  |
| GenerateToken(string userId, string userName, string userEmail, Guid jwtId, int expireMinutes = 30) | JwtToken |  |
| GenerateRefreshToken(string userId, string userName, Guid jwtId, int expireMinutes = 60) | JwtToken |  |
| GenerateRefreshToken(string userId, string userName, string userEmail, Guid jwtId, int expireMinutes = 60) | JwtToken |  |
| GetPrincipalAsync(string accessToken) | Task<ClaimsPrincipal?> |  |
