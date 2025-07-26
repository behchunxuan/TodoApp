using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Application.DTOs;
using TodoApp.Application.Services;
using TodoApp.Core.Constants;
using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;
using Xunit;

namespace TodoApp.Tests.Services
{
    public class TodoService_GetByIdTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnItem_WhenFound()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var dummyItem = new TodoItem { todo_id = 1, title = "Test Task", status = TodoStatus.Pending };

            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dummyItem);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.True(result.success);
            Assert.NotNull(result.data);
            Assert.Equal(1, result.data.todo_id);
            Assert.Equal("Test Task", result.data.title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnError_WhenNotFound()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((TodoItem?)null);

            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockValidator.Object);

            // Act
            var result = await service.GetByIdAsync(123);

            // Assert
            Assert.False(result.success);
            Assert.Null(result.data);
            Assert.Equal("Item not found.", result.message);
        }
    }
}
