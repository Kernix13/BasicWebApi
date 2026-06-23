using BasicWebApi.MinimalApi.Models;

namespace BasicWebApi.MinimalApi.Services;

public interface ITaskService
{
    List<Todo> GetTodos();
    Todo? GetTodoById(int id);
    Todo AddTodo(Todo task);
    void DeleteTodoById(int id);
}