using System.Collections.Generic;

namespace Sales.API.Models;


public class PaginatedResult<T>
{
    public IEnumerable<T> Data { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}