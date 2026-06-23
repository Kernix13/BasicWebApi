namespace BasicWebApi.MinimalApi.Models;

public record Todo(int Id, string Title, DateTime DueDate, bool IsCompleted);