using System.Reflection;
using Base.NPOI.Interface;
using NPOI.SS.UserModel;

namespace Base.NPOI.Implement;

public partial class DataReplace : SetBase, IDataReplace
{
    public DataReplace(IWorkbook wb) : base(wb) { }


    public IList<string> GetReplaceParamListFromExcel()
    {
        ISheet sheet = WorkBook.GetSheetAt(0);
        IList<string> result = [];
        foreach (IRow row in sheet)
        {
            foreach (ICell cell in row.Cells)
            {
                if (cell.CellType == CellType.String && cell.StringCellValue.StartsWith("{{") && cell.StringCellValue.EndsWith("}}"))
                {
                    result.Add(cell.StringCellValue.Replace("{{", "").Replace("}}", ""));
                }
            }
        }

        return result;
    }

    public void ReplaceDataInExcel<T>(T dataModel)
    {
        // 將物件轉為 Dictionary
        ISheet sheet = WorkBook.GetSheetAt(0);
        Dictionary<string, object> dataDict = ToDictionary(dataModel);

        foreach (IRow row in sheet)
        {
            foreach (ICell cell in row.Cells)
            {
                if (cell.CellType == CellType.String && cell.StringCellValue.Contains("{{"))
                {
                    string original = cell.StringCellValue;
                    string replaced = ReplacePlaceholders(original, dataDict);
                    cell.SetCellValue(replaced);
                }
            }
        }
    }


    private static string ReplacePlaceholders(string input, Dictionary<string, object> data)
    {
        if (string.IsNullOrEmpty(input) || !input.Contains("{{"))
            return input;

        string result = "";
        System.Text.RegularExpressions.MatchCollection matches = MyRegex().Matches(input);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            string placeholder = match.Value;          // {{Key}}
            string key = match.Groups[1].Value.Trim(); // Key

            if (data.TryGetValue(key, out object? value) && value != null)
            {
                result = value.ToString();
            }
        }

        return result;
    }

    private static Dictionary<string, object> ToDictionary<T>(T model)
    {
        Dictionary<string, object> dict = new();
        if (model is Dictionary<string, object> d)
            return d;

        foreach (PropertyInfo prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            dict[prop.Name] = prop.GetValue(model);
        }
        return dict;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\{\{(.*?)\}\}")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}
