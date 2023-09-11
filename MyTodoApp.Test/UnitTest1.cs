using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyTodoApp.Controllers;
using MyTodoApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ToDoWebApi.Models;
using Xunit;

namespace MyTodoApp.Test
{
    public class TodoControllerTests : IDisposable
    {
        private readonly TodoContext _todoContext;
        private readonly IOptions<FeatureToggles> _featureToggles;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TestTodoDatabase")
                .Options;
            _todoContext = new TodoContext(options);
            _todoContext.Database.EnsureCreated();  // Make sure the database is created

            // Feature toggles
            _featureToggles = Options.Create(new FeatureToggles());

            _controller = new TodoController(_todoContext, _featureToggles);
        }



        [Fact]
        public void GetAll_ReturnsBadRequest_WhenFeatureIsDisabled()
        {
            _featureToggles.Value.EnableTodoGetAll = false;

            var result = _controller.GetAll();

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetAll_ReturnsOk_WhenFeatureIsEnabled()
        {
            _featureToggles.Value.EnableTodoGetAll = true;
            //_todoContext.Todos.Remove(new Todo { Id = 1, Title = "Test", IsCompleted = false });
            _todoContext.Todos.Add(new Todo { Id = 1, Title = "Test", IsCompleted = false });
            _todoContext.SaveChanges();

            var result = _controller.GetAll() as OkObjectResult;

            var items = Assert.IsType<List<Todo>>(result.Value);
            bool assertion = items.Count > 0;
            Assert.True(assertion);
        }
        [Fact]
        public void Get_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            _featureToggles.Value.EnableTodoGet = true;

            var result = _controller.Get(143);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_ReturnsOk_WhenTodoExists()
        {
            _featureToggles.Value.EnableTodoGet = true;
            //Adding remove to avoid inmemory DB conflicts
            _todoContext.Todos.Remove(new Todo { Id = 2, Title = "Test", IsCompleted = false });

            _todoContext.Todos.Add(new Todo { Id = 2, Title = "Test", IsCompleted = false });
            _todoContext.SaveChanges();

            var result = _controller.Get(2) as OkObjectResult;

            var item = Assert.IsType<Todo>(result.Value);
            Assert.Equal("Test", item.Title);
        }

        [Fact]
        public void Create_ReturnsBadRequest_WhenFeatureIsDisabled()
        {
            _featureToggles.Value.EnableTodoCreation = false;
            var result = _controller.Create(new Todo { Id = 1, Title = "Test", IsCompleted = false });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Create_ReturnsBadRequest_WhenModelIsInvalid()
        {
            _featureToggles.Value.EnableTodoCreation = true;
            var result = _controller.Create(new Todo { Id = 1 });
            //_controller.Delete(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Create_ReturnsCreatedAtAction_WhenModelIsValid()
        {
            _featureToggles.Value.EnableTodoCreation = true;
            _controller.Delete(1);
            var result = _controller.Create(new Todo { Id = 1, Title = "Test", IsCompleted = false }) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.Equal("Get", result.ActionName);
        }

        [Fact]
        public void Update_ReturnsBadRequest_WhenFeatureIsDisabled()
        {
            _featureToggles.Value.EnableTodoPut = false;
            var result = _controller.Update(1, new Todo { Id = 14, Title = "Updated Test", IsCompleted = true });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            _featureToggles.Value.EnableTodoPut = true;
            var result = _controller.Update(1, new Todo { Id = 15, Title = "Updated Test", IsCompleted = true });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_ReturnsNoContent_WhenTodoIsUpdated()
        {
            _featureToggles.Value.EnableTodoPut = true;
            _todoContext.Todos.Add(new Todo { Id = 1, Title = "Test", IsCompleted = false });
            _todoContext.SaveChanges();

            var result = _controller.Update(1, new Todo { Id = 1, Title = "Updated Test", IsCompleted = true });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ReturnsBadRequest_WhenFeatureIsDisabled()
        {
            _featureToggles.Value.EnableTodoDeletion = false;
            var result = _controller.Delete(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNotFound_WhenTodoDoesNotExist()
        {
            _featureToggles.Value.EnableTodoDeletion = true;
            var result = _controller.Delete(132);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ReturnsNoContent_WhenTodoIsDeleted()
        {
            _featureToggles.Value.EnableTodoDeletion = true;
            _todoContext.Todos.Add(new Todo { Id = 1, Title = "Test", IsCompleted = false });
            _todoContext.SaveChanges();

            var result = _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        public void Dispose()
        {
            _todoContext.Database.EnsureDeleted(); // This will delete the database
        }
    }
}
