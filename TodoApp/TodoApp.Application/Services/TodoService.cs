using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.DTOs;
using TodoApp.Application.Filters;
using TodoApp.Application.Wrappers;
using TodoApp.Core.Constants;
using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;

namespace TodoApp.Application.Services
{
    public class TodoService
    {
        private readonly ITodoRepository _repository;
        private readonly IValidator<SaveTodoItemDto> _createValidator;
        private readonly IValidator<SaveTodoItemDto> _updateValidator;
        public TodoService(
            ITodoRepository repository,
            IValidator<SaveTodoItemDto> createValidator,
            IValidator<SaveTodoItemDto> updateValidator)
        {
            _repository = repository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        private static TodoItemDto ToDto(TodoItem item)
        {
            return new TodoItemDto
            {
                todo_id = item.todo_id,
                title = item.title,
                content = item.content,
                tag = item.tag,
                status = item.status,
                priority = item.priority,
                submitted_date = item.submitted_date,
                completed_date = item.completed_date,
                cancelled_date = item.cancelled_date
            };
        }
        private static TodoItem ToEntity(SaveTodoItemDto dto)
        {
            return new TodoItem
            {
                title = dto.title,
                content = dto.content,
                tag = dto.tag,
                status = string.IsNullOrWhiteSpace(dto.status) ? TodoStatus.Pending : dto.status,
                priority = string.IsNullOrWhiteSpace(dto.priority) ? TodoPriority.Medium : dto.priority,
                submitted_date = DateTime.Now
            };
        }

        private static IQueryable<TodoItem> ApplyFilter(IQueryable<TodoItem> query, TodoFilterModel filter)
        {
            query = query.Where(i => i.deleted_date == null);

            if (!string.IsNullOrWhiteSpace(filter.status))
                query = query.Where(i => i.status == filter.status);

            if (!string.IsNullOrWhiteSpace(filter.priority))
                query = query.Where(i => i.priority == filter.priority);

            if (!string.IsNullOrWhiteSpace(filter.tag))
                query = query.Where(i => i.tag == filter.tag);

            if (filter.year > 0)
                query = query.Where(i => i.submitted_date.Year == filter.year);

            if (filter.month > 0)
                query = query.Where(i => i.submitted_date.Month == filter.month);

            if (!string.IsNullOrWhiteSpace(filter.search_text))
            {
                string keyword = filter.search_text.ToLower();
                query = query.Where(i =>
                    i.title.ToLower().Contains(keyword) ||
                    i.content.ToLower().Contains(keyword) ||
                    i.tag.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(filter.sort_field))
            {
                bool descending = filter.sort_direction?.ToLower() == "desc";

                query = filter.sort_field switch
                {
                    "priority" => descending
                        ? query.OrderByDescending(i =>
                            i.priority == "High" ? 3 :
                            i.priority == "Medium" ? 2 :
                            i.priority == "Low" ? 1 : 0)
                        : query.OrderBy(i =>
                            i.priority == "High" ? 3 :
                            i.priority == "Medium" ? 2 :
                            i.priority == "Low" ? 1 : 0),

                    "submittedDate" => descending
                        ? query.OrderByDescending(i => i.submitted_date)
                        : query.OrderBy(i => i.submitted_date),

                    _ => query.OrderByDescending(i => i.submitted_date)
                };
            }
            else
            {
                query = query.OrderByDescending(i => i.submitted_date);
            }

            return query;
        }

        public async Task<PagedResponse<List<TodoItemDto>>> GetPagedAsync(TodoFilterModel filter)
        {
            var query = _repository.Query();

            query = ApplyFilter(query, filter);

            var total = query.Count();

            var items = query
                .AsEnumerable()
                .OrderByDescending(i => GetPriorityOrder(i.priority))
                .ThenByDescending(i => i.submitted_date)
                .Skip((filter.page - 1) * filter.page_size)
                .Take(filter.page_size)
                .Select(ToDto)
                .ToList();

            return new PagedResponse<List<TodoItemDto>>(items, filter.page, filter.page_size, total);
        }

        public async Task<ApiResponse<List<TodoItemDto>>> GetListAsync(TodoFilterModel filter)
        {
            var query = _repository.Query();

            filter.sort_field ??= "submittedDate";
            filter.sort_direction ??= "desc";

            query = ApplyFilter(query, filter);

            var items = query
                .Select(ToDto)
                .ToList();                                    

            return ApiResponse<List<TodoItemDto>>.Success(items);
        }

        public async Task<ApiResponse<TodoItemDto>> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || item.deleted_date != null)
                return ApiResponse<TodoItemDto>.Fail("Item not found.");

            return ApiResponse<TodoItemDto>.Success(ToDto(item));
        }

