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
    public class TodoService_GetListTests
    {
        [Fact]
        public async Task GetListAsync_ShouldReturnFilteredList_WhenMatchFound()
        {
            // Arrange
            var data = new List<TodoItem>
        {
            new() { todo_id = 1, title = "Work task", status = TodoStatus.Pending, tag = "work" },
            new() { todo_id = 2, title = "Home task", status = TodoStatus.Completed, tag = "home" },
            new() { todo_id = 3, title = "Work update", status = TodoStatus.Pending, tag = "work" }
        }.AsQueryable();

            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(r => r.Query()).Returns(data);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            var filter = new TodoFilterModel { tag = "work" };

            // Act
            var result = await service.GetListAsync(filter);

            // Assert
            Assert.True(result.success);
            Assert.NotNull(result.data);
            Assert.Equal(2, result.data!.Count);
            Assert.All(result.data, item => Assert.Equal("work", item.tag));
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnEmptyList_WhenNoMatch()
        {
            // Arrange
            var data = new List<TodoItem>
        {
            new() { todo_id = 1, title = "Task A", tag = "a" },
            new() { todo_id = 2, title = "Task B", tag = "b" }
        }.AsQueryable();

            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(r => r.Query()).Returns(data);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            var filter = new TodoFilterModel { tag = "notfound" };

            // Act
            var result = await service.GetListAsync(filter);

            // Assert
            Assert.True(result.success);
            Assert.NotNull(result.data);
            Assert.Empty(result.data);
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnFilteredList_WhenSearchMatchesTitle()
        {
            // Arrange
            var data = new List<TodoItem>
            {
                new() { todo_id = 1, title = "Meeting notes", tag = "work" },
                new() { todo_id = 2, title = "Shopping list", tag = "personal" },
                new() { todo_id = 3, title = "Team meeting summary", tag = "work" }
            }.AsQueryable();

            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(r => r.Query()).Returns(data);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            var filter = new TodoFilterModel { search_text = "meeting" };

            // Act
            var result = await service.GetListAsync(filter);

            // Assert
            Assert.True(result.success);
            Assert.Equal(2, result.data!.Count);
            Assert.All(result.data, item => Assert.Contains("meeting", item.title, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnSortedList_BySubmittedDateDescending()
        {
            // Arrange
            var data = new List<TodoItem>
            {
                new() { todo_id = 1, title = "Task A", submitted_date = new DateTime(2025, 4, 1) },
                new() { todo_id = 2, title = "Task B", submitted_date = new DateTime(2025, 8, 1) },
                new() { todo_id = 3, title = "Task C", submitted_date = new DateTime(2025, 6, 1) }
            }.AsQueryable();

            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(r => r.Query()).Returns(data);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            var filter = new TodoFilterModel { sort_field = "submitted_date", sort_direction = "desc" };

            // Act
            var result = await service.GetListAsync(filter);

            // Assert
            Assert.True(result.success);
            var sorted = result.data!;
            Assert.Equal(3, sorted.Count);
            Assert.True(sorted[0].submitted_date >= sorted[1].submitted_date);
            Assert.True(sorted[1].submitted_date >= sorted[2].submitted_date);
        }
    }
}
