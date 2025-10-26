# IP Geolocation and Country Blocking API

A .NET 9.0 Web API application for managing blocked countries and validating IP addresses using third-party geolocation APIs. The application uses in-memory storage (ConcurrentDictionary) for thread-safe operations without requiring a database.

## Features

- **Country Blocking Management**: Add, remove, and view blocked countries
- **IP Geolocation Lookup**: Fetch country details for any IP address
- **Automatic IP Blocking Verification**: Check if an IP address belongs to a blocked country
- **Temporal Country Blocking**: Block countries for specific durations (auto-expires)
- **Blocked Attempt Logging**: Track all blocked access attempts with details
- **Pagination & Filtering**: Paginated responses with search/filter capabilities
- **Background Services**: Automatic cleanup of expired temporal blocks

## Technology Stack

- **Framework**: .NET 9.0
- **Package Management**: NuGet
- **In-Memory Storage**: ConcurrentDictionary
- **Background Jobs**: Hangfire
- **API Integration**: IPGeolocation.io
- **Validation**: FluentValidation
- **IoC Container**: Autofac
- **Mediation**: MediatR
- **API Documentation**: Swagger/OpenAPI

## Quick Start

### Prerequisites

- .NET 9.0 SDK installed
- Valid API key from IPGeolocation.io (Get one at [ipgeolocation.io](https://ipgeolocation.io))

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd Interview\ Question/Main
```

2. Configure your API key in `appsettings.json`:
```json
{
  "IpGeolocation": {
    "ApiKey": "your-api-key-here",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  }
}
```

3. Restore NuGet packages:
```bash
dotnet restore
```

4. Run the application:
```bash
dotnet run
```

5. Access the API:
- **Swagger UI**: http://localhost:5000 (or the port shown in your terminal)
- **Hangfire Dashboard**: http://localhost:5000/hangfire

## API Endpoints

### 1. Block a Country
- **Endpoint**: `POST /api/countries/block`
- **Description**: Adds a country to the blocked list
- **Request Body**:
```json
{
  "countryCode": "US"
}
```

### 2. Unblock a Country
- **Endpoint**: `DELETE /api/countries/block/{countryCode}`
- **Description**: Removes a country from the blocked list
- **Example**: `DELETE /api/countries/block/US`

### 3. Get All Blocked Countries
- **Endpoint**: `GET /api/countries`
- **Description**: Returns paginated list of blocked countries
- **Query Parameters**:
  - `pageNumber`: Page number (default: 1)
  - `pageSize`: Items per page (default: 5, max: 30)
  - `countryCode`: Filter by country code (optional)
- **Example**: `GET /api/countries?pageNumber=1&pageSize=10&countryCode=US`

### 4. IP Geolocation Lookup
- **Endpoint**: `GET /api/ip/lookup`
- **Description**: Fetches country details for an IP address
- **Query Parameters**:
  - `ipAddress`: IP address to lookup (optional - uses caller IP if omitted)
- **Example**: `GET /api/ip/lookup?ipAddress=8.8.8.8`

### 5. Check if IP is Blocked
- **Endpoint**: `GET /api/ip/check-block`
- **Description**: Verifies if the caller's IP address belongs to a blocked country
- **Note**: Automatically detects caller IP, fetches country via API, and logs if blocked
- **Example**: `GET /api/ip/check-block`

### 6. Get Blocked Attempt Logs
- **Endpoint**: `GET /api/logs/blocked-attempts`
- **Description**: Returns paginated log of blocked access attempts
- **Query Parameters**:
  - `pageNumber`: Page number (default: 1)
  - `pageSize`: Items per page (default: 5, max: 30)
- **Example**: `GET /api/logs/blocked-attempts?pageNumber=1&pageSize=10`

### 7. Temporarily Block a Country
- **Endpoint**: `POST /api/countries/temporal-block`
- **Description**: Blocks a country for a specific duration (auto-expires)
- **Request Body**:
```json
{
  "countryCode": "US",
  "durationMinutes": 120
}
```
- **Duration Validation**: Must be between 1-1440 minutes (24 hours)

## Background Services

### Automatic Expiration Cleanup

A recurring Hangfire job runs every 5 minutes to automatically remove expired temporal blocks:

- **Job ID**: `CleanupBlockedAttempts`
- **Schedule**: Every 5 minutes (`*/5 * * * *`)
- **Dashboard**: Access at `/hangfire`

## Response Structure

All endpoints return a standardized response:

```json
{
  "data": {
    // Response data
  },
  "message": "Success message",
  "isSuccess": true,
  "errorCode": null
}
```

### Error Response

```json
{
  "data": null,
  "message": "Error message",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

## Pagination Headers

Paginated endpoints include the following response headers:

- `X-Pagination`
- `X-Page-Number`
- `X-Page-Size`
- `X-Total-Count`
- `X-Total-Pages`

## Architecture

### Project Structure

```
Main/
├── Common/
│   ├── Base/           # Base classes for endpoints and handlers
│   ├── Config/         # Autofac configuration
│   └── Response/       # Response models
├── Extensions/         # Extension methods
├── Feature/
│   └── IpGeoLocation/
│       ├── Endpoints/  # API endpoints
│       ├── In-MemoryList/ # In-memory storage implementations
│       ├── Queries/    # MediatR queries
│       └── VM/         # View models and validators
├── Helpers/            # Helper classes
├── Middlewares/        # Global error handling
├── Services/           # Service models
└── Program.cs          # Application entry point
```

### Key Components

- **In-Memory Storage**: `BlockedCountryIPTracker` and `BlockedAttemptMemoryStore`
- **Request Handling**: MediatR for query handling
- **Dependency Injection**: Autofac for IoC
- **Background Jobs**: Hangfire for scheduled tasks
- **Validation**: FluentValidation for request validation

## Thread Safety

The application uses `ConcurrentDictionary` for thread-safe operations:

- Country blocking operations
- Blocked attempt logging
- Temporal block tracking

## Testing

### Using Swagger UI

1. Navigate to `http://localhost:5000` (or your configured port)
2. Use the interactive API documentation
3. Try the following test flow:
   - Block a country: `POST /api/countries/block` with `{"countryCode": "US"}`
   - View blocked countries: `GET /api/countries`
   - Check IP: `GET /api/ip/check-block`
   - View logs: `GET /api/logs/blocked-attempts`
   - Unblock: `DELETE /api/countries/block/US`

### Using HTTP Client

Example with cURL:

```bash
# Block a country
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "GB"}'

# Get all blocked countries
curl "http://localhost:5000/api/countries?pageNumber=1&pageSize=10"

# Lookup IP
curl "http://localhost:5000/api/ip/lookup?ipAddress=8.8.8.8"

# Check if blocked
curl "http://localhost:5000/api/ip/check-block"

# Get logs
curl "http://localhost:5000/api/logs/blocked-attempts?pageNumber=1&pageSize=5"

# Unblock country
curl -X DELETE "http://localhost:5000/api/countries/block/GB"
```

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "IpGeolocation": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  },
  "AllowedHosts": "*"
}
```

### Environment-Specific Settings

- `appsettings.json`: Production settings
- `appsettings.Development.json`: Development settings

## Rate Limiting

The application integrates with IPGeolocation.io API which has rate limits:
- Free tier: 1,000 requests/month
- Higher tiers available at [ipgeolocation.io/pricing](https://ipgeolocation.io/pricing)

Handle API errors gracefully with proper error responses.

## Logging

The application uses Serilog for logging:
- **Console**: Development output
- **Seq**: Structured logging (configured at http://localhost:5341)

## Data Persistence

⚠️ **Important**: All data is stored in memory and will be lost when the application stops.

For production use, consider implementing persistence to a database.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## License

This project is provided as-is for evaluation purposes.

## Contact

For questions or issues regarding this assignment implementation, please refer to the project documentation or contact the development team.

---

**Note**: This is a demonstration project. For production deployments, consider implementing:
- Database persistence
- Authentication and authorization
- Rate limiting
- Caching strategies
- Monitoring and alerting

