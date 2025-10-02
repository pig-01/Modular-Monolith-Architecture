using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.OrderAggregate;

public partial class Order : IAggregateRoot
{
    [Key]
    public long OrderId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PricingCode { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [Column("SDate")]
    public DateOnly? Sdate { get; set; }

    [Column("EDate")]
    public DateOnly? Edate { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    [InverseProperty("Order")]
    public virtual ICollection<OrderTenant> OrderTenants { get; set; } = [];

    public static Order Create(string pricingCode, DateTime orderDate, DateTime startDate, DateTime endDate) => new()
    {
        PricingCode = pricingCode,
        OrderDate = orderDate,
        Sdate = new DateOnly(startDate.Year, startDate.Month, startDate.Day),
        Edate = new DateOnly(endDate.Year, endDate.Month, endDate.Day)
    };
}
