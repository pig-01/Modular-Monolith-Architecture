# 程式碼撰寫規則

###### tags: `demo` `demo Demo` `code rules`

> 本文件用於規範程式碼撰寫規則，以確保程式碼的一致性、可讀性、可維護性。

[TOC]

## 1. 通則

* **縮排**：使用**空格**進行縮排，每個縮排層級為 **4 個空格**。

  這表示每一層的程式碼區塊都應該向右縮進 4 個空格，以增加程式碼的可讀性。

```csharp=
// Bad
public void GetData()
{
  Console.WriteLine("Hello, World!");
}

// Good
public void GetData()
{
	Console.WriteLine("Hello, World!");
}
```

* **字元編碼**：所有檔案應使用 UTF-8-BOM 字元編碼。

  這能確保所有檔案 (包含程式碼、文字檔、設定檔等) 都能正確顯示各種字元，特別是中文等非 ASCII 字元。另外，含有「中文命名」的專案，這條規範尤其重要。

* **檔案結尾換行**：檔案結尾**不**需要插入新的一行。

  這是一種常見的程式碼規範，避免在檔案末尾出現多餘的空行。

## 2. 程式碼結構

### 2.1. using 指示詞

* `using` 指示詞應根據以下規則排序：
  * 系統命名空間 (例如 `System.*`) 應排在前面。

    這能幫助開發者快速識別程式碼使用的系統相關資源。

  * 不同的 `using` 指示詞群組應以空白行分隔。

    透過分組和空白行區隔，可以更清晰地了解程式碼的依賴關係。

* `using` 指示詞應放在命名空間之外。

  這確保了 `using` 指示詞的作用範圍涵蓋整個檔案，並且符合常見的程式碼風格。

* `using` 宣告應使用簡化的 `using` 語句形式 (例如 `using var foo = ...;`)。

  這種簡化的語法能使程式碼更簡潔，並減少程式碼的冗餘。

### 2.2. 命名空間

* 命名空間應使用檔案範圍的命名空間宣告 (file-scoped namespace) (例如 `namespace MyNamespace;`)。

  這種宣告方式更簡潔，並且能更清楚地表明命名空間的作用範圍。

* 命名空間名稱應與資料夾結構一致。

  這能讓程式碼結構更清晰，並方便維護和管理專案。

## 3. 程式碼風格

### 3.1. 關鍵字與型別

* 應使用 C# 語言關鍵字來表示內建型別 (例如，應使用 `int` 而不是 `System.Int32`)。

  使用關鍵字能使程式碼更簡潔，並符合 C# 的慣例。

```csharp=
// Bad
System.Int32 count = 0;

// Good
int count = 0;
```

* 在本地變數、參數和成員中使用預先定義的型別關鍵字。

  這能確保程式碼的一致性，並減少開發者在閱讀程式碼時的認知負擔。

```csharp=
// Bad
var count = 0;

// Good
int count = 0;
```

### 3.2. `this`

* 在程式碼中，**不**應使用 `this.` 來存取事件、欄位、方法和屬性成員，除非有明確的語法衝突。

  這能使程式碼更簡潔，並減少程式碼的噪音。

```csharp=
// Bad
this.count = 0;

// Good
count = 0;
```

### 3.3. 括號

* 在算術二元運算子、其他二元運算子和關係二元運算子中，應**總是**使用括號以增加程式碼清晰度。

  括號能明確表達運算子的優先順序，避免程式碼出現意料之外的行為。

```csharp=
// Bad
int result = a + b * c;

// Good
int result = a + (b * c);
```

* 大括號應該獨立一行，並且與控制結構對齊。

```csharp=
// Bad
if (condition) {
 Console.WriteLine("Hello, World!");
}

// Good
if (condition)
{
 Console.WriteLine("Hello, World!");
}
```


* 在其他運算子中，只有在**必要時**才使用括號。

  避免不必要的括號可以使程式碼更簡潔。

```csharp=
// Bad
if ((a > b) && (b > c))
{
 Console.WriteLine("Hello, World!");
}

// Good
if (a > b && b > c)
{
 Console.WriteLine("Hello, World!");
}
```

### 3.4. 修飾詞

* 對於非介面成員，應**總是**指定存取修飾詞 (`public`、`private`、`protected`)。

  明確指定存取修飾詞能確保程式碼的可維護性，並避免出現不必要的安全隱患。

```csharp=
// Bad
public class MyClass
{
 int count;
}

// Good
public class MyClass
{
 private int count;
}
```

