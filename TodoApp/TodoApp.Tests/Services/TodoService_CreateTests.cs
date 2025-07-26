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
    public class TodoService_CreateTests
    {
        [Fact]
        public async Task AddAsync_ShouldCreateTodo_WhenValidationSucceeds()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var mockUpdateValidator = new Mock<IValidator<SaveTodoItemDto>>();

            var dto = new SaveTodoItemDto
            {
                title = "New Task",
                content = "Details here",
                tag = "work"
            };

            // Validator returns success
            mockValidator.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new ValidationResult());

            TodoItem capturedEntity = null!;
            mockRepo.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                    .Callback<TodoItem>(e =>
                    {
                        e.todo_id = 42;
                        capturedEntity = e;
                    })
                    .Returns(Task.CompletedTask);

            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockUpdateValidator.Object);

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            Assert.True(result.success);
            Assert.Equal(42, result.data);
            Assert.Equal("Item created successfully.", result.message);
            Assert.Equal("New Task", capturedEntity.title);
            Assert.Equal("work", capturedEntity.tag);
            Assert.Equal("Pending", capturedEntity.status); // default status
        }

        [Fact]
        public async Task AddAsync_ShouldReturnError_WhenValidationFails()
        {
            // Arrange
            var mockRepo = new Mock<ITodoRepository>();
            var mockValidator = new Mock<IValidator<SaveTodoItemDto>>();
            var mockUpdateValidator = new Mock<IValidator<SaveTodoItemDto>>();

            var dto = new SaveTodoItemDto
            {
                title = "", // missing title
                content = "", // missing content
                tag = "" // missing tag
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("title", "Title is required."),
                new ValidationFailure("content", "Content is required."),
                new ValidationFailure("tag", "Tag is required.")
            };

            mockValidator.Setup(v => v.ValidateAsync(dto, default))
                         .ReturnsAsync(new ValidationResult(validationFailures));

            var service = new TodoService(mockRepo.Object, mockValidator.Object, mockUpdateValidator.Object);

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            Assert.False(result.success);
            Assert.Equal("Validation failed.", result.message);
            Assert.NotNull(result.errors);
            Assert.Equal(3, result.errors!.Count);
            Assert.Contains(result.errors, e => e.PropertyName == "title");
            Assert.Contains(result.errors, e => e.PropertyName == "content");
            Assert.Contains(result.errors, e => e.PropertyName == "tag");
            mockRepo.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
        }
    }
}
