using Base.Domain.Models.NPOI;

namespace Base.NPOI.Interface;

public interface IDataImport
{
    /// <summary>
    /// 設置ExcelData的資料擷取位置
    /// </summary>
    /// <param name="headerRowIndex">取得對應db欄位的key值所在的列，預設1</param>
    /// <param name="dataStartRow">從第幾列開始擷取data，預設3</param>
    /// <param name="dataStartColumn">從第幾欄開始擷取data，預設1 (通常第0欄用來放驗證錯誤資訊)</param>
    void SetExcelDataPosition(int headerRowIndex, int dataStartRow, int dataStartColumn);

    /// <summary>
    /// 將excel的資料取出，如需調整資料擷取位置請使用SetExcelDataPosition
    /// </summary>
    IList<Dictionary<string, object>> GetExcelData();

    /// <summary>
    /// 在Excel資料匯入前進行驗證，包刮必填、FormulaSQL、唯一值
    /// </summary>
    /// <param name="templates">NPOIExcelTemplate</param>
    /// <param name="checkLists">從FormulaSql中取得的資料限制清單</param>
    /// <param name="pkDuplicateLists">從目標table中查到的的重複資料清單</param>
    bool ValidateExcelWithNPOITemplate(IList<NPOIExcelTemplate> templates, Dictionary<string, IList<string>> checkLists, Dictionary<string, IList<string>> pkDuplicateLists);

}
