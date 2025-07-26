using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.Filters;

public class FilterModel
{
    public int page { get; set; } = 1;
    public int page_size { get; set; } = 10;
    public string? search_text { get; set; }
    

    // Sorting
    public string? sort_field { get; set; } 
    public string? sort_direction { get; set; } = "asc";


    // Filter by year and month
    public int year { get; set; } = 0;
    public int month { get; set; } = 0;
}
