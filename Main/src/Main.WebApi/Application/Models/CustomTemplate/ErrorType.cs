using Main.Domain.SeedWork;

namespace Main.Dto.ViewModel.CustomTemplate;

public class ErrorType(int id, string name) : Enumeration(id, name)
{
    public static readonly ErrorType RequiredFieldMissing = new(1, "必填欄位缺失");
    public static readonly ErrorType CycleFormatError = new(2, "預設週期格式錯誤");
    public static readonly ErrorType DocumentNameMismatch = new(3, "表單名稱筆無法對應");
    public static readonly ErrorType MultiLanguageTitleMissing = new(4, "多國語系標題缺失");
    public static readonly ErrorType MultiLanguageNameMissing = new(5, "多國語系名稱缺失");
}
