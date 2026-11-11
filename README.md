# ASP.NET Core Task API

A minimal C# REST API for creating, listing, and completing tasks. The task feature separates HTTP routing and validation from the in-memory store.

## Run locally

Requires the .NET 9 SDK. From the repository root:

```powershell
dotnet run --project src/TaskApi -- --urls http://127.0.0.1:5050
```

| Method | Path | Result |
| --- | --- | --- |
| GET | /health | Service health |
| GET | /tasks | All tasks ordered by ID |
| POST | /tasks | Create a task from `{"title":"Review pull request"}` |
| GET | /tasks/{id} | Fetch a task |
| PUT | /tasks/{id}/complete | Mark a task complete |

`src/TaskApi/Tasks` contains the task endpoints and store. `tests/smoke.ps1` exercises HTTP responses and task state. Run it with PowerShell 7 in a second terminal while the API is running.

The sample runs on loopback and stores tasks in memory. A public deployment would need authentication and durable storage.
