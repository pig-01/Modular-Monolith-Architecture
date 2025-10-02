using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.FunctionAggregate;

[Table("SCFunction")]
[Index("ParentFunctionId", Name = "IDX_SCFunction_1")]
[Index("FunctionCode", "TenantId", Name = "UQ_SCFunction_FunctionCode", IsUnique = true)]
public partial class Scfunction
{
    /// <summary>
    /// 功能項目代號
    /// </summary>
    [Key]
    [Column("FunctionID")]
    public long FunctionId { get; set; }

    /// <summary>
    /// 功能代號
    /// </summary>
    [StringLength(50)]
    public string FunctionCode { get; set; } = null!;

    /// <summary>
    /// 繁體中文名稱
    /// </summary>
    [Column("zhCHTName")]
    [StringLength(100)]
    public string? ZhChtname { get; set; }

    /// <summary>
    /// 英文名稱
    /// </summary>
    [Column("enUSName")]
    [StringLength(100)]
    public string? EnUsname { get; set; }

    /// <summary>
    /// 簡體中文名稱
    /// </summary>
    [Column("zhCHSName")]
    [StringLength(100)]
    public string? ZhChsname { get; set; }

    /// <summary>
    /// 日文名稱
    /// </summary>
    [Column("jaJPName")]
    [StringLength(100)]
    public string? JaJpname { get; set; }

    /// <summary>
    /// 功能項目描述
    /// </summary>
    [StringLength(255)]
    public string? FunctionDesc { get; set; }

    /// <summary>
    /// 功能項目種類
    /// </summary>
    [StringLength(50)]
    public string? FunctionKind { get; set; }

    /// <summary>
    /// 功能類別 1=功能 2=資料夾 3=檔案
    /// </summary>
    [StringLength(1)]
    public string FunctionType { get; set; } = null!;

    /// <summary>
    /// 是否要出現在功能目錄
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// 功能執行目錄
    /// </summary>
    [StringLength(200)]
    public string? ProgramPath { get; set; }

    /// <summary>
    /// 父功能項目代號
    /// </summary>
    [Column("ParentFunctionID")]
    public long? ParentFunctionId { get; set; }

    /// <summary>
    /// 功能圖示
    /// </summary>
    [StringLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? SortSequence { get; set; }

    /// <summary>
    /// TenantID
    /// </summary>
    [Column("TenantID")]
    [StringLength(20)]
    [Unicode(false)]
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// 建檔時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 建檔人員
    /// </summary>
    [StringLength(50)]
    public string CreatedUser { get; set; } = null!;

    /// <summary>
    /// 最後修改時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 最後修改人員
    /// </summary>
    [StringLength(50)]
    public string? ModifiedUser { get; set; }

    /// <summary>
    /// 頁籤是否延遲載入
    /// </summary>
    public bool? IsLazyLoad { get; set; }

    /// <summary>
    /// 是否關閉
    /// </summary>
    public bool Disabled { get; set; }
}
