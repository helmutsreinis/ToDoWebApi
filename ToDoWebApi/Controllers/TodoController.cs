using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyTodoApp.Models;
using System.Linq;
using ToDoWebApi.Models;
using MyTodoApp.Models;
using System.Linq;
namespace MyTodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly FeatureToggles _featureToggles;
        public TodoController(TodoContext context, IOptions<FeatureToggles> featureToggles)
        {
            _context = context;
            _featureToggles = featureToggles.Value;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (!_featureToggles.EnableTodoGetAll)
            {
                return BadRequest("Todo get all is disabled.");
            }
            return Ok(_context.Todos.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!_featureToggles.EnableTodoGet)
            {
                return BadRequest("Todo get by id is disabled.");
            }
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        // POST api/todos
        [HttpPost]
        public IActionResult Create(Todo todo)
        {
            try
            {
                if (!_featureToggles.EnableTodoCreation)
                {
                    return BadRequest("Todo creation is disabled.");
                }
                if (todo == null || string.IsNullOrEmpty(todo.Title))
                {
                    return BadRequest("Invalid todo item");
                }

                _context.Todos.Add(todo);
                _context.SaveChanges();

            }
            catch
            {

                
            }
            

            return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
        }

        // PUT api/todos/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, Todo updatedTodo)
        {
            if (!_featureToggles.EnableTodoPut)
            {
                return BadRequest("Todo PUT is disabled.");
            }
            var todo = _context.Todos.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.Title = updatedTodo.Title ?? todo.Title;
            todo.IsCompleted = updatedTodo.IsCompleted;

            _context.Todos.Update(todo);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE api/todos/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (!_featureToggles.EnableTodoDeletion)
                {
                    return BadRequest("Todo deletion is disabled.");
                }
                var todo = _context.Todos.Find(id);
                if (todo == null)
                {
                    return NotFound();
                }

                _context.Todos.Remove(todo);
                _context.SaveChanges();
            }
            catch
            {

                return NotFound($"Item by Id {id} not found.");
            }
            

            return NoContent();
        }

        [HttpGet("GetFeatures")]
        public ActionResult<FeatureToggles> GetFeatureToggles()
        {
            return Ok(_featureToggles);
        }

        [HttpPut("ToggleFeatures")]
        public ActionResult UpdateFeatureToggles([FromBody] FeatureToggles toggles)
        {
            _featureToggles.EnableTodoCreation = toggles.EnableTodoCreation;
            _featureToggles.EnableTodoDeletion = toggles.EnableTodoDeletion;
            _featureToggles.EnableTodoGet = toggles.EnableTodoGet;
            _featureToggles.EnableTodoGetAll = toggles.EnableTodoGetAll;
            _featureToggles.EnableTodoPost = toggles.EnableTodoPost;
            _featureToggles.EnableTodoPut = toggles.EnableTodoPut;
            return Ok();
        }

    }

    
}
