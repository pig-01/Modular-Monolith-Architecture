using System;

namespace Main.Dto.ViewModel.PlanSearch;

//DB中View Table 的 DTO
public class ViewCrossPlanSearch
{
    public int PlanID { get; set; }
    public string? PlanName { get; set; }
    public int PlanYear { get; set; }
    public int? CompanyID { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyCode { get; set; }
    public int? AreaID { get; set; }
    public string? AreaName { get; set; }
    public string? AreaCode { get; set; }
    public int? PlanDetailID { get; set; }
    public string? PlanDetailName { get; set; }
    public string? PlanDetailChName { get; set; }
    public string? PlanDetailEnName { get; set; }
    public string? PlanDetailJpName { get; set; }
    public string? CycleType { get; set; }
    public int? CycleNumber { get; set; }
    public string? GroupID { get; set; }
    public string? RowNumber { get; set; }
    public string? RowIdNumber { get; set; }
    public int? PlanDocumentDataID { get; set; }
    public string? FieldName { get; set; }
    public string? FieldValue { get; set; }
    public string? FieldType { get; set; }
    public string? Unit { get; set; }
    public string? CustomName { get; set; }
    public string? DataAreaName { get; set; }
    // public bool? HasViewPermission { get; set; } // 測試 檢視權限是否正常
}
