# 程式碼撰寫規則(簡約版)

> 本文件用於規範程式碼撰寫規則，以確保程式碼的一致性、可讀性、可維護性。
>
> 撰寫規則與 Visual Studio 的 C# 格式化規則和程式碼清除設定相同，並且會隨著Visual Studio的更新與.editorconfig而更新。
>
> 撰寫規則與 Visual Studio Code 的 C# 擴充套件規則相同，並且會隨著Visual Studio Code的更新而更新。
>
> 詳情請閱讀[HackMD](https://hackmd.io/@Jason084/rJddEH0hyg)

[TOC]

## 1. 通則

* **縮排**：使用**空格**進行縮排，每個縮排層級為 **4 個空格**。
* **字元編碼**：所有檔案應使用 UTF-8-BOM 字元編碼。
* **檔案結尾換行**：檔案結尾**不**需要插入新的一行。

## 2. 程式碼結構

### 2.1. using 指示詞

* `using` 指示詞應根據清除程式碼中的using排序設定
* `using` 指示詞應放在命名空間之外。
* `using` 宣告可以使用簡化的 `using` 語句形式 (例如 `using var foo = ...;`)。

### 2.2. 命名空間

* 命名空間應使用檔案範圍的命名空間宣告 (file-scoped namespace) (例如 `namespace MyNamespace;`)。
* 命名空間名稱應與資料夾結構一致。

## 3. 程式碼風格

### 3.1. 關鍵字與型別

* 應使用 C# 語言關鍵字來表示內建型別 (例如，應使用 `int` 而不是 `System.Int32`)。
* 在本地變數、參數和成員中使用預先定義的型別關鍵字。

### 3.2. `this`

* 在程式碼中，**不**應使用 `this.` 來存取事件、欄位、方法和屬性成員，除非有明確的語法衝突。

### 3.3. 括號

* 在算術二元運算子、其他二元運算子和關係二元運算子中，應**總是**使用括號以增加程式碼清晰度。
* 大括號應該獨立一行，並且與控制結構對齊。
* 在其他運算子中，只有在**必要時**才使用括號。

### 3.4. 修飾詞

* 對於非介面成員，應**總是**指定存取修飾詞 (`public`、`private`、`protected`)。
* 建議使用以下修飾詞順序：`public`, `private`, `protected`, `internal`, `file`, `const`, `static`, `extern`, `new`, `virtual`, `abstract`, `sealed`, `override`, `readonly`, `unsafe`, `required`, `volatile`, `async`。

### 3.5. 表達式層級偏好

* 應使用 `??` 空合併運算子 (Coalesce expression)。
* 應使用集合初始器 (Collection initializer)。
* 應使用明確的元組名稱 (Explicit tuple names)。
* 應使用空值傳播運算子 (Null propagation operator, `?.`)。
* 應使用物件初始化器 (Object initializer)。
* 應優先使用自動屬性 (Auto property)。
* 當型別鬆散匹配時，應使用集合表達式 (Collection expression)。
* 應使用複合賦值運算子 (例如 `+=`，`-=`)。
* 在條件賦值和返回時，應使用條件表達式 (`?:`) 而非 `if/else` 語句。
* 在 `foreach` 迴圈中，當有明確的型別宣告時，應優先使用明確的型別轉換。
* 應推斷匿名型別成員名稱和元組名稱。
* 應優先使用 `is null` 檢查，而不是使用 `== null` 或 `!= null` 的方式。
* 應使用簡化的布林表達式 (Boolean expression)。
* 應使用簡化的插值字串 (Interpolated string)。

### 3.6. 欄位

* 如果可能，應將欄位宣告為 `readonly`。

### 3.7. 參數

* 應檢查是否使用未使用的參數。

### 3.8. 變數宣告

* 不應在其他地方使用 `var` 關鍵字。
* 不應針對內建型別使用 `var`。
* 在型別明顯可推斷時可以使用 `var`。

### 3.9. 表達式主體成員

* 應使用表達式主體來定義存取子 (accessors)。
* 不應使用表達式主體來定義建構子、局部函式、方法和運算子。
* 應使用表達式主體來定義索引子和 lambda。
* 應使用表達式主體來定義屬性。

### 3.10. 模式比對

* 應優先使用模式比對而非使用 `as` 運算子並進行空值檢查。
* 應優先使用模式比對而非使用 `is` 運算子並進行型別轉換檢查。
* 應優先使用擴展屬性模式。
* 應優先使用 `not` 模式。
* 應優先使用模式比對。[C# 檔案 模式比對概觀](https://learn.microsoft.com/zh-tw/dotnet/csharp/fundamentals/functional/pattern-matching)
* 應優先使用 switch 表達式。

### 3.11. 空值檢查

* 應使用條件委派呼叫 (`?.Invoke`)。

### 3.12. 其他

* 匿名函式應宣告為 `static`。
* 局部函式應宣告為 `static`。
* 結構體應宣告為 `readonly`。
* 結構體成員應宣告為 `readonly`。
* 應使用大括號 (`{}`)。
* 應使用簡單的 `using` 語句。
* 應優先使用主要建構子 (Primary Constructor)。
* 應優先使用最上層語句 (Top-level statements)。
* 應優先使用簡單的 default 表達式。
* 應使用解構的變數宣告。
* 當型別明顯時，應使用隱式物件建立 (Implicit object creation)。
* 應使用內聯變數宣告 (Inlined variable declaration)。
* 應優先使用索引運算子。
* 應優先使用局部函式而非匿名函式。
* 應優先使用空值檢查而非型別檢查。
* 應優先使用範圍運算子。
* 應優先使用元組交換。
* 應優先使用 UTF-8 字串文字。
* 應使用 `throw` 表達式。
* 應使用棄置字元 (discard, `_`) 變數來忽略未使用的賦值。

## 4. 格式化規則

### 4.1. 換行

* `catch` 前應有換行。
* `else` 前應有換行。
* `finally` 前應有換行。
* 匿名型別中的成員前應有換行。
* 物件初始化器中的成員前應有換行。
* 查詢表達式子句之間應有換行。
* 參數之間應有換行。

### 4.2. 縮排

* 區塊內容應縮排。
* 大括號 `{}` **不應**縮排。
* `case` 內容**應**縮排。
* 當 `case` 內容使用區塊 `{}` 時，**應**縮排。
* `label` **應**比目前縮排少一層。

### 4.3. 空格

* 轉換 (`cast`) 後**不應**有空格。
* 繼承子句中的冒號 `:` 後**應有**空格。
* 逗號 `,` 後**應**有空格。
* 點號 `.` 前後**不應**有空格。
* 控制流程語句中的關鍵字後**應**有空格。
* `for` 語句中的分號 `;` 後**應**有空格。
* 二元運算子前後**應**有空格。
* 宣告語句周圍**不應**有空格。
* 繼承子句中的冒號 `:` 前**應**有空格。
* 逗號 `,` 前**不應**有空格。
* 開啟方括號 `[` 前**不應**有空格。
* `for` 語句中的分號 `;` 前**不應**有空格。
* 空方括號 `[]` 之間**不應**有空格。
* 方法呼叫的空參數列表括號之間**不應**有空格。
* 方法呼叫的名稱與左括號之間**不應**有空格。
* 方法呼叫的參數列表括號之間**不應**有空格。
* 方法宣告的空參數列表括號之間**不應**有空格。
* 方法宣告的名稱與左括號之間**不應**有空格。
* 方法宣告的參數列表括號之間**不應**有空格。
* 括號之間**不應**有空格。

### 4.4. 換行

* 應保留單行程式碼區塊。
* 應保留單行程式碼語句。

## 5. 命名規則

### 5.1. 命名風格

* **PascalCase**：首字母大寫，單字之間無分隔符號，例如 `MyClassName`。
* **IPascalCase**：首字母為 `I` 的 PascalCase，例如 `IMyInterface`。
* **TPascalCase**：首字母為 `T` 的 PascalCase，例如 `TMyType`。
* **camelCase**：首字母小寫，單字之間無分隔符號，例如 `myVariable`。

### 5.2. 命名規則定義

* **型別和命名空間** (Type and Namespace) 應使用 **PascalCase** (例如 `MyClass`, `MyNamespace`)。
* **介面** (Interface) 應使用 **IPascalCase** (例如 `IMyInterface`)。
* **型別參數** (Type Parameter) 應使用 **TPascalCase** (例如 `TMyType`)。
* **方法** (Method) 應使用 **PascalCase** (例如 `MyMethod()`)。
* **屬性** (Property) 應使用 **PascalCase** (例如 `MyProperty`)。
* **事件** (Event) 應使用 **PascalCase** (例如 `MyEvent`)。
* **本地變數** (Local Variable) 應使用 **camelCase** (例如 `myVariable`)。
* **本地常數** (Local Constant) 應使用 **camelCase** (例如 `myConstant`)。
* **參數** (Parameter) 應使用 **camelCase** (例如 `myParameter`)。
* **公開欄位** (Public Field) 應使用 **PascalCase** (例如 `MyField`)。
* **私有欄位** (Private Field) 應使用 **camelCase** (例如 `myField`)。
* **私有靜態欄位** (Private Static Field) 應使用 **camelCase** (例如 `myStaticField`)。
* **公開常數欄位** (Public Constant Field) 應使用 **PascalCase** (例如 `MyConstant`)。
* **私有常數欄位** (Private Constant Field) 應使用 **PascalCase** (例如 `MyPrivateConstant`)。
* **公開靜態 `readonly` 欄位** (Public Static `readonly` Field) 應使用 **PascalCase** (例如 `MyStaticReadonlyField`)。
* **私有靜態 `readonly` 欄位** (Private Static `readonly` Field) 應使用 **PascalCase** (例如 `MyPrivateStaticReadonlyField`)。
* **列舉** (Enum) 應使用 **PascalCase** (例如 `MyEnum`)。
* **局部函式** (Local Function) 應使用 **PascalCase** (例如 `MyLocalFunction()`)。

### 5.3. 變數命名

* 變數名稱應該具有描述性，並且能夠清晰表達變數的用途。
* 變數名稱應該使用**小駝峰命名法（camelCase）**。
* 變數名稱應該避免使用單個字母命名，除非是臨時變數。
* 變數名稱應該避免使用特殊符號，如下劃線、美元符號等。

### 5.4. 方法命名

* 方法名稱應該具有描述性，並且能夠清晰表達函數的用途。
* 方法名稱應該使用**大駝峰命名法（PascalCase）**。
* 方法名稱應該避免使用特殊符號，如下劃線、美元符號等。

#### 5.4.1. 動詞使用

* 方法名稱應該使用動詞開頭，並且能夠清晰表達方法的操作。
* 方法名稱應該使用動詞的原形，如 `Get`、`Set`、`Add`、`Remove` 等。

* 動詞使用對應清單如下：
  * 查詢資料：
    * 取得資料(單筆有條件)：`Get`, `Find`
    * 搜尋資料(多筆有條件)：`Query`, `Search`
  * 新增資料：
    * 新增資料(資料庫)：`Add`, `Insert`
    * 建立資料：`Create`
  * 更新資料：
    * 更新資料：`Update`, `Modify`
    * 更新和新增資料：`Upsert`
  * 刪除資料：
    * 刪除資料：`Delete`, `Remove`
    * 清除資料：`Clear`
  * 檢查資料：
    * 檢查資料：`Check`, `Validate`
    * 確認存在：`Exists`, `Is`

### 5.5. 類別命名

* 類別名稱應該具有描述性，並且能夠清晰表達類別的用途。
* 類別名稱應該使用**大駝峰命名法（PascalCase）**。
* 類別名稱應該避免使用特殊符號，如下劃線、美元符號等。

### 5.6. 屬性命名

* 屬性名稱應該具有描述性，並且能夠清晰表達屬性的用途。
* 屬性名稱應該使用**大駝峰命名法（PascalCase）**。
* 屬性名稱應該避免使用特殊符號，如下劃線、美元符號等。

### 5.7. 欄位命名

* 欄位名稱應該具有描述性，並且能夠清晰表達欄位的用途。
* 欄位名稱應該使用**小駝峰命名法（camelCase）**。
* 欄位名稱應該避免使用特殊符號，如下劃線、美元符號等。

### 5.8. 參數命名

* 參數名稱應該具有描述性，並且能夠清晰表達參數的用途。
* 參數名稱應該使用**小駝峰命名法（camelCase）**。
* 參數名稱應該避免使用特殊符號，如下劃線、美元符號等。

### 5.9. 常數命名

* 常數名稱應該使用大寫字母，並使用下劃線分隔單詞。
* 常數名稱應該使用**大駝峰命名法（PascalCase）**。
* 常數名稱應該具有描述性，並且能夠清晰表達常數的用途。

## 6. 註解規則

* 註解應該具有描述性，並且能夠清晰表達程式碼的用途。

## 7. 其他規則

* log 使用規則

```csharp=
// Debug log
_logger.LogDebug("Hello, World!");

// Information log
_logger.LogInformation("Hello, World!");

// Warning log
_logger.LogWarning("Hello, World!");

// Error log
_logger.LogError("Hello, World!");
```

* log 參數規定

```csharp=
// Bad
_logger.LogDebug($"Hello, " + "World!");
_logger.LogDebug($"Hello, {"World!"}");
_logger.LogDebug(strHelloWorld);

// Debug log with parameter
_logger.LogDebug("Hello, {Name}!", "World");
```
