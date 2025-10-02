using Base.Domain.Models.NPOI;
using Base.NPOI.Implement;
using Base.NPOI.Interface;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Base.NPOI;

public class NpoiUtil : IDisposable
{

    private readonly ExcelType _fileType = ExcelType.xlsx;
    private readonly Stream _fsExcel;
    private IDataImport _dataImport;
    private IDataExport _dataExport;
    private IDataReplace _dataReplace;

    public enum ExcelType
    {
        xls,
        xlsx
    }

    public IWorkbook WorkBook { get; private set; }

    /// <summary>
    /// 判斷Excel版本
    /// </summary>
    /// <param name="fsExcel"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>
    private static IWorkbook getWorkBook(Stream fsExcel, ExcelType fileType)
    {
        IWorkbook wb;
        if (fileType == ExcelType.xlsx)
        {
            fsExcel.Position = 0;
            wb = new XSSFWorkbook(fsExcel);
        }
        else
        {
            fsExcel.Position = 0;
            wb = new HSSFWorkbook(fsExcel);
        }
        return wb;
    }

    public static NpoiUtil WorkFactory(ExcelType excelType)
    {
        IWorkbook wb;
        if (excelType == ExcelType.xlsx)
        {
            wb = new XSSFWorkbook();
        }
        else
        {
            wb = new HSSFWorkbook();
        }
        wb.CreateSheet();
        MemoryStream mem = new();
        //wb.Write(mem);
        return new NpoiUtil(wb, mem, excelType);
    }

    public static NpoiUtil WorkFactory(string path)
    {
        MemoryStream mem = new();
        using (FileStream fsExcel = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) // 直接抄一份範本到 memory 裡面
        {
            ExcelType ft = fsExcel.Name.EndsWith(".xlsx") ? ExcelType.xlsx : ExcelType.xls;
            fsExcel.CopyTo(mem);
            return new NpoiUtil(getWorkBook(mem, ft), mem, ft);
        }
    }

    public static NpoiUtil WorkFactory(MemoryStream mem, ExcelType excelType) => new(getWorkBook(mem, excelType), mem, excelType);

    private NpoiUtil(IWorkbook wb, Stream fsExcel, ExcelType fileType)
    {
        WorkBook = wb;
        _fsExcel = fsExcel;
        _fileType = fileType;
    }

    private IDataImport DataImport
    {
        get
        {
            _dataImport ??= new DataImport(WorkBook);
            return _dataImport;
        }
    }

    private IDataExport DataExport
    {
        get
        {
            _dataExport ??= new DataExport(WorkBook);
            return _dataExport;
        }
    }

    private IDataReplace DataReplace
    {
        get
        {
            _dataReplace ??= new DataReplace(WorkBook);
            return _dataReplace;
        }
    }

    #region DataExport

    /// <summary>
    /// 設置ExcelData的資料放置位置
    /// </summary>
    /// <param name="headerRowIndex">取得對應db欄位的key值所在的列，預設1</param>
    /// <param name="dataStartRow">從第幾列開始擷取data，預設3</param>
    /// <param name="dataStartColumn">從第幾欄開始擷取data，預設1 (通常第0欄用來放驗證錯誤資訊)</param>
    public void SetExcelDataExportPosition(int headerRowIndex, int dataStartRow, int dataStartColumn, int sheetIndex) => DataExport.SetExcelDataPosition(headerRowIndex, dataStartRow, dataStartColumn, sheetIndex);

    /// <summary>
    /// 不使用範本做資料匯出時，可用此函式在空白excel中放入欄位header
    /// </summary>
    /// <param name="fields">欄位格式設置</param>
    public void InitDataFields(IList<NPOIDataField> fields) => DataExport.InitDataFields(fields);

    /// <summary>
    /// 將查詢的資料放入Excel，如需調整資料放入位置請使用SetExcelDataPosition
    /// </summary>
    public void ExportDataToExcel<T>(IList<T> datas) where T : class => DataExport.ExportDataToExcel(datas);

    /// <summary>
    /// 針對單一儲存格設值
    /// </summary>
    public void SetCellValueByIndex(int row, int column, object value) => DataExport.SetCellValueByIndex(row, column, value);

    #endregion


    #region DataImport

    /// <summary>
    /// 設置ExcelData的資料擷取位置
    /// </summary>
    /// <param name="headerRowIndex">取得對應db欄位的key值所在的列，預設1</param>
    /// <param name="dataStartRow">從第幾列開始擷取data，預設3</param>
    /// <param name="dataStartColumn">從第幾欄開始擷取data，預設1 (通常第0欄用來放驗證錯誤資訊)</param>
    public void SetExcelDataImportPosition(int headerRowIndex, int dataStartRow, int dataStartColumn) => DataImport.SetExcelDataPosition(headerRowIndex, dataStartRow, dataStartColumn);

    /// <summary>
    /// 將excel的資料取出，如需調整資料擷取位置請使用SetExcelDataPosition
    /// </summary>
    public IList<Dictionary<string, object>> GetExcelData() => DataImport.GetExcelData();

    /// <summary>
    /// 在Excel資料匯入前進行驗證，包刮必填、FormulaSQL、唯一值
    /// </summary>
    /// <param name="templates">NPOIExcelTemplate</param>
    /// <param name="checkLists">從FormulaSql中取得的資料限制清單</param>
    /// <param name="pkDuplicateLists">從目標table中查到的的重複資料清單</param>
    public bool ValidateExcelWithNPOITemplate(IList<NPOIExcelTemplate> templates, Dictionary<string, IList<string>> checkLists, Dictionary<string, IList<string>> pkDuplicateLists) => DataImport.ValidateExcelWithNPOITemplate(templates, checkLists, pkDuplicateLists);

    #endregion

    #region DataReplace

    /// <summary>
    /// 取得excel內部的{{欄位名稱}}資料清單
    /// </summary>
    public IList<string> GetReplaceParamListFromExcel() => DataReplace.GetReplaceParamListFromExcel();

    /// <summary>
    /// 把excel內部的{{欄位名稱}}資料做替換，若無符合則會替換為空字串
    /// </summary>
    public void ReplaceDataInExcel<T>(T dataModel) => DataReplace.ReplaceDataInExcel(dataModel);

    #endregion

    public void ExportMemoryStream(MemoryStream ms) => WorkBook.Write(ms);

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        WorkBook = null;
        _fsExcel.Close();
        _fsExcel.Dispose();
    }
}