* 建議使用以下修飾詞順序：`public`, `private`, `protected`, `internal`, `file`, `const`, `static`, `extern`, `new`, `virtual`, `abstract`, `sealed`, `override`, `readonly`, `unsafe`, `required`, `volatile`, `async`。

  統一的修飾詞順序可以使程式碼更一致，並方便閱讀。

### 3.5. 表達式層級偏好

* 應使用 `??` 空合併運算子 (Coalesce expression)。

  使用空合併運算子能使程式碼更簡潔，並避免處理空值時出現錯誤。

```csharp=
// Bad
int count = (a != null) ? a : 0;

// Good
int count = a ?? 0;
```

* 應使用集合初始器 (Collection initializer)。

  使用集合初始器能使程式碼更簡潔，並方便初始化集合。

```csharp=
// Bad
List<int> numbers = new List<int>();
numbers.Add(1);
numbers.Add(2);
numbers.Add(3);

// Good
List<int> numbers = new List<int> { 1, 2, 3 };
```

* 應使用明確的元組名稱 (Explicit tuple names)。

  為元組明確指定名稱能使程式碼更易讀，並方便理解元組的含義。

```csharp=
// Bad
var person = (Name: "John", Age: 30);

// Good
var person = (Name: "John", Age: 30);
```

* 應使用空值傳播運算子 (Null propagation operator, `?.`)。

  使用空值傳播運算子能簡化空值檢查，並避免出現空值錯誤。

```csharp=
// Bad
int? count = null;
int result = count.HasValue ? count.Value : 0;

// Good
int? count = null;
int result = count?.Value ?? 0;
```

* 應使用物件初始化器 (Object initializer)。

  使用物件初始化器能使程式碼更簡潔，並方便初始化物件。

```csharp=
// Bad 
Person person = new Person(); 
person.Name = "John"; 
person.Age = 30;

// Good 
Person person = new Person { Name = "John", Age = 30 };
```

* 當換行時，運算子應放在行首。

  這能讓程式碼更易讀，並清楚表達運算符的優先順序。

```csharp=
// Bad
int result = a +
             b +
             c;

// Good
int result = a
    + b
    + c;
```

* 應優先使用自動屬性 (Auto property)。

  自動屬性能減少程式碼的冗餘，並使程式碼更簡潔。

```csharp=
// Bad
private string _name;
public string Name
{
    get { return _name; }
    set { _name = value; }
}

// Good
public string Name { get; set; }
```

* 當型別鬆散匹配時，應使用集合表達式 (Collection expression)。

  集合表達式能更簡潔地初始化集合，並適用於不同型別之間的轉換。

```csharp=
// Bad
List<int> numbers = new List<int>();
numbers.Add(1);

// Good
List<int> numbers = new List<int> { 1 };
```

* 應使用複合賦值運算子 (例如 `+=`，`-=`)。

  複合賦值運算子能使程式碼更簡潔，並減少程式碼的冗餘。

```csharp=
// Bad
count = count + 1;

// Good
count += 1;
```

* 在條件賦值和返回時，應使用條件表達式 (`?:`) 而非 `if/else` 語句。

  條件表達式能更簡潔地表達條件邏輯，並使程式碼更易讀。

```csharp=
// Bad
int result;
if (count > 0)
{
    result = count;
}
else
{
    result = 0;
}

// Good
int result = count > 0 ? count : 0;
```

* 在 `foreach` 迴圈中，當有明確的型別宣告時，應優先使用明確的型別轉換。

  這能確保程式碼的型別安全，並減少運行時錯誤。

```csharp=
// Bad
foreach (var item in items)
{
    Console.WriteLine(item);
}

// Good
foreach (string item in items)
{
    Console.WriteLine(item);
}
```

* 應推斷匿名型別成員名稱和元組名稱。

  自動推斷名稱能使程式碼更簡潔，並減少程式碼的冗餘。

```csharp=
// Example 
var person = new { Name = "John", Age = 30 };
```

* 應優先使用 `is null` 檢查，而不是使用 `== null` 或 `!= null` 的方式。

  使用 `is null` 能更簡潔地檢查空值，並符合 C# 的語法慣例。

```csharp=
// Bad
if (count == null)
{
    Console.WriteLine("Count is null");
}

// Good
if (count is null)
{
    Console.WriteLine("Count is null");
}
```

