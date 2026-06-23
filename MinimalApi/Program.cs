using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

using BasicWebApi.MinimalApi.Models;
using BasicWebApi.MinimalApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Implement service registration using a factory method to create an instance of InMemoryTaskService
builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

var app = builder.Build();

var todos = new List<Todo>();

// Rewrite middleware to redirect /tasks/{id} to /todos/{id}
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

// Custom middleware to log request and response details
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] started");
    await next(context);
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] completed");
});

// 1. GET /todos
app.MapGet("/todos", (ITaskService service) => service.GetTodos());

// 2. GET /todos/{id}
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, ITaskService service) =>
{
    var targetTodo = service.GetTodoById(id);
    return targetTodo is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(targetTodo);
});

// 3. POST /todos
app.MapPost("/todos", (Todo task, ITaskService service) =>
{
    service.AddTodo(task);
    return TypedResults.Created($"/todos/{task.Id}", task);
})
.AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary<string, string[]>();

    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(Todo.DueDate), ["Due date cannot be in the past."]);
    }

    if (taskArgument.IsCompleted)
    {
        errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed todo."]);
    }

    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    return await next(context);
});

// 4. DELETE /todos/{id}
app.MapDelete("/todos/{id}", (int id, ITaskService service) =>
{
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});

// 5. PUT /todos/{id}

app.Run();