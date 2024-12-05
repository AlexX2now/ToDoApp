using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _todoService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var todoItem = await _todoService.GetByIdAsync(id);
            return todoItem == null ? NotFound() : Ok(todoItem);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TodoItem todoItem)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdItem = await _todoService.CreateAsync(todoItem);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TodoItem todoItem)
        {
            if (id != todoItem.Id) return BadRequest();
            var existingItem = await _todoService.GetByIdAsync(id);
            if (existingItem == null) return NotFound();
            await _todoService.UpdateAsync(todoItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _todoService.DeleteAsync(id);
            return NoContent();
        }
    }
}
