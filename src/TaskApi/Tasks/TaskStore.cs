using System.Collections.Concurrent;

namespace TaskApi.Tasks;

public sealed record TaskItem(int Id, string Title, bool Completed);

public sealed class TaskStore
{
    private readonly ConcurrentDictionary<int, TaskItem> tasks = new();
    private int nextId;

    public TaskItem[] List() => tasks.Values.OrderBy(task => task.Id).ToArray();

    public TaskItem? Find(int id) => tasks.GetValueOrDefault(id);

    public TaskItem Create(string title)
    {
        var task = new TaskItem(Interlocked.Increment(ref nextId), title, false);
        tasks[task.Id] = task;
        return task;
    }

    public TaskItem? Complete(int id)
    {
        if (!tasks.TryGetValue(id, out var task))
        {
            return null;
        }

        var completed = task with { Completed = true };
        tasks[id] = completed;
        return completed;
    }
}
