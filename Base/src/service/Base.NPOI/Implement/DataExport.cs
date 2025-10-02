using System.Reflection;
using Base.Domain.Models.NPOI;
using Base.NPOI.Interface;
using NPOI.SS.UserModel;

namespace Base.NPOI.Implement;

public class DataExport : SetBase, IDataExport
{
    public DataExport(IWorkbook wb) : base(wb) { }

    private int _headerRowIndex = 1;
    private int _dataStartRow = 3;
    private int _dataStartColumn = 1;
    private int _sheetIndex = 0;

    public void SetExcelDataPosition(int headerRowIndex, int dataStartRow, int dataStartColumn, int sheetIndex)
    {
        _headerRowIndex = headerRowIndex;
        _dataStartRow = dataStartRow;
        _dataStartColumn = dataStartColumn;
        _sheetIndex = sheetIndex;
    }

    public void InitDataFields(IList<NPOIDataField> fields)
    {
        ISheet sheet = WorkBook.GetSheetAt(_sheetIndex);
        IRow fieldRow = sheet.CreateRow(_headerRowIndex);
        IRow nameRow = sheet.CreateRow(_headerRowIndex + 1);

        // 建立表頭樣式
        ICellStyle headerStyle = WorkBook.CreateCellStyle();
        headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
        headerStyle.FillPattern = FillPattern.SolidForeground;
        headerStyle.Alignment = HorizontalAlignment.Center;

        for (int i = 0; i < fields.Count; i++)
        {
            ICell fieldCell = fieldRow.CreateCell(_dataStartColumn + i);
            ICell nameCell = nameRow.CreateCell(_dataStartColumn + i);
            SetCellValue(fieldCell, fields[i].Field);
            SetCellValue(nameCell, fields[i].Name);
            fieldCell.CellStyle = headerStyle;
            nameCell.CellStyle = headerStyle;
            // 設置欄位寬度
            sheet.SetColumnWidth(_dataStartColumn + i, fields[i].Width * 256);
        }
    }

    public void ExportDataToExcel<T>(IList<T> datas) where T : class
    {

        ISheet sheet = WorkBook.GetSheetAt(_sheetIndex);
        IRow headerRow = sheet.GetRow(_headerRowIndex);

        List<Dictionary<string, object>> excelData = ConvertToDictionaryList(datas);

        int cellCount = headerRow.LastCellNum;

        for (int i = 0; i < excelData.Count; i++)
        {
            IRow currentRow = sheet.CreateRow(_dataStartRow + i);

            for (int col = _dataStartColumn; col < cellCount; col++)
            {
                ICell cell = currentRow.CreateCell(col);
                string header = headerRow.GetCell(col).ToString();
                object cellValue = excelData[i].TryGetValue(header, out object? value) ? value : null;
                SetCellValue(cell, cellValue);
            }
        }
    }

    private static List<Dictionary<string, object>> ConvertToDictionaryList<T>(IList<T> list) where T : class
    {
        List<Dictionary<string, object>> dictionaryList = new();

        foreach (T item in list)
        {
            if (item is Dictionary<string, object> existingDictionary)
            {
                dictionaryList.Add(existingDictionary);
            }
            else
            {
                Dictionary<string, object> dictionary = new();

                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(item, null);

                    dictionary[propertyName] = propertyValue;
                }

                dictionaryList.Add(dictionary);
            }
        }

        return dictionaryList;
    }

    public void SetCellValueByIndex(int row, int column, object value)
    {
        ISheet sheet = WorkBook.GetSheetAt(_sheetIndex);
        IRow currentRow = sheet.GetRow(row);

        currentRow ??= sheet.CreateRow(row);

        ICell currentCell = currentRow.GetCell(column);

        currentCell ??= currentRow.CreateCell(column);
        SetCellValue(currentCell, value);
    }

    private void SetCellValue(ICell cell, object value)
    {
        if (value is int intValue)
        {
            cell.SetCellValue(intValue);
        }
        else if (value is double doubleValue)
        {
            cell.SetCellValue(doubleValue);
        }
        else if (value is bool boolValue)
        {
            cell.SetCellValue(boolValue);
        }
        else if (value is DateTime dateValue)
        {
            cell.SetCellValue(dateValue);
            ICellStyle dateStyle = WorkBook.CreateCellStyle();
            IDataFormat dateFormat = WorkBook.CreateDataFormat();
            dateStyle.DataFormat = dateFormat.GetFormat("yyyy-MM-dd");
            cell.CellStyle = dateStyle;
        }
        else
        {
            cell.SetCellValue(value?.ToString());
        }
    }
}
