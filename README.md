# Async Web Page Downloader (.NET)

## Problem Statement

Modern applications often require downloading data from multiple
external sources. A naive approach of firing all requests simultaneously
can quickly overwhelm both the client machine and the remote servers,
leading to resource exhaustion, throttling, or failure. Additionally,
handling transient network failures gracefully (e.g., timeouts, 5xx
errors) is critical for building resilient systems.

**Task**: Implement a .NET service that downloads multiple web pages
asynchronously with:\
- Concurrency control (limit degree of parallelism)\
- Resilience (automatic retries with backoff)\
- Cancellation support\
- Structured logging and error handling\
- Clean architecture and testability

------------------------------------------------------------------------

## Solution

This project demonstrates a **clean, layered architecture** for building
a resilient async web page downloader:

                      +-----------------+
                      |   API Layer     |  (ASP.NET Core Minimal API)
                      +-----------------+
                               |
                               v
                      +-----------------+
                      | Application     |  (Use Cases, Orchestration)
                      | - DownloadManager
                      | - Interfaces: IWebPageFetcher, IContentStore
                      +-----------------+
                               |
            -------------------------------------------
            |                                         |
            v                                         v
    +-------------------+                     +----------------------+
    | Infrastructure    |                     | Infrastructure       |
    | (HTTP Fetcher)    |                     | (Content Storage)    |
    | - HttpClient +    |                     | - FileSystem Store   |
    |   Polly Retry     |                     | - In-Memory Store    |
    +-------------------+                     +----------------------+

### Key Features

-   **Parallel downloads with bounded concurrency** via `SemaphoreSlim`\
-   **Thread-safe result collection** via
    `ConcurrentBag<DownloadResult>`\
-   **Retry policy** using Polly with exponential backoff\
-   **Graceful cancellation** via `CancellationToken`\
-   **Structured logging** and error handling via RFC 7807
    `ProblemDetails`\
-   **Unit Tests** with xUnit to validate orchestration logic\
-   **Clean Architecture**: Application orchestrates, Infrastructure
    handles I/O, API hosts

------------------------------------------------------------------------

## How to Run

### Prerequisites

-   .NET 8 SDK or later
-   Git

### Steps

``` bash
# clone repository
git clone https://github.com/your-username/AsyncWebPageDownloader.git
cd AsyncWebPageDownloader

# build solution
dotnet build

# run API
dotnet run --project src/WebDownloader.Api
```

The API will start locally (by default at `http://localhost:5000` or
`http://localhost:5187`).\
Swagger UI is available at `/swagger`.

------------------------------------------------------------------------

## How to Test

### Unit Tests

``` bash
dotnet test
```

### Example Request

`POST /api/downloads`

``` json
{
  "urls": [
    "https://www.example.com",
    "https://www.wikipedia.org"
  ],
  "degreeOfParallelism": 4
}
```

### Example Response

``` json
[
  {
    "url": "https://www.example.com",
    "statusCode": 200,
    "contentLength": 1256,
    "sha256": "ABC123...",
    "storedAt": "memory://example_com_root_1A2B3C4D",
    "elapsedMs": 120,
    "status": 0,
    "error": null
  },
  {
    "url": "https://www.wikipedia.org",
    "statusCode": 200,
    "contentLength": 34890,
    "sha256": "DEF456...",
    "storedAt": "memory://wikipedia_org_root_9Z8Y7X6W",
    "elapsedMs": 210,
    "status": 0,
    "error": null
  }
]
```

------------------------------------------------------------------------

## Highlights

-   **SOLID principles**: clear separation of concerns, abstractions
    over implementations\
-   **Design Patterns**: Strategy (storage), Factory (HttpClient),
    Decorator-like logging\
-   **Resilience**: retries, backoff, cancellation support\
-   **Observability**: structured logs, ProblemDetails errors,
    extensible to OpenTelemetry\
-   **Extensibility**: swap storage backends, extend fetcher policies,
    integrate with messaging systems

------------------------------------------------------------------------

## License

This project is provided as an open-source sample for demonstration and
portfolio purposes.\
Feel free to fork and adapt it to your needs.
