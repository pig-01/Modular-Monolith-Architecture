using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataHub.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;


namespace DataHub.Domain.AggregatesModel.CustomerAggregate;

[Table("Customer")]
public partial class Customer : IAggregateRoot
{
    [Key]
    [StringLength(36)]
    [Unicode(false)]
    public string CustomerId { get; set; } = null!;
}