* 應使用簡化的布林表達式 (Boolean expression)。

  在格式化 .NET 程式碼時，偏好簡化的布林表達式。這意味著在可能的情況下，編輯器會建議將複雜的布林邏輯簡化。例如，將 `if (x == true)` 簡化為 `if (x)`。這樣可以使程式碼更加簡潔和易讀，同時也避免了不必要的比較運算，並提高程式碼的執行效率。

```csharp=
// Bad
if (x == true)
{
    Console.WriteLine("X is true");
}

// Good
if (x)
{
    Console.WriteLine("X is true");
}
```

* 應使用簡化的插值字串 (Interpolated string)。

  在格式化 .NET 程式碼時，偏好簡化的插值字串。插值字串是指在字串中嵌入變數或表達式的值。例如，將 `$"{x.ToString()}"` 簡化為 `$"{x}"`。這樣可以使程式碼更加簡潔，並減少不必要的呼叫，同時也能提升程式碼的可讀性。

```csharp=
// Bad
string message = $"{x.ToString()}";

// Good
string message = $"{x}";
```

### 3.6. 欄位

* 如果可能，應將欄位宣告為 `readonly`。

  如果欄位的值在初始化之後不會被修改，則應宣告為 `readonly`，以確保程式碼的安全性，並防止程式碼意外修改欄位的值。

```csharp=
// Bad
private int count;

// Good
private readonly int count;
```

### 3.7. 參數

* 應檢查是否使用未使用的參數。

  如果參數在方法中沒有被使用，則應該移除該參數，以避免程式碼的混亂，並增加程式碼的可讀性。

```csharp=
// Bad
public void GetData(int count)
{
    Console.WriteLine("Hello, World!");
}

// Good
public void GetData()
{
    Console.WriteLine("Hello, World!");
}
```

### 3.8. 變數宣告

* 不應在其他地方使用 `var` 關鍵字。

   `var` 關鍵字只能用於區域變數宣告，以避免程式碼的混亂。

```csharp=
// Bad
var count = 0;

// Good
int count = 0;
```

* 不應針對內建型別使用 `var`。

  針對內建型別 (如 `int`, `string`, `bool` 等)，應明確指定型別，以提高程式碼的可讀性。

```csharp=
// Bad
var count = 0;

// Good
int count = 0;
```

* 在型別明顯可推斷時可以使用 `var`。

   只有在型別可以明顯從等號右邊推斷出來時，才可以使用 `var`，以避免程式碼的可讀性降低。

```csharp=
// Bad
var count = GetCount();

// Good
int count = GetCount();
```

### 3.9. 表達式主體成員

* 應使用表達式主體來定義存取子 (accessors)。

  對於簡單的存取子 (如 `get` 和 `set`)，應使用表達式主體，以使程式碼更簡潔。

```csharp=
// Bad
private int count;
public int Count
{
    get
    {
        return count;
    }
    set
    {
        count = value;
    }
}

// Good
private int count;
public int Count
{
    get => count;
    set => count = value;
}
```

* 不應使用表達式主體來定義建構子、局部函式、方法和運算子。

  對於複雜的建構子、局部函式、方法和運算子，應使用區塊語法，以增加程式碼的可讀性。

```csharp=
// Example 
public MyClass() { // 建構子內容 }
```

* 應使用表達式主體來定義索引子和 lambda。

  對於簡單的索引子和 lambda，應使用表達式主體，以使程式碼更簡潔。

```csharp=
// Example
public int this[int index] => items[index];
```

* 應使用表達式主體來定義屬性。

   對於簡單的屬性，應使用表達式主體，以使程式碼更簡潔。

```csharp=
// Example
public int Count => items.Count;
```

### 3.10. 模式比對

* 應優先使用模式比對而非使用 `as` 運算子並進行空值檢查。

   模式比對能更簡潔地檢查型別和空值，並減少程式碼的冗餘。

```csharp=
// Bad
if (obj is MyClass)
{
    var myClass = obj as MyClass;
    Console.WriteLine(myClass.Name);
}

// Good
if (obj is MyClass myClass)
{
    Console.WriteLine(myClass.Name);
}
```

* 應優先使用模式比對而非使用 `is` 運算子並進行型別轉換檢查。

   模式比對能更簡潔地檢查型別和進行型別轉換，並減少程式碼的冗餘。

```csharp=
// Bad
if (obj is MyClass)
{
    var myClass = (MyClass)obj;
    Console.WriteLine(myClass.Name);
}

// Good
if (obj is MyClass myClass)
{
    Console.WriteLine(myClass.Name);
}
```

