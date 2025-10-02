using Base.Domain.Models.NPOI;

namespace Base.NPOI.Interface;

public interface IDataExport
{
    /// <summary>
    /// 設置ExcelData的資料放入位置
    /// </summary>
    /// <param name="headerRowIndex">取得對應db欄位的key值所在的列，預設1</param>
    /// <param name="dataStartRow">從第幾列開始放入data，預設3</param>
    /// <param name="dataStartColumn">從第幾欄開始放入data，預設1</param>
    /// /// <param name="sheetIndex">從第幾個sheet，預設0</param>
    void SetExcelDataPosition(int headerRowIndex, int dataStartRow, int dataStartColumn, int sheetIndex);

    /// <summary>
    /// 不使用範本做資料匯出時，可用此函式在空白excel中放入欄位header
    /// </summary>
    /// <param name="fields">欄位格式設置</param>
    void InitDataFields(IList<NPOIDataField> fields);

    /// <summary>
    /// 將查詢的資料放入Excel，如需調整資料放入位置請使用SetExcelDataPosition
    /// </summary>
    void ExportDataToExcel<T>(IList<T> datas) where T : class;

    /// <summary>
    /// 針對單一儲存格設值
    /// </summary>
    void SetCellValueByIndex(int row, int column, object value);
}

