namespace Base.Domain.Models.NPOI;

public class NPOIExcelTemplate
{
    public required string Type { get; set; } // Template種類
    public required string Field { get; set; } // 欄位
    public bool IsRequired { get; set; } // 是否必填
    public bool IsPk { get; set; } // 是否不可重複
    public required string FormulaSql { get; set; } // SQL比對清單
    public required string AutoFillValue { get; set; } // 匯入時自動填充的欄位值
    public DateTime? CreatedDate { get; set; } // 建立日期
    public required string CreatedUser { get; set; } // 建立人員
    public DateTime? ModifiedDate { get; set; } // 異動日期
    public required string ModifiedUser { get; set; } // 異動人員
}

