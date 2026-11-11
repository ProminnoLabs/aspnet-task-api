param([string]$BaseUrl = 'http://127.0.0.1:5050')
$ErrorActionPreference = 'Stop'

$ready = $false
for ($attempt = 0; $attempt -lt 30; $attempt++) {
    try {
        $null = Invoke-WebRequest "$BaseUrl/health" -TimeoutSec 2
        $ready = $true
        break
    } catch {
        Start-Sleep -Milliseconds 300
    }
}
if (!$ready) { throw 'API failed to become healthy' }

$invalid = Invoke-WebRequest "$BaseUrl/tasks" -Method Post -ContentType application/json -Body '{"title":" "}' -SkipHttpErrorCheck
if ($invalid.StatusCode -ne 400) { throw 'Expected HTTP 400 for a blank title' }

$response = Invoke-WebRequest "$BaseUrl/tasks" -Method Post -ContentType application/json -Body '{"title":" Review pull request "}'
$task = $response.Content | ConvertFrom-Json
if ($response.StatusCode -ne 201 -or $task.title -ne 'Review pull request' -or $task.completed) {
    throw 'Unexpected create response'
}

$completed = Invoke-RestMethod "$BaseUrl/tasks/$($task.id)/complete" -Method Put
$fetched = Invoke-RestMethod "$BaseUrl/tasks/$($task.id)"
$listed = Invoke-RestMethod "$BaseUrl/tasks"
if (!$completed.completed -or !$fetched.completed -or $task.id -notin $listed.id) {
    throw 'Task completion was not visible through the API'
}

$missing = Invoke-WebRequest "$BaseUrl/tasks/-1" -SkipHttpErrorCheck
if ($missing.StatusCode -ne 404) { throw 'Expected HTTP 404 for an unknown task' }
'API smoke checks passed'
