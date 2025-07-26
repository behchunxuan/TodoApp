using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.DTOs;

public class SaveTodoItemDto
{
    public int? todo_id { get; set; } // nullable, so POST can skip it
    public string title { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
    public string tag { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public string priority { get; set; } = string.Empty;
}