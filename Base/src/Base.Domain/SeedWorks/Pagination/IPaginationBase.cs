namespace Base.Domain.SeedWorks.Pagination;

public interface IPaginationBase<T>
{
    int TotalCount { get; set; }

    abstract List<T> List { get; set; }
}
