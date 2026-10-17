namespace BeeMart.WebClient.Models;

/// <summary>
/// Generic pagination envelope matching the API's paged responses
/// (History Transaction: max 20 items per page).
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalCount / (double)PageSize)
        : 0;

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
