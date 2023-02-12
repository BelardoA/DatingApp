namespace API.Helpers;

public class PaginationHeader
{
    public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPage)
    {
        CurrentPage = currentPage;
        ItemsPerPage = itemsPerPage;
        TotalItems = totalItems;
        TotalPage = totalPage;
    }

    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPage { get; set; }
}