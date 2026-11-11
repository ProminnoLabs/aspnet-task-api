namespace TaskApi.Tasks;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        var tasks = app.MapGroup("/tasks");
        tasks.MapGet("/", (TaskStore store) => store.List());
        tasks.MapGet("/{id:int}", (int id, TaskStore store) =>
            store.Find(id) is { } task ? Results.Ok(task) : Results.NotFound());
        tasks.MapPost("/", Create);
        tasks.MapPut("/{id:int}/complete", (int id, TaskStore store) =>
            store.Complete(id) is { } task ? Results.Ok(task) : Results.NotFound());
    }

    private static IResult Create(CreateTask request, TaskStore store)
    {
        var title = request.Title?.Trim();
        if (string.IsNullOrEmpty(title) || title.Length > 120)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["title"] = ["Enter a title with 1 to 120 characters."]
            });
        }

        var task = store.Create(title);
        return Results.Created($"/tasks/{task.Id}", task);
    }

    private sealed record CreateTask(string? Title);
}
