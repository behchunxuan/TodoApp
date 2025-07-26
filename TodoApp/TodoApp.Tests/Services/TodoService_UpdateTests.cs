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
    public class TodoService_UpdateTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldUpdateTodo_WhenValidationSucceeds()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var mockUpdateValidator = new Mock<IValidator<SaveTodoItemDto>>();

            var dto = new SaveTodoItemDto
            {
                todo_id = 1,
                title = "Updated Task",
                content = "Updated Content",
                tag = "personal",
                status = TodoStatus.Pending,
                priority = TodoPriority.High
            };

            var existingItem = new TodoItem
            {
                todo_id = 1,
                title = "Old Task",
                content = "Old Content",
                tag = "old",
                status = TodoStatus.Pending,
                priority = TodoPriority.Medium,
                created_date = DateTime.UtcNow.AddDays(-1)
            };

            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);
            mockUpdateValidator.Setup(v => v.ValidateAsync(dto, default))
                               .ReturnsAsync(new ValidationResult());

            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockUpdateValidator.Object);

            // Act
            var result = await service.UpdateAsync(dto);

            // Assert
            Assert.True(result.success);
            Assert.Equal("Item updated successfully.", result.message);
            Assert.Equal("Updated Task", existingItem.title);
            Assert.Equal("Updated Content", existingItem.content);
            Assert.Equal("personal", existingItem.tag);
            Assert.Equal(TodoStatus.Pending, existingItem.status);
            Assert.Equal(TodoPriority.High, existingItem.priority);
            Assert.NotNull(existingItem.updated_date);
            mockRepo.Verify(r => r.UpdateAsync(existingItem), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenTodoNotFound()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var mockUpdateValidator = new Mock<IValidator<SaveTodoItemDto>>();

            var dto = new SaveTodoItemDto
            {
                todo_id = 99,
                title = "Testing Not Found"
            };

            mockUpdateValidator
                .Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());

            mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TodoItem?)null);
            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockUpdateValidator.Object);

            // Act
            var result = await service.UpdateAsync(dto);

            // Assert
            Assert.False(result.success);
            Assert.Equal("Item not found or already deleted.", result.message);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var mockUpdateValidator = new Mock<IValidator<SaveTodoItemDto>>();

            var dto = new SaveTodoItemDto
            {
                todo_id = 1,
                title = "", // Invalid title
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("title", "Title is required.")
            };

            var existingItem = new TodoItem { todo_id = 1, title = "Old" };
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);
            mockUpdateValidator.Setup(v => v.ValidateAsync(dto, default))
                               .ReturnsAsync(new ValidationResult(validationFailures));

            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockUpdateValidator.Object);

            // Act
            var result = await service.UpdateAsync(dto);

            // Assert
            Assert.False(result.success);
            Assert.Equal("Validation failed.", result.message);
            Assert.Single(result.errors!);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
        }
    }
}
