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
    public class TodoService_CompleteTests
    {
        [Fact]
        public async Task CompleteAsync_ShouldSetCompleted_WhenValid()
        {
            var item = new TodoItem { todo_id = 1, status = TodoStatus.Pending };
            var repo = new Mock<ITodoRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var validator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(repo.Object, validator.Object, validator.Object);

            var result = await service.CompleteAsync(1);

            Assert.True(result.success);
            Assert.Equal(TodoStatus.Completed, item.status);
            Assert.NotNull(item.completed_date);
        }

        [Fact]
        public async Task CompleteAsync_ShouldFail_WhenAlreadyCompleted()
        {
            var item = new TodoItem { todo_id = 1, status = TodoStatus.Completed };
            var repo = new Mock<ITodoRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var validator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(repo.Object, validator.Object, validator.Object);

            var result = await service.CompleteAsync(1);

            Assert.False(result.success);
            Assert.Equal("Item already completed.", result.message);
        }

        [Fact]
        public async Task CompleteAsync_ShouldFail_WhenCancelled()
        {
            var item = new TodoItem { todo_id = 1, status = TodoStatus.Cancelled };
            var repo = new Mock<ITodoRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var validator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(repo.Object, validator.Object, validator.Object);

            var result = await service.CompleteAsync(1);

            Assert.False(result.success);
            Assert.Equal("Cannot complete a cancelled item.", result.message);
        }
    }
}
