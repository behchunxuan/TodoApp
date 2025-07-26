using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Filters;
using TodoApp.Application.Services;
using TodoApp.Application.Wrappers;
using TodoApp.Core.Constants;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        //private readonly ILogger<TodoController> _logger;
        private readonly TodoService _todoService;

        public TodoController(TodoService service)
        {
            _todoService = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetListAsync([FromBody] TodoFilterModel filter)
        {
            var result = await _todoService.GetListAsync(filter);
            return Ok(result);
        }

        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedAsync([FromBody] TodoFilterModel filter)
        {
            var result = await _todoService.GetPagedAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _todoService.GetByIdAsync(id);
            if (!result.success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] SaveTodoItemDto dto)
        {
            var result = await _todoService.AddAsync(dto);
            return result.success ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] SaveTodoItemDto dto)
        {
            var result = await _todoService.UpdateAsync(dto);
            return result.success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _todoService.DeleteAsync(id);
            return result.success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("complete/{id}")]
        public async Task<IActionResult> CompleteAsync(int id)
        {
            var result = await _todoService.CompleteAsync(id);
            return result.success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelAsync(int id)
        {
            var result = await _todoService.CancelAsync(id);
            return result.success ? Ok(result) : BadRequest(result);
        }
    }
}
