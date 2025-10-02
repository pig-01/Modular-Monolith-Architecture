using Base.Domain.Models.NPOI;
using Base.NPOI.Interface;
using NPOI.SS.UserModel;

namespace Base.NPOI.Implement;

public class DataImport : SetBase, IDataImport
{
    public DataImport(IWorkbook wb) : base(wb) { }

    private int _headerRowIndex = 1;
    private int _dataStartRow = 3;
    private int _dataStartColumn = 1;

    public void SetExcelDataPosition(int headerRowIndex, int dataStartRow, int dataStartColumn)
    {
        _headerRowIndex = headerRowIndex;
        _dataStartRow = dataStartRow;
        _dataStartColumn = dataStartColumn;
    }

    public IList<Dictionary<string, object>> GetExcelData()
    {

        ISheet sheet = this.WorkBook.GetSheetAt(0);
        IRow headerRow = sheet.GetRow(_headerRowIndex);

        List<Dictionary<string, object>> excelData = new List<Dictionary<string, object>>();

        int cellCount = headerRow.LastCellNum;

        for (int row = _dataStartRow; row <= sheet.LastRowNum; row++)
        {
            IRow currentRow = sheet.GetRow(row);
            if (currentRow == null) continue;

            Dictionary<string, object> rowData = new Dictionary<string, object>();

            for (int col = _dataStartColumn; col < cellCount; col++)
            {
                ICell cell = currentRow.GetCell(col);
                string header = headerRow.GetCell(col).ToString();
                object cellValue = GetCellValue(cell);

                rowData[header] = cellValue;
            }

            // 判斷至少要有一格有值，不然會擷取到空的ROW
            if (rowData.Values.Any(value => value != null))
            {
                excelData.Add(rowData);
            }
        }
        return excelData;
    }

    public bool ValidateExcelWithNPOITemplate(IList<NPOIExcelTemplate> templates, Dictionary<string, IList<string>> checkLists, Dictionary<string, IList<string>> pkDuplicateLists)
    {

        // 取得excel資料
        var excelDatas = GetExcelData();

        // 設定錯誤訊息的cell樣式
        ISheet sheet = this.WorkBook.GetSheetAt(0);
        IFont font = this.WorkBook.CreateFont();
        ICellStyle errorCellStyle = this.WorkBook.CreateCellStyle();
        font.Color = IndexedColors.Red.Index;
        errorCellStyle.SetFont(font);

        // 是否驗證通過，如果在驗證中遇到錯誤，就會被設為false
        bool isValid = true;

        for (int i = 0; i < excelDatas.Count; i++)
        {
            var errorMessage = "";
            for (int j = 0; j < templates.Count; j++)
            {
                // 取得field、value及checkList後，放入ValidateCurrentCell做驗證，若有錯誤則errorMessage會有值，代表驗證沒過
                var field = templates[j].Field;
                var value = excelDatas[i].ContainsKey(field) ? excelDatas[i][field] : null;
                var checkList = checkLists.ContainsKey(field) ? checkLists[field] : null;
                var pkDuplicateList = pkDuplicateLists.ContainsKey(field) ? pkDuplicateLists[field] : null;
                errorMessage += ValidateCurrentCell(templates[j], value, checkList, pkDuplicateList, excelDatas);
            }

            // 驗證沒過的話，將錯誤資訊塞入資料所在row的第0欄，並設定驗證未過
            if (errorMessage.Length > 0)
            {
                isValid = false;
            }
            IRow row = sheet.GetRow(_dataStartRow + i);
            ICell cell = row.GetCell(0);
            if (cell == null) cell = row.CreateCell(0);
            cell.CellStyle = errorCellStyle;
            cell.SetCellValue(errorMessage);
        }

        return isValid;
    }


    /// <summary>
    /// 根據template做cell值的驗證判斷
    /// </summary>
    private string ValidateCurrentCell(NPOIExcelTemplate template, object value, IList<string> checkList, IList<string> pkDuplicateList, IList<Dictionary<string, object>> excelDatas)
    {

        string result = "";
        // 驗證value的必填
        if (template.IsRequired && !(value?.ToString()?.Length > 0))
        {
            result += template.Field + "不能為空, ";
        }

        // 驗證value是否符合FormulaSQL拉回來的資料清單
        if (checkList?.Count > 0 && !checkList.Contains(value?.ToString()))
        {
            result += template.Field + "的值不在限制範圍內, ";
        }

        // 驗證value是否重複
        if (template.IsPk)
        {
            var excelValueList = excelDatas.Select(o => o[template.Field]).Where(item => item != null).ToList();
            if (excelValueList.GroupBy(x => x).Where(g => g.Key.Equals(value?.ToString())).Any(g => g.Count() > 1) ||
                (pkDuplicateList?.Count > 0 && pkDuplicateList.Contains(value.ToString()))
            )
            {
                result += template.Field + "的值不能重複, ";
            }
        }

        return result;
    }

    private object? GetCellValue(ICell cell)
    {
        if (cell == null)
            return null;

        switch (cell.CellType)
        {
            case CellType.Numeric:
                return (DateUtil.IsCellDateFormatted(cell) ? (object)cell.DateCellValue : cell.NumericCellValue);
            case CellType.String:
                return cell.StringCellValue;
            case CellType.Boolean:
                return cell.BooleanCellValue;
            case CellType.Formula:
                return cell.CellFormula;
            default:
                return cell.ToString();
        }
    }
}
