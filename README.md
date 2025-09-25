\## README.md

```markdown

\# Async Web Page Downloader (.NET)





A senior-level, production-ready sample demonstrating \*\*asynchronous, parallel web page downloads\*\* with clean architecture, SOLID principles, error handling via \*\*ProblemDetails\*\*, structured logging, cancellation support, and unit tests.





\## Why this design?

\- \*\*Clean layering\*\*: `Api` (transport \& composition) · `Application` (use cases) · `Infrastructure` (I/O, Http, storage).

\- \*\*SOLID\*\*: Abstractions for Http fetch (`IWebPageFetcher`) and storage (`IContentStore`). `DownloadManager` has a single responsibility: orchestration.

\- \*\*Patterns\*\*: Strategy (pluggable content store: Memory/File), Factory (HttpClientFactory), Decorator-ish logging, Options pattern, Resilience (Polly).

\- \*\*Observability\*\*: structured logs; errors surfaced as RFC7807 ProblemDetails by middleware.

\- \*\*Safety\*\*: bounded concurrency, cancellation tokens, and retries for transient failures.





\## Endpoints

\- `POST /api/downloads`





Request:

```json

{

"urls": \[

"https://www.example.com",

"https://www.wikipedia.org"

],

"degreeOfParallelism": 4

}

```





Response (200 OK):

```json

\[

{

"url": "https://www.example.com",

"statusCode": 200,

"contentLength": 1256,

"sha256": "ABCD...",

"storedAt": "memory://example\_com\_root\_1A2B3C4D",

"elapsedMs": 120,

"status": 0,

"error": null

},

{ "...": "..." }

]

```





Errors are returned as `application/problem+json`.





\## Run locally

```bash

\# from repository root

dotnet new sln -n AsyncWebPageDownloader

dotnet new web -n WebDownloader.Api -o src/WebDownloader.Api

dotnet new classlib -n WebDownloader.Application -o src/WebDownloader.Application

dotnet new classlib -n WebDownloader.Infrastructure -o src/WebDownloader.Infrastructure

dotnet new xunit -n WebDownloader.Tests -o tests/WebDownloader.Tests

\# (add project references as created in the repository; in this sample code files are provided already)

dotnet build

dotnet test

dotnet run --project src/WebDownloader.Api

```





Then open Swagger UI at `http://localhost:5000/swagger` (or the port shown in console).





\## Configuration

`appsettings.json`:

```json

{

"Downloader": {

"DefaultDegreeOfParallelism": 4,

"StorageMode": "Memory", // or "File"

"StorageDirectory": "downloads"

}

}

```





\## Highlights

\- \*\*Cancellation\*\*: pass `CancellationToken` (e.g., client disconnect) flows to Http calls and storage.

\- \*\*Retries\*\*: transient errors (>=500 or 408) retried with exponential backoff.

\- \*\*Hashing\*\*: SHA256 to verify content integrity and enable deduplication strategies later.

\- \*\*Bounded parallelism\*\*: `SemaphoreSlim` to control concurrency and avoid overwhelming targets.

\- \*\*Testability\*\*: core logic tested with stubbed fetcher and in-memory store.





\## Possible Extensions

\- Persist results and job states (e.g., SQLite + EF Core) to support async job tracking.

\- Stream responses directly to disk to avoid large memory usage for big pages.

\- Add \*\*OpenTelemetry\*\* (traces/metrics) and dashboard via Grafana/Prometheus.

\- Add gRPC endpoint alternative and message-driven trigger for large queues.





\## Notes

\- No company names are embedded in code.

\- This repository is suitable for publishing to a personal GitHub as a portfolio-quality sample.

