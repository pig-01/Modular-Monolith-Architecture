namespace Base.Domain.SeedWorks.Pagination;

public interface IPaginationRequestBase
{
    int Page { get; set; }
    int PerPage { get; set; }
}
