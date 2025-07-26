using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.Filters;

public class TodoFilterModel : FilterModel
{
    public string? tag { get; set; }
    public string? status { get; set; }
    public string? priority { get; set; }
}