* 應優先使用擴展屬性模式。

  擴展屬性模式能更簡潔地存取嵌套屬性，並減少程式碼的冗餘。

```csharp=
// Bad
if (person != null && person.Address != null && person.Address.City != null)
{
    Console.WriteLine(person.Address.City);
}

// Good
if (person?.Address?.City is string city)
{
    Console.WriteLine(city);
}
```

* 應優先使用 `not` 模式。

  `not` 模式能更簡潔地表達條件否定，並增加程式碼的可讀性。

```csharp=
// Bad
if (!(count > 0))
{
    Console.WriteLine("Count is not greater than 0");
}

// Good
if (count <= 0)
{
    Console.WriteLine("Count is not greater than 0");
}
```

* 應優先使用模式比對。[C# 檔案 模式比對概觀](https://learn.microsoft.com/zh-tw/dotnet/csharp/fundamentals/functional/pattern-matching)

  在可以使用模式比對的地方，應盡量使用模式比對，以增加程式碼的可讀性。

```csharp=
// Bad
if (obj is MyClass)
{
    var myClass = (MyClass)obj;
    Console.WriteLine(myClass.Name);
}

// Good
if (obj is MyClass myClass)
{
    Console.WriteLine(myClass.Name);
}
```

* 應優先使用 switch 表達式。

  當需要使用 `switch` 語句時，應優先使用 `switch` 表達式，以增加程式碼的簡潔性。

```csharp=
// Bad
switch (count)
{
    case 0:
        Console.WriteLine("Count is 0");
        break;
    case 1:
        Console.WriteLine("Count is 1");
        break;
    default:
        Console.WriteLine("Count is not 0 or 1");
        break;
}

// Good
string message = count switch
{
    0 => "Count is 0",
    1 => "Count is 1",
    _ => "Count is not 0 or 1"
};
Console.WriteLine(message);
```

### 3.11. 空值檢查

* 應使用條件委派呼叫 (`?.Invoke`)。

  條件委派呼叫能更簡潔地檢查委派是否為空，並避免出現空值錯誤。

```csharp=
// Bad
if (action != null)
{
    action();
}

// Good
action?.Invoke();
```

### 3.12. 其他

* 匿名函式應宣告為 `static`。

  如果匿名函式不需要訪問外部變數，則應宣告為 `static`，以提高程式碼的性能。

```csharp=
// Bad
Func<int, int> add = (int x) => x + 1;

// Good
static Func<int, int> add = (int x) => x + 1;
```

* 局部函式應宣告為 `static`。

  如果局部函式不需要訪問外部變數，則應宣告為 `static`，以提高程式碼的性能。

```csharp=
// Bad
void Add(int x, int y)
{
    int result = Add(x, y);
}

// Good
static void Add(int x, int y)
{
    int result = Add(x, y);
}
```

* 結構體應宣告為 `readonly`。

  如果結構體的值在初始化之後不會被修改，則應宣告為 `readonly`，以確保程式碼的安全性，並防止程式碼意外修改結構體的值。

```csharp=
// Bad
struct Point
{
    public int X;
    public int Y;
}

// Good
readonly struct Point
{
    public int X;
    public int Y;
}
```

* 結構體成員應宣告為 `readonly`。

  如果結構體的成員的值在初始化之後不會被修改，則應宣告為 `readonly`，以確保程式碼的安全性，並防止程式碼意外修改結構體成員的值。
* 
```csharp=
// Bad
struct Point
{
    public int X;
    public int Y;
}

// Good
readonly struct Point
{
    public readonly int X;
    public readonly int Y;
}
```

* 應使用大括號 (`{}`)。

  在所有控制流程語句 (如 `if`、`for`、`while` 等) 中都應使用大括號，以提高程式碼的可讀性。

```csharp=
// Bad
if (count > 0)
    Console.WriteLine("Count is greater than 0");

// Good
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}
```

* 應使用簡單的 `using` 語句。

  對於只需要釋放資源的 `using` 語句，應使用簡單的 `using` 語句，以減少程式碼的冗餘。

```csharp=
// Bad
using (var stream = new FileStream("file.txt", FileMode.Open))
{
    stream.Read(buffer, 0, buffer.Length);
}

// Good
using var stream = new FileStream("file.txt", FileMode.Open);
stream.Read(buffer, 0, buffer.Length);
```

* 應優先使用主要建構子 (Primary Constructor)。

  如果類別或結構體只有主要的建構子，則應使用主要建構子，以減少程式碼的冗餘。

```csharp=
// Bad
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

// Good
public class Person(string name, int age)
{
    public string Name { get; set; } = name;
    public int Age { get; set; } = age;
}
```

* 應優先使用最上層語句 (Top-level statements)。

   如果程式碼只包含簡單的程式碼邏輯，則應使用最上層語句，以減少程式碼的冗餘。

```csharp=
// Bad
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

// Good
Console.WriteLine("Hello, World!");
```

* 應優先使用簡單的 default 表達式。

   當需要使用 `default` 值時，應使用簡單的 `default` 表達式，以增加程式碼的可讀性。

```csharp=
// Bad
int count = default(int);

// Good
int count = default;
```

* 應使用解構的變數宣告。

  在需要使用元組或其他可解構型別時，應使用解構的變數宣告，以增加程式碼的可讀性。

```csharp=
// Bad
var (name, age) = GetPerson();

// Good
(string name, int age) = GetPerson();
```

* 當型別明顯時，應使用隱式物件建立 (Implicit object creation)。

  當型別可以明顯從等號右邊推斷出來時，應使用隱式物件建立，以增加程式碼的簡潔性。

```csharp=
// Bad
Person person = new Person { Name = "John", Age = 30 };

// Good
var person = new Person { Name = "John", Age = 30 };

// Good
Person person = new() { Name = "John", Age = 30 };
```

* 應使用內聯變數宣告 (Inlined variable declaration)。

   如果變數只在一個地方被使用，則應使用內聯變數宣告，以減少程式碼的冗餘。

```csharp=
// Bad
int count = GetCount();
Console.WriteLine(count);

// Good
Console.WriteLine(GetCount());
```

* 應優先使用索引運算子。

  當需要使用索引存取集合時，應優先使用索引運算子，以增加程式碼的可讀性。

```csharp=
// Bad
int count = numbers.ElementAt(0);

// Good
int count = numbers[0];
```

* 應優先使用局部函式而非匿名函式。

  如果需要使用函式，且函式不需要訪問外部變數，則應優先使用局部函式，以提高程式碼的性能。

```csharp=
// Bad
Func<int, int> add = (int x) => x + 1;

// Good
int Add(int x) => x + 1;
```

* 應優先使用空值檢查而非型別檢查。

  在需要檢查物件是否為空時，應優先使用空值檢查，以避免出現不必要的型別錯誤。

```csharp=
// Bad
if (obj is MyClass)
{
    var myClass = (MyClass)obj;
    Console.WriteLine(myClass.Name);
}

// Good
if (obj is MyClass myClass)
{
    Console.WriteLine(myClass.Name);
}
```

* 應優先使用範圍運算子。

  當需要使用範圍時，應優先使用範圍運算子，以增加程式碼的可讀性。

```csharp=
// Bad
for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine(numbers[i]);
}

// Good
foreach (var number in numbers)
{
    Console.WriteLine(number);
}
```

* 應優先使用元組交換。

  當需要交換兩個變數的值時，應優先使用元組交換，以增加程式碼的簡潔性。

```csharp=
// Bad
int temp = a;
a = b;
b = temp;

// Good
(a, b) = (b, a);
```

* 應優先使用 UTF-8 字串文字。

  當需要使用 UTF-8 字串文字時，應使用 UTF-8 字串文字，以確保程式碼的字元編碼正確。

* 應使用 `throw` 表達式。

  當需要 `throw` 例外時，應使用 `throw` 表達式，以增加程式碼的簡潔性。

```csharp=
// Example
if (count < 0)
{
    throw new ArgumentOutOfRangeException(nameof(count), "Count must be greater than or equal to 0");
}
```

* 應使用棄置字元 (discard, `_`) 變數來忽略未使用的賦值。

  當程式碼中有未使用的變數賦值時，應使用棄置字元 (`_`) 忽略，以避免程式碼的警告。

```csharp=
// Bad
int count = GetCount();

// Good
_ = GetCount();
```

## 4. 格式化規則

### 4.1. 換行

* `catch` 前應有換行。

  在 `try...catch` 語句中，`catch` 關鍵字之前應有一個新行，以增加程式碼的可讀性。

```csharp=
// Bad
try
{
    Console.WriteLine("Hello, World!");
}catch (SmtpException smtpException)
{
    Console.WriteLine(smtpException.Message);
}
}catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// Good
try
{
    Console.WriteLine("Hello, World!");
}
catch (SmtpException smtpException)
{
    Console.WriteLine(smtpException.Message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

* `else` 前應有換行。

  在 `if...else` 語句中，`else` 關鍵字之前應有一個新行，以增加程式碼的可讀性。

```csharp=
// Bad
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}else
{
    Console.WriteLine("Count is not greater than 0");
}

