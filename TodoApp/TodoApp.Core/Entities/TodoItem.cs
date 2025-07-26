using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Core.Constants;

namespace TodoApp.Core.Entities;

public class TodoItem
{
    [Key]
    public int todo_id { get; set; }

    public string title { get; set; } = string.Empty;

    public string content { get; set; } = string.Empty;

    public string tag { get; set; } = string.Empty;

    public string status { get; set; } = TodoStatus.Pending; // Pending, InProgress, Completed, Cancelled

    public string priority { get; set; } = TodoPriority.Low; // Low, Medium, High

    public DateTime submitted_date { get; set; }

    public DateTime? completed_date { get; set; }

    public DateTime? cancelled_date { get; set; }

    public DateTime created_date { get; set; } = DateTime.Now;

    public DateTime? updated_date { get; set; }

    public DateTime? deleted_date { get; set; }
}