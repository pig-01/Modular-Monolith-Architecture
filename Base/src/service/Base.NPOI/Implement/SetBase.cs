using Base.NPOI.Interface;
using NPOI.SS.UserModel;

namespace Base.NPOI.Implement;

public class SetBase : ISetBase
{
    public IWorkbook WorkBook { get; private set; }

    public SetBase(IWorkbook wb) => WorkBook = wb;
}