// Good
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}
else
{
    Console.WriteLine("Count is not greater than 0");
}
```

* `finally` 前應有換行。

  在 `try...finally` 語句中，`finally` 關鍵字之前應有一個新行，以增加程式碼的可讀性。

```csharp=
// Bad
try
{
    Console.WriteLine("Hello, World!");
}finally
{
Console.WriteLine("Finally block");
}

// Good
try
{
    Console.WriteLine("Hello, World!");
}
finally
{
    Console.WriteLine("Finally block");
}
```

* 匿名型別中的成員前應有換行。

  在匿名型別中，成員之間應有新行，以增加程式碼的可讀性。

```csharp=
// Bad
var person = new { Name = "John", Age = 30 };

// Good
var person = new
{
    Name = "John",
    Age = 30
};
```

* 物件初始化器中的成員前應有換行。

  在物件初始化器中，成員之間應有新行，以增加程式碼的可讀性。

```csharp=
// Bad
Person person = new Person { Name = "John", Age = 30 };

// Good
Person person = new Person
{
    Name = "John",
    Age = 30
};
```

* 查詢表達式子句之間應有換行。

  在查詢表達式中，各個子句之間應有新行，以增加程式碼的可讀性。

```csharp=
// Bad
WebApplication.CreateBuilder(args).Build().Run();

