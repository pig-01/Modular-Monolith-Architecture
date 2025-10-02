namespace Base.Domain.Models.NPOI;

public class NPOIDataField
{
    public required string Field { get; set; } // 欄位key
    public required string Name { get; set; } // 欄位名稱
    public int Width { get; set; } // 欄位寬度
}

