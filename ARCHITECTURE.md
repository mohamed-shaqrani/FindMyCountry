# Architecture Documentation

Complete architectural overview of the IP Geolocation and Country Blocking API.

## Table of Contents

1. [System Overview](#system-overview)
2. [Architecture Patterns](#architecture-patterns)
3. [Project Structure](#project-structure)
4. [Component Design](#component-design)
5. [Data Flow](#data-flow)
6. [In-Memory Storage](#in-memory-storage)
7. [Background Services](#background-services)
8. [Security Considerations](#security-considerations)

## System Overview

The IP Geolocation and Country Blocking API is a .NET 9.0 Web API application designed to:
- Manage blocked countries in memory
- Validate IP addresses using third-party geolocation APIs
- Log blocked access attempts
- Support temporal (time-limited) country blocks

### Key Characteristics

- **No Database**: Uses in-memory storage (ConcurrentDictionary)
- **Thread-Safe**: Concurrent collections ensure thread safety
- **API Integration**: Integrates with IPGeolocation.io
- **Background Jobs**: Hangfire for scheduled tasks
- **Validation**: FluentValidation for request validation
- **Dependency Injection**: Autofac for IoC container

## Architecture Patterns

### 1. Clean Architecture

The project follows clean architecture principles:

```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│    (Endpoints/Controllers)          │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│     Application Layer               │
│    (Queries/Handlers)               │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│        Domain Layer                  │
│   (Models/ViewModels)                │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│    Infrastructure Layer              │
│  (Storage/External APIs)             │
└─────────────────────────────────────┘
```

### 2. CQRS (Command Query Responsibility Segregation)

- **Queries**: Read operations (e.g., `IpGeolocationQuery`)
- **Handlers**: Process queries using MediatR
- **Commands**: Write operations (implicit in endpoints)

### 3. Mediator Pattern

- Uses MediatR for decoupling
- Request/Response pattern
- Single handler per query

### 4. Repository Pattern

- Abstract storage concerns
- `ICountryIPTracker` for country blocking
- `IBlockedAttemptStore` for logging

## Project Structure

```
Main/
├── Common/                           # Shared infrastructure
│   ├── Base/
│   │   ├── BaseEndpoint.cs         # Base endpoint class
│   │   ├── BaseEndpointParam.cs    # Endpoint parameters
│   │   ├── BaseRequestHandler.cs   # Base handler class
│   │   └── BaseRequestHandlerParam.cs
│   ├── Config/
│   │   └── AutofacModule.cs       # IoC configuration
│   └── Response/
│       ├── Endpint/
│       │   └── EndpointResponse.cs  # Standard response
│       ├── RequestResult/
│       │   └── RequestResult.cs
│       ├── ErrorCode.cs            # Error codes
│       └── ValidationHelper.cs     # Validation utilities
├── Extensions/                      # Extension methods
│   ├── CompressionServiceExtension.cs
│   └── HttpExtensions.cs           # HTTP extensions (pagination)
├── Feature/
│   └── IpGeoLocation/              # Feature module
│       ├── Endpoints/              # API endpoints
│       │   ├── Countries/
│       │   │   ├── BlockCountryEndpoint.cs
│       │   │   ├── UnBlockCountryEndpoint.cs
│       │   │   ├── AllBlockedCountriesEndpoint.cs
│       │   │   └── TemporalCountryBlockEndpoint.cs
│       │   ├── IP/
│       │   │   ├── FindCountryIpEndpoint.cs
│       │   │   └── CheckBlockCountryEndpoint.cs
│       │   ├── Logs/
│       │   │   └── BlockedAttemptsLogEndpoint.cs
│       │   └── PaginationParam.cs
│       ├── In-MemoryList/          # Storage layer
│       │   ├── BlockedAttemptMemoryStore.cs
│       │   └── BlockedCountry/
│       │       ├── CountryIPTracker.cs
│       │       └── ICountryIPTracker.cs
│       ├── Queries/                  # CQRS queries
│       │   └── IpGeolocationQuery.cs
│       └── VM/                     # View models
│           ├── BlockCountryViewModel.cs
│           ├── TemporalCountryBlockViewModel.cs
│           ├── AllBlockedCountriesViewModel.cs
│           ├── CountryIpAddressViewModel.cs
│           └── Validators/         # FluentValidation
├── Helpers/                         # Helper classes
│   ├── CountryTrackerDetails.cs
│   ├── CountryParam.cs
│   ├── IpGeolocationOptions.cs
│   ├── PageList.cs
│   └── PaginationHeader.cs
├── Middlewares/
│   └── GlobalErrorHandlerMiddleware.cs
├── Services/
│   └── IpGeolocationResponse.cs
├── Program.cs                      # Entry point
└── Main.csproj                     # Project file
```

## Component Design

### 1. Endpoints

**Purpose**: Handle HTTP requests/responses

**Key Features**:
- Inherit from `BaseEndpoint<TRequest, TResponse>`
- Validate requests using FluentValidation
- Return standardized `EndpointResponse<T>`

**Example**:
```csharp
public sealed class BlockCountryEndpoint 
    : BaseEndpoint<BlockCountryViewModel, EndpointResponse<bool>>
{
    // Handles POST /api/countries/block
}
```

### 2. Request Handlers

**Purpose**: Implement business logic

**Key Features**:
- Implement `IRequestHandler<TRequest, TResponse>`
- Use MediatR for decoupling
- Handle external API calls

**Example**:
```csharp
internal sealed class IpGeolocationHandler 
    : BaseRequestHandler<IpGeolocationQuery, RequestResult<IpGeolocationResponse>>
{
    // Fetches IP geolocation from external API
}
```

### 3. In-Memory Storage

**Purpose**: Store data in memory

**Key Features**:
- Thread-safe ConcurrentDictionary
- Singleton pattern
- Automatic cleanup

**Interfaces**:
- `ICountryIPTracker` - Manages blocked countries
- `IBlockedAttemptStore` - Manages access logs

### 4. View Models

**Purpose**: Request/response data transfer objects

**Key Features**:
- FluentValidation for validation
- Immutable records
- Strongly typed

**Example**:
```csharp
public sealed record BlockCountryViewModel(string CountryCode);

public class BlockCountryViewModelValidator 
    : AbstractValidator<BlockCountryViewModel>
{
    // Validation rules
}
```

### 5. Background Services

**Purpose**: Scheduled tasks

**Key Features**:
- Hangfire for job scheduling
- Recurring jobs
- Memory storage (Hangfire.MemoryStorage)

**Jobs**:
- CleanupBlockedAttempts (every 5 minutes)

## Data Flow

### Block Country Flow

```
1. Client sends POST /api/countries/block
   └── { "countryCode": "US" }
        │
2. BlockCountryEndpoint receives request
   ├── Validates using FluentValidation
   └── Checks country code validity
        │
3. CountryIPTracker.TryAdd()
   ├── Uses ConcurrentDictionary
   └── Returns true/false
        │
4. Response sent to client
   └── Success or Error
```

### Check IP Flow

```
1. Client sends GET /api/ip/check-block
        │
2. CheckBlockCountryEndpoint receives request
   ├── Gets caller IP (HttpContext)
   └── Calls MediatR with IpGeolocationQuery
        │
3. IpGeolocationHandler
   ├── Calls IPGeolocation.io API
   └── Returns country details
        │
4. CountryIPTracker.IsCountryBlocked()
   ├── Checks ConcurrentDictionary
   └── Returns true/false
        │
5. If blocked, logs to BlockedAttemptStore
        │
6. Response sent to client
   └── Blocked or Not Blocked
```

### IP Lookup Flow

```
1. Client sends GET /api/ip/lookup?ipAddress=8.8.8.8
   (If ipAddress omitted, uses caller IP)
        │
2. FindCountryIpEndpoint
   ├── Validates IP format
   └── Calls MediatR with IpGeolocationQuery
        │
3. IpGeolocationHandler
   ├── Constructs API URL
   ├── Calls IPGeolocation.io
   └── Returns response
        │
4. Response sent to client
   └── Country details
```

## In-Memory Storage

### Country Blocking Storage

**Implementation**: `BlockedCountryIPTracker`

**Data Structure**:
```csharp
ConcurrentDictionary<string, CountryTrackerDetails>
```

**Features**:
- Thread-safe operations
- Automatic temporal block expiration
- Search functionality
- Duplicate prevention

**Methods**:
- `IsCountryBlocked(string countryCode)` - Check if blocked
- `RemoveExpiredBlockedCountries()` - Cleanup expired blocks
- `Search(string countryCode)` - Search/filter

### Blocked Attempt Logs

**Implementation**: `BlockedAttemptMemoryStore`

**Data Structure**:
```csharp
ConcurrentDictionary<Guid, BlockedAttemptLog>
```

**Features**:
- Thread-safe addition
- Ordered by timestamp
- Pagination support

**Methods**:
- `Add(BlockedAttemptLog)` - Add log entry
- `GetAll()` - Get all logs (ordered)

## Background Services

### Hangfire Configuration

**Purpose**: Schedule recurring tasks

**Configuration**:
```csharp
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();
```

**Dashboard**: `/hangfire`

### Recurring Jobs

**Job Name**: `CleanupBlockedAttempts`

**Schedule**: Every 5 minutes (`*/5 * * * *`)

**Task**: Remove expired temporal blocks

**Implementation**:
```csharp
RecurringJob.AddOrUpdate(
    recurringJobId: "CleanupBlockedAttempts",
    methodCall: () => _countryIPTracker.RemoveExpiredBlockedCountries(),
    cronExpression: "*/5 * * * *"
);
```

## Security Considerations

### Current Implementation

1. **No Authentication**: API is public
2. **Input Validation**: FluentValidation on all inputs
3. **Rate Limiting**: Depends on third-party API limits
4. **Error Handling**: Global error middleware

### Recommendations for Production

1. **Add Authentication**
   - JWT Bearer tokens
   - API keys for clients
   - Role-based access control

2. **Implement Rate Limiting**
   - Per-client limits
   - Per-endpoint limits
   - IP-based throttling

3. **Add Authorization**
   - User roles
   - Permission-based access
   - Audit logging

4. **Secure Storage**
   - Persist to database
   - Encrypt sensitive data
   - Backup strategy

5. **Add Monitoring**
   - Application Insights
   - Health checks
   - Metrics collection

## Dependency Injection

### Configuration (Autofac)

```csharp
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new AutofacModule());
});
```

### Registered Services

- `ICountryIPTracker` → `BlockedCountryIPTracker` (Singleton)
- `IBlockedAttemptStore` → `BlockedAttemptMemoryStore` (Singleton)
- `HttpClient` - For external API calls
- `IMediator` - For query handling
- Hangfire services

## Error Handling

### Global Error Middleware

**Purpose**: Catch all unhandled exceptions

**Features**:
- Logs errors
- Returns standardized error responses
- Prevents stack trace exposure

### Error Codes

- `VALIDATION_ERROR` - Invalid input
- `CONFLICT` - Duplicate resource
- `NOT_FOUND` - Resource not found
- `INTERNAL_ERROR` - Server error

## Logging

### Serilog Configuration

**Outputs**:
- Console (development)
- Seq (structured logging)

**Configuration**:
```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Seq("http://localhost:5341/")
    .WriteTo.Console()
    .CreateLogger();
```

## API Integration

### IPGeolocation.io Integration

**Base URL**: `https://api.ipgeolocation.io/ipgeo`

**Request**: `GET ?apiKey={key}&ip={ipAddress}`

**Response**:
```json
{
  "ip": "8.8.8.8",
  "country_name": "United States",
  "country_code2": "US",
  "country_code3": "USA",
  "isp": "Google LLC"
}
```

**Error Handling**:
- HTTP errors
- Timeout handling
- Rate limit detection

## Performance Considerations

### Thread Safety

- `ConcurrentDictionary` for thread-safe operations
- No locking required
- Lock-free data structures

### Memory Management

- Singleton services for storage
- No memory leaks
- Automatic cleanup

### Scalability

- Stateless design
- Horizontal scaling ready
- Consider database for production

## Testing Strategy

### Unit Tests

- Test endpoints independently
- Mock external dependencies
- Validate business logic

### Integration Tests

- Test full request/response flow
- Test external API integration
- Test error handling

### Load Testing

- Concurrent requests
- Rate limit handling
- Memory usage monitoring

## Future Enhancements

1. **Database Integration**
   - SQL Server / PostgreSQL
   - Entity Framework Core
   - Migration strategy

2. **Caching**
   - Redis for caching
   - Response caching
   - API response caching

3. **Monitoring**
   - Application Insights
   - Health checks
   - Metrics dashboard

4. **Advanced Features**
   - IP whitelist/blacklist
   - User-specific blocking
   - Geographic analytics
   - Alerting system

## Conclusion

This architecture provides:
- Clean separation of concerns
- Thread-safe operations
- Scalable design
- Maintainable code
- Production-ready foundation

The current implementation is suitable for development and testing. For production use, additional considerations (authentication, database, monitoring) should be implemented.