// Good
WebApplication.CreateBuilder(args)
    .Build()
    .Run();
```
* 參數之間應有換行。

  在方法的參數列表中，每個參數應該在不同的行上，以增加程式碼的可讀性。

```csharp=
// Bad
public void GetData(int count, string name, int age)
{
    Console.WriteLine("Hello, World!");
}

// Good
public void GetData(
    int count,
    string name,
    int age)
{
    Console.WriteLine("Hello, World!");
}
```

### 4.2. 縮排

* 區塊內容應縮排。

  所有程式碼區塊的內容都應該向右縮排，以增加程式碼的可讀性。

```csharp=
// Bad
if (count > 0)
{
Console.WriteLine("Count is greater than 0");
}

// Good
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}
```

* 大括號 `{}` **不應**縮排。

  程式碼區塊的左大括號和右大括號，應與對應的程式碼關鍵字對齊，而不是縮排，以增加程式碼的可讀性。

```csharp=
// Bad
if (count > 0)
    {
        Console.WriteLine("Count is greater than 0");
    }   

// Good
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}
```

* `case` 內容**應**縮排。

  在 `switch` 語句中，`case` 關鍵字之後的內容應向右縮排，以增加程式碼的可讀性。

```csharp=
// Bad
switch (count)
{
case 0:
Console.WriteLine("Count is 0");
break;
case 1:
Console.WriteLine("Count is 1");
break;
default:
Console.WriteLine("Count is not 0 or 1");
break;
}

// Good
switch (count)
{
    case 0:
        Console.WriteLine("Count is 0");
        break;
    case 1:
        Console.WriteLine("Count is 1");
        break;
    default:
        Console.WriteLine("Count is not 0 or 1");
        break;
}
```

* 當 `case` 內容使用區塊 `{}` 時，**應**縮排。

  當 `case` 關鍵字之後的內容使用大括號包圍時，應將內容向右縮排，以增加程式碼的可讀性。

```csharp=
// Bad
switch (count)
{
case 0:
{
Console.WriteLine("Count is 0");
break;
}
case 1:
{
Console.WriteLine("Count is 1");
break;
}
default:
{
Console.WriteLine("Count is not 0 or 1");
break;
}
}

// Good
switch (count)
{
    case 0:
    {
        Console.WriteLine("Count is 0");
        break;
    }
    case 1:
    {
        Console.WriteLine("Count is 1");
        break;
    }
    default:
    {
        Console.WriteLine("Count is not 0 or 1");
        break;
    }
}
```

* `label` **應**比目前縮排少一層。

  `label` 標籤應該比目前的程式碼區塊縮排少一層，以增加程式碼的可讀性。

```csharp=
// Example
switch (count)
{
    case 0:
        Console.WriteLine("Count is 0");
        break;
    case 1:
        Console.WriteLine("Count is 1");
        break;
    default:
        Console.WriteLine("Count is not 0 or 1");
        break;
}
```

### 4.3. 空格

* 轉換 (`cast`) 後**不應**有空格。

  型別轉換運算子和變數之間不應有空格。

```csharp=
// Bad
int count = (int) 10.5;

