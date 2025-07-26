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
    public class TodoService_DeleteTests
    {
        [Fact]
        public async Task DeleteAsync_ShouldSoftDelete_WhenValid()
        {
            var item = new TodoItem { todo_id = 1, status = TodoStatus.Pending };
            var repo = new Mock<ITodoRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var validator = new Mock<IValidator<SaveTodoItemDto>>();
            var service = new TodoService(repo.Object, validator.Object, validator.Object);

            var result = await service.DeleteAsync(1);

            Assert.True(result.success);
            Assert.NotNull(item.deleted_date);
        }
    }
}