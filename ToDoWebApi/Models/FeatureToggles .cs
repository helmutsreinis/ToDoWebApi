namespace ToDoWebApi.Models
{
    public class FeatureToggles
    {
        public bool EnableTodoGetAll { get; set; }
        public bool EnableTodoGet { get; set; }
        public bool EnableTodoCreation { get; set; }
        public bool EnableTodoDeletion { get; set; }
        public bool EnableTodoPost { get; set; }
        public bool EnableTodoPut { get; set; }
    }
}
