using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Core.Constants;
using TodoApp.Core.Entities;
using TodoApp.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        // Skip seeding if data exists
        if (context.TodoItems.Any()) return;

        var now = DateTime.Now;

        var todos = new[]
        {
            new TodoItem { title = "Seed - Buy groceries", content = "Milk, eggs, bread, and vegetables.", tag = "personal", priority = "High", status = TodoStatus.Pending, submitted_date = now },
            new TodoItem { title = "Seed - Finish project", content = "Finalize and submit the client project.", tag = "work", priority = "Medium", status = TodoStatus.Completed, submitted_date = now.AddDays(-2) },
            new TodoItem { title = "Seed - Call plumber", content = "Fix leaking kitchen sink.", tag = "home", priority = "Low", status = TodoStatus.Pending, submitted_date = now.AddDays(-1) },
            new TodoItem { title = "Seed - Fix bugs", content = "Resolve reported UI issues and test again.", tag = "work", priority = "High", status = TodoStatus.Pending, submitted_date = now.AddDays(-3) },
            new TodoItem { title = "Seed - Plan birthday party", content = "Book venue, order cake, and send invites.", tag = "personal", priority = "Medium", status = TodoStatus.Completed, submitted_date = now.AddDays(-10) },
            new TodoItem { title = "Seed - Dentist appointment", content = "Routine checkup at 10am.", tag = "health", priority = "High", status = TodoStatus.Cancelled, submitted_date = now.AddDays(-5) },
            new TodoItem { title = "Seed - Read a book", content = "Finish reading 'Atomic Habits'.", tag = "leisure", priority = "Low", status = TodoStatus.Pending, submitted_date = now.AddDays(-7) },
            new TodoItem { title = "Seed - Submit tax returns", content = "Upload documents and submit via e-filing.", tag = "finance", priority = "High", status = TodoStatus.Completed, submitted_date = now.AddDays(-15) },
            new TodoItem { title = "Seed - Team meeting", content = "Discuss sprint progress and blockers.", tag = "work", priority = "Medium", status = TodoStatus.Pending, submitted_date = now.AddDays(-4) },
            new TodoItem { title = "Seed - Laundry", content = "Wash and fold clothes.", tag = "home", priority = "Low", status = TodoStatus.Completed, submitted_date = now.AddDays(-6) },
            new TodoItem { title = "Seed - Workout", content = "Gym session: cardio and weights.", tag = "health", priority = "Medium", status = TodoStatus.Cancelled, submitted_date = now.AddDays(-2) },
            new TodoItem { title = "Seed - Grocery shopping", content = "Weekly grocery run at Tesco.", tag = "personal", priority = "Low", status = TodoStatus.Pending, submitted_date = now },
            new TodoItem { title = "Seed - Water plants", content = "Water all indoor and balcony plants.", tag = "home", priority = "Low", status = TodoStatus.Completed, submitted_date = now.AddMonths(-1) },
            new TodoItem { title = "Seed - Watch webinar", content = "Join the free webinar on software testing.", tag = "learning", priority = "Medium", status = TodoStatus.Completed, submitted_date = now.AddMonths(-1) },
            new TodoItem { title = "Seed - Pay bills", content = "Electricity, water, and internet bills due.", tag = "finance", priority = "High", status = TodoStatus.Cancelled, submitted_date = now.AddMonths(-1) },
        };

        context.TodoItems.AddRange(todos);
        context.SaveChanges();
    }
}