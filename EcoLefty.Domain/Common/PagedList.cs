namespace EcoLefty.Domain.Common;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int? totalCount = null)
    {
        pageSize = Math.Max(pageSize, 1);

        TotalCount = totalCount ?? source.Count();
        TotalPages = TotalCount / pageSize;

        if (TotalCount % pageSize > 0)
            TotalPages++;

        PageSize = pageSize;
        PageIndex = pageIndex;

        AddRange(totalCount != null ? source : source.Skip((pageIndex - 1) * pageSize).Take(pageSize));
    }

    public int PageIndex { get; } // 1-based

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => PageIndex > 0;

    public bool HasNextPage => PageIndex + 1 < TotalPages;
}