namespace Base.NPOI.Interface;

public interface IDataReplace
{

    /// <summary>
    /// 取得excel內部的{{欄位名稱}}資料清單
    /// </summary>
    IList<string> GetReplaceParamListFromExcel();

    /// <summary>
    /// 把excel內部的{{欄位名稱}}資料做替換，若無符合則會替換為空字串
    /// </summary>
    void ReplaceDataInExcel<T>(T dataModel);

}