        public async Task<ApiResponse<int>> AddAsync(SaveTodoItemDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ApiResponse<int>.Fail("Validation failed.", validation.Errors);

            dto.status = TodoStatus.Pending;
            var entity = ToEntity(dto);
            await _repository.AddAsync(entity);
            return ApiResponse<int>.Success(entity.todo_id, "Item created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(SaveTodoItemDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ApiResponse<bool>.Fail("Validation failed.", validation.Errors);

            if (dto.todo_id == null || dto.todo_id == 0)
                return ApiResponse<bool>.Fail("Missing ID.");

            var item = await _repository.GetByIdAsync(dto.todo_id.Value);
            if (item == null || item.deleted_date != null)
                return ApiResponse<bool>.Fail("Item not found or already deleted.");

            if (item.status == TodoStatus.Cancelled)
                return ApiResponse<bool>.Fail("Cannot update a cancelled item.");

            item.title = string.IsNullOrWhiteSpace(dto.title) ? item.title : dto.title;
            item.content = string.IsNullOrWhiteSpace(dto.content) ? item.content : dto.content;
            item.tag = string.IsNullOrWhiteSpace(dto.tag) ? item.tag : dto.tag;
            item.status = dto.status ?? item.status;
            item.priority = dto.priority ?? item.priority;

            item.updated_date = DateTime.Now;

            await _repository.UpdateAsync(item);
            return ApiResponse<bool>.Success(true, "Item updated successfully.");
        }

        public async Task<ApiResponse<bool>> CompleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || item.deleted_date != null)
                return ApiResponse<bool>.Fail("Item not found or already deleted.");

            if (item.status == TodoStatus.Completed)
                return ApiResponse<bool>.Fail("Item already completed.");

            if (item.status == TodoStatus.Cancelled)
                return ApiResponse<bool>.Fail("Cannot complete a cancelled item.");

            item.status = TodoStatus.Completed;
            item.completed_date = DateTime.Now;

            await _repository.UpdateAsync(item);
            return ApiResponse<bool>.Success(true, "Item marked as completed.");
        }

        public async Task<ApiResponse<bool>> CancelAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || item.deleted_date != null)
                return ApiResponse<bool>.Fail("Item not found or already deleted.");

            if (item.status == TodoStatus.Cancelled)
                return ApiResponse<bool>.Fail("Item already cancelled.");

            if (item.status == TodoStatus.Completed)
                return ApiResponse<bool>.Fail("Completed items cannot be cancelled.");

            item.status = TodoStatus.Cancelled;
            item.cancelled_date = DateTime.Now;
            item.updated_date = DateTime.Now;

            await _repository.UpdateAsync(item);
            return ApiResponse<bool>.Success(true, "Item cancelled successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null || item.deleted_date != null)
                return ApiResponse<bool>.Fail("Item not found or already deleted.");

            if (item.status == TodoStatus.Completed)
                return ApiResponse<bool>.Fail("Cannot delete a completed task.");

            item.deleted_date = DateTime.Now;
            item.updated_date = DateTime.Now;

            await _repository.UpdateAsync(item);
            return ApiResponse<bool>.Success(true, "Item deleted successfully.");
        }

        private static int GetPriorityOrder(string priority)
        {
            return priority switch
            {
                TodoPriority.High => 3,
                TodoPriority.Medium => 2,
                TodoPriority.Low => 1,
                _ => 0
            };
        }

    }
}
