using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.Wrappers;

public class PagedResponse<T> : ApiResponse<T>
{
    public int current_page { get; set; }
    public int page_size { get; set; }
    public int total_pages { get; set; }
    public int total_records { get; set; }

    public PagedResponse(T? data, int current_page, int page_size, int total_records, string? message = null)
    {
        this.success = true;
        this.message = message ?? string.Empty;
        this.data = data!;
        this.current_page = current_page;
        this.page_size = page_size;
        this.total_records = total_records;
        this.total_pages = (int)Math.Ceiling(total_records / (double)page_size);
    }
}