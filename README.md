# Backend Web Development with .NET for Beginners

Building web apps and services with ASP.NET core. The project includes CRUD routes, middleware, a Todo class and an interface, testing with .http file, endpoint filtering, and dependency injection.

Notes are from this [YouTube playlist](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oWunQnm3WnZxJrseIw2zSAk).

<!--
  namespace: N/A
  GitHub slug: BasicWebApi
  About text: A full C# CRUD app using ASP.NET core including middleware.
 -->

<span aria-hidden="true"><br></span>

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download) 10.0
- [Visual Studio Code](https://code.visualstudio.com/) with [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

<span aria-hidden="true"><br></span>

## Installation & Usage

1. Clone this repository and switch into project folder

   ```sh
   git clone https://github.com/Kernix13/BasicWebApi.git
   cd BasicWebApi/MinimalApi
   ```

2. Run the application

   ```bash
   dotnet run
   ```

3. Build the application

   ```bash
   dotnet build
   ```

### Testing the Endpoints

This project includes a `MinimalApi.http` file, allowing you to test the API endpoints directly inside your code editor without leaving your workspace.

1. Ensure the application is running via the terminal command above.
2. Open `MinimalApi/MinimalApi.http` in VS Code.
3. Click the **Send Request** link that appears directly above any endpoint (GET, POST, etc.) to view the response.

### <span aria-hidden="true">⚡</span> Quick Start

```sh
git clone https://github.com/Kernix13/BasicWebApi.git
cd BasicWebApi/MinimalApi
dotnet run
```

<span aria-hidden="true"><br></span>

## Project Structure

I deviated from the viseo and created a `Models` and `Services` folder and moved the business logic into specific files in those folders. Here is what those folders look like:

```python
BasicWebApi/
└── MinimalApi/
    ├── Models/
    │   └── Todo.cs
    ├── Services/
    │   ├── ITaskService.cs
    │   └── InMemoryTaskService.cs
    | ...
```

<span aria-hidden="true"><br></span>

## Syntax and notes for this project

- ASP.NET core: a framework to build web apps, includes middleware, built-in support for building web APIs
- Web APIs: apis that communicate over the internet
  - Have a request/response (client/server) pattern
  - They use JSON or XML

```sh
# Creates a minimal ASP.NET Core application with very little included
dotnet new web -n ProjectName
```

```cs
// Starter code in Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

- The Web Application builder provides you with APIs for configuring the application host
  - The web application used to configure the HTTP pipeline, and routes.
- Configures the request/response pipeline
  - allow users to configure route handlers
  - exposes a `.Run()` method that you can call to start your http server and initiate your http request processing pipeline
- Request sent > server is responsible for processing the request and sending back a response
- Minimal apis allow you to describe how requests should be processed by a server by using an endpoint
- There are 3 components for defining how a web request should be handled:

```cs
//    1     2    3 --------------- |
app.MapGet("/", () => "Hello World!");
/*
  1. The http method: MapGet
  2. The url route: "/"
  3. The handler: () => "Hello World!"
      - executes when an incoming request is processed that matches the method & the route
*/
```

- Create a type for Todos using a record (in `Models/Todo.cs`)
- Run `dotnet run` or click the Play button top right in `Program.cs`, then open a browser to `http://localhost:5136/todos`.
- Or use the `.http` file to send requests.

```cs
// I am not familiar with the record type
public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted)
```

### Using statements in `Program.cs` & a Todo List

```cs
// Results needs the following using statement
using Microsoft.AspNetCore.Http.HttpResults;
// RewriteOptions needs the following using statement
using Microsoft.AspNetCore.Rewrite;

// Todos are stored in memory, no DB at this point
var todos = new List<Todo>();
```

### Create a route:

```cs
// 2. GET /todos/{id}
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) =>
{
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    return targetTodo is null ? TypedResults.NotFound() : TypedResults.Ok(targetTodo);
});
```

- Hard code examples in the .http file to test each endpoint/route for create, upate and delete

### Middleware:

- Logic that runs on every http request sent to the server - that is where middleware comes in
- Middleware: a piece of code that can run before & after each request is processed
  - They run on all requests in a nested order
  - Execute common functionality on each request, e.g. logging, host static files, etc
- ASP.NET core has middleware built in but you can write your own middleware
- Middlewares run in nested order
- NOTE: middleware is usually registered with the `Use` keyword - indicates you want to register a middleware
- Most ASP.NET core middleware come with some kind of options

### Middleware to support logging:

- `app.Use()`
  - `context`: represents the current request and response
  - `next`: to invoke the next middleware
- Why is `async` and `await` used?

```cs
// Logger middleware
app.Use(async (context, next) =>
{
    // code here, e.g.
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] started");
    await next(context);
});
```

### Endpoint filters:

- Execute common functionality on every request sent to a specific endpoint
- That is where endpoint filters come in
- Endpoint filters follow some of the same concepts as middleware
- They are commonly used for - the objects passed to an endpoint or what is returned
- Chain on `.AddEndpointFilter()` - you can chain multiple `AddEndpointFilter` statements
- there is also `context` and `next` like with middlewares
  - Middlewares run in your application pipelines
  - Endpoint filters run in the context of an endpoint(s)
- Endpoint filter for MapPost endpoint: validation - use it to make sure the Todo is not in the past or the IsCompleted is not set to `true`
  - the body retrieves the Todo argument (`GetArgument`)
  - also, a `new Dictionary` to capture the errors
  - then if statements for validation and to check for errors

```cs
// 3. POST /todos
app.MapPost("/todos", (Todo task, ITaskService service) =>
{
    service.AddTodo(task);
    return TypedResults.Created($"/todos/{task.Id}", task);
})
.AddEndpointFilter(async (context, next) =>
{
    // see code in Program.cs
});
```

### Dependency injection:

- Dependencies: are objects that other objects can depend on
  - usually implemented via C# classes and interfaces
  - Dependencies can also be referred to as Services b\c they are stored in the service container
  - A Service is a class that holds your business logic
- Injection: the process by which a referenced dependency is resolved from the service container (?)
- This happend either
  - when a class is constructed or
  - in the route handlers you write
- NOTE: Singleton - a service that lasts for the lifetime of the app (?)

> It seems `Services` refer to classes and/or interfaces (?)

1. Create an interface
2. Implement the interface
3. Stopped understanding the video at this point

```cs
// Register a service
builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

// Pass in ITaskService as a param to the routes
// Original:
app.MapGet("/todos", () => todos);
// New: (see Services/InMemoryTaskService)
app.MapGet("/todos", (ITaskService service) => service.GetTodos());
```

> What is a DI container? Part 7 video is confusing.

**7:26 she talks about using a database**

<span aria-hidden="true"><br></span>

## Starter code that differs from the final code

Below are code blocks for the simpler code used in the beginning of the video series. The final code in the files were changed by the end of the series.

### POST route in `Program.cs`:

```cs
// 3. POST /todos - see line 40 in Program.cs
app.MapPost("/todos", (Todo task) =>
{
    todos.Add(task);
    return TypedResults.Created($"/todos/{task.Id}", task);
});
```

### GET routes

```cs
// 1. GET /todos - see line 28 in Program.cs
app.MapGet("/todos", () => todos);

// 2. GET /todos/{id} - see line 31 in Program.cs
app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) =>
{
    var targetTodo = todos.SingleOrDefault(todo => id == todo.Id)
    return targetTodo is null
      ? TypedResults.NotFound()
      : TypedResults.Ok(targetTodo);
});
```

### DELETE route:

```cs
// 4. DELETE /todos/{id} - see line 66 in Program.cs
app.MapDelete("/todos/{id}", (int id) =>
{
    todos.RemoveAll(todo => id == todo.Id);
    return TypedResults.NoContent();
});
```