// Good
int count = (int)10.5;
```

* 繼承子句中的冒號 `:` 後**應有**空格。

  在類別繼承或介面實作中，冒號之後應有一個空格。

```csharp=
// Bad
public class Person: IPerson
{
    public string Name { get; set; }
}

// Good
public class Person : IPerson
{
    public string Name { get; set; }
}
```

* 逗號 `,` 後**應**有空格。

  在變數宣告、方法參數和集合初始化器中，逗號之後應有一個空格。

```csharp=
// Bad
int[] numbers = {1,2,3};

// Good
int[] numbers = { 1, 2, 3 };
```

* 點號 `.` 前後**不應**有空格。

  在物件成員存取時，點號前後都不應有空格。

```csharp=
// Bad
Console .WriteLine("Hello, World!");

// Good
Console.WriteLine("Hello, World!");
```

* 控制流程語句中的關鍵字後**應**有空格。

  在 `if`、`for`、`while` 等控制流程語句中，關鍵字之後應有一個空格。

```csharp=
// Bad
if(count>0)
{
    Console.WriteLine("Count is greater than 0");
}

// Good
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}
```

* `for` 語句中的分號 `;` 後**應**有空格。

  在 `for` 迴圈中，分號之後應有一個空格。

```csharp=
// Bad
for (int i = 0;i < 10;i++)
{
    Console.WriteLine(i);
}

// Good
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}
```

* 二元運算子前後**應**有空格。

  在二元運算子 (如 `+`、`-`、`*`、`/` 等) 前後都應有一個空格。

```csharp=
// Bad
int sum = 10+20;

// Good
int sum = 10 + 20;
```

* 宣告語句周圍**不應**有空格。

  在變數宣告語句前後不應有額外空格。

```csharp=
// Bad
int count  = 0 ;

// Good
int count = 0;
```

* 繼承子句中的冒號 `:` 前**應**有空格。

  在類別繼承或介面實作中，冒號之前應有一個空格。

```csharp=
// Bad
public class Person :IPerson
{
    public string Name { get; set; }
}

// Good
public class Person : IPerson
{
    public string Name { get; set; }
}
```

* 逗號 `,` 前**不應**有空格。

  在變數宣告、方法參數和集合初始化器中，逗號之前不應有空格。

```csharp=
// Bad
int[] numbers = { 1 , 2 , 3 };

