using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.FunctionAggregate;

/// <summary>
/// 以 SCFunctionCode 為唯一值，用來紀錄各 Function 唯一的資料。
/// </summary>
[Table("SCFunctionItem")]
[Index("FunctionCode", Name = "UQ_SCFunctionItem_FunctionCode", IsUnique = true)]
public partial class ScfunctionItem
{
    /// <summary>
    /// Identity
    /// </summary>
    [Key]
    [Column("SCFunctionItemID")]
    public long ScfunctionItemId { get; set; }

    /// <summary>
    /// 功能代號
    /// </summary>
    [StringLength(50)]
    public string? FunctionCode { get; set; }

    /// <summary>
    /// 應用程式代號(FK)
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string? Application { get; set; }

    /// <summary>
    /// 模組代號(FK)
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string? Module { get; set; }
}
