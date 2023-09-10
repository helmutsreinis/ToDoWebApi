using Microsoft.EntityFrameworkCore;


namespace MyTodoApp.Models
{
    public class TodoContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options)
        {
            // Ensure DB is created.
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=MyTodoApp.db");
    }
}