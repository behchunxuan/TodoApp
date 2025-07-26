using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.DTOs;
using TodoApp.Application.Filters;
using TodoApp.Application.Services;
using TodoApp.Core.Constants;
using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;
using Xunit;

namespace TodoApp.Tests.Services
{
    public class TodoService_GetPagedTests
    {
        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedItems()
        {
            // Arrange
            var data = new List<TodoItem>
            {
                new() { todo_id = 1, title = "Task 1", status = TodoStatus.Pending, submitted_date = DateTime.Now },
                new() { todo_id = 2, title = "Task 2", status = TodoStatus.Pending, submitted_date = DateTime.Now },
                new() { todo_id = 3, title = "Task 3", status = TodoStatus.Pending, submitted_date = DateTime.Now },
                new() { todo_id = 4, title = "Task 4", status = TodoStatus.Pending, submitted_date = DateTime.Now }
            }.AsQueryable();

            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(r => r.Query()).Returns(data);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            var filter = new TodoFilterModel
            {
                page = 1,
                page_size = 2
            };

            // Act
            var result = await service.GetPagedAsync(filter);

            // Assert
            Assert.True(result.success);
            Assert.NotNull(result.data);

            // ✅ Cast to List<TodoItemDto>
            var items = result.data as List<TodoItemDto>;
            Assert.NotNull(items);
            Assert.Equal(2, items!.Count); // Two items for page 1

            Assert.Equal(1, result.current_page);
            Assert.Equal(2, result.page_size);
            Assert.Equal(4, result.total_records);
            Assert.Equal(2, result.total_pages); // 4 / 2 = 2
        }
    }
}
