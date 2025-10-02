namespace DataHub.Cloud.Models.Provision;

public class AddOnOrder
{
    public required string CustomerId { get; set; }
    public required string Email { get; set; }
    public required string TenantID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Years { get; set; }
    public required IList<AddOnOdrerItem> OrderItems { get; set; }
}

public enum OrderTyp
{
    AddOn = 1,
    Renew = 2,
}

public class AddOnOdrerItem
{
    public required string Module { get; set; }
    public OrderTyp OrderType { get; set; }
    public int AddOnEmp { get; set; }
}