// Good
int[] numbers = { 1, 2, 3 };
```

* 開啟方括號 `[` 前**不應**有空格。

  在集合或索引器存取中，開啟方括號之前不應有空格。

```csharp=
// Bad
int count = numbers[ 0 ];

// Good
int count = numbers[0];
```

* `for` 語句中的分號 `;` 前**不應**有空格。

  在 `for` 迴圈中，分號之前不應有空格。

```csharp=
// Bad
for (int i = 0 ; i < 10 ; i++)
{
    Console.WriteLine(i) ;
}

// Good
for (int i = 0; i < 10; i++)
{
    Console.WriteLine(i);
}
```

* 空方括號 `[]` 之間**不應**有空格。

  在宣告空陣列或空索引時，空方括號之間不應有空格。

```csharp=
// Bad
int[ ] numbers = { 1, 2, 3 };

// Good
int[] numbers = { 1, 2, 3 };
```
* 方法呼叫的空參數列表括號之間**不應**有空格。

  在方法呼叫時，如果參數列表為空，則括號之間不應有空格。

```csharp=
// Bad
GetData( );

// Good
GetData();
```

* 方法呼叫的名稱與左括號之間**不應**有空格。

  在方法呼叫時，方法名稱和左括號之間不應有空格。

```csharp=
// Bad
GetData (10);

// Good
GetData(10);
```

* 方法呼叫的參數列表括號之間**不應**有空格。

  在方法呼叫時，如果參數列表不為空，則參數列表的括號之間不應有空格。

```csharp=
// Bad
GetData( 10 );

// Good
GetData(10);
```

* 方法宣告的空參數列表括號之間**不應**有空格。

  在方法宣告時，如果參數列表為空，則括號之間不應有空格。

```csharp=
// Bad
public void GetData( ) { }

// Good
public void GetData() { }
```

* 方法宣告的名稱與左括號之間**不應**有空格。

  在方法宣告時，方法名稱和左括號之間不應有空格。

```csharp=
// Bad
public void GetData (int count) { }

// Good
public void GetData(int count) { }
```

* 方法宣告的參數列表括號之間**不應**有空格。

  在方法宣告時，如果參數列表不為空，則參數列表的括號之間不應有空格。

```csharp=
// Bad
public void GetData ( int count ) { }

// Good
public void GetData(int count) { }
```

* 括號之間**不應**有空格。

  在括號內不應有額外空格。

```csharp=
// Bad
if ( count > 0 )
{
    Console.WriteLine("Count is greater than 0");
}

// Good
if (count > 0)
{
    Console.WriteLine("Count is greater than 0");
}
```

### 4.4. 換行

* 應保留單行程式碼區塊。

  在格式化 C# 程式碼時，應該保留單行的區塊。區塊通常是指用大括號 `{}` 包圍的程式碼片段，例如 `if` 語句或方法定義。如果這個區塊的內容可以放在同一行，編輯器將會保留這種單行格式，而不會自動將其展開成多行。例如，`if (x) { return true; }` 會被保留在一行內。

* 應保留單行程式碼語句。

  在格式化 C# 程式碼時，應該保留單行的語句。這意味著如果某個語句可以放在一行內，編輯器將會保留這種單行格式，而不會自動將其拆分成多行。例如，`int x = 10;` 會被保留在一行內。


## 5. 命名規則

### 5.1. 命名風格

* **PascalCase**：首字母大寫，單字之間無分隔符號，例如 `MyClassName`。
* **IPascalCase**：首字母為 `I` 的 PascalCase，例如 `IMyInterface`。
* **TPascalCase**：首字母為 `T` 的 PascalCase，例如 `TMyType`。
* **camelCase**：首字母小寫，單字之間無分隔符號，例如 `myVariable`。

* ### 5.2. 命名規則定義

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

```csharp=
// Bad
var a = 1;

// Good
var count = 1;
```

### 5.4. 方法命名

* 方法名稱應該具有描述性，並且能夠清晰表達函數的用途。
* 方法名稱應該使用**大駝峰命名法（PascalCase）**。
* 方法名稱應該避免使用特殊符號，如下劃線、美元符號等。

```csharp=
// Bad
public void get_data() { }

// Good
public void GetData() { }
```

#### 5.4.1. 動詞使用

* 方法名稱應該使用動詞開頭，並且能夠清晰表達方法的操作。
* 方法名稱應該使用動詞的原形，如 `Get`、`Set`、`Add`、`Remove` 等。

```csharp=
// Bad
public void data() { }

// Good
public void GetData() { }
```

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

```csharp=
// Bad
public class data_manager { }

// Good
public class DataManager { }
```

### 5.6. 屬性命名

* 屬性名稱應該具有描述性，並且能夠清晰表達屬性的用途。
* 屬性名稱應該使用**大駝峰命名法（PascalCase）**。
* 屬性名稱應該避免使用特殊符號，如下劃線、美元符號等。

```csharp=
// Bad
public string first_name { get; set; }

// Good
public string FirstName { get; set; }
```

### 5.7. 欄位命名

* 欄位名稱應該具有描述性，並且能夠清晰表達欄位的用途。
* 欄位名稱應該使用**小駝峰命名法（camelCase）**。
* 欄位名稱應該避免使用特殊符號，如下劃線、美元符號等。

```csharp=
// Bad
private string first_name;

// Good
private string firstName;
```

### 5.8. 參數命名

* 參數名稱應該具有描述性，並且能夠清晰表達參數的用途。
* 參數名稱應該使用**小駝峰命名法（camelCase）**。
* 參數名稱應該避免使用特殊符號，如下劃線、美元符號等。

```csharp=
// Bad
public void GetData(string first_name) { }

// Good
public void GetData(string firstName) { }
```

### 5.9. 常數命名

* 常數名稱應該使用大寫字母，並使用下劃線分隔單詞。
* 常數名稱應該使用**大駝峰命名法（PascalCase）**。
* 常數名稱應該具有描述性，並且能夠清晰表達常數的用途。

```csharp=
// Bad
public const int MAX_COUNT = 100;

// Good
public const int MaxCount = 100;
```


## 6. 註解規則

* 註解應該具有描述性，並且能夠清晰表達程式碼的用途。

```csharp=
// Bad
// Get data
public void GetData() { }

// Good
// Get data from database
public void GetData() { }
```

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
