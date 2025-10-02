using NPOI.SS.UserModel;

namespace Base.NPOI.Interface;

public interface ISetBase
{
    IWorkbook WorkBook { get; }
    // 未來有其他工具需要統一初始化可以寫在這
}

