using BasicWebApi.MinimalApi.Models;

namespace BasicWebApi.MinimalApi.Services;

class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = [];

    public List<Todo> GetTodos() => _todos;

    public Todo? GetTodoById(int id) => _todos.SingleOrDefault(task => id == task.Id);

    public Todo AddTodo(Todo task)
    {
        _todos.Add(task);
        return task;
    }

    public void DeleteTodoById(int id) => _todos.RemoveAll(task => id == task.Id);
}