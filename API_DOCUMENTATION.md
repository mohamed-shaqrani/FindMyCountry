# API Documentation

## IP Geolocation and Country Blocking API

Complete API reference documentation for all endpoints.

## Base URL

```
http://localhost:5000
```

## Authentication

Currently, the API does not require authentication. This is recommended for production deployments.

## Response Format

All endpoints return a standardized response format:

### Success Response

```json
{
  "data": { /* Response data */ },
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
  "errorCode": "ERROR_CODE"
}
```

### HTTP Status Codes

- `200 OK`: Successful request
- `400 Bad Request`: Validation error or invalid input
- `404 Not Found`: Resource not found
- `409 Conflict`: Conflict (e.g., duplicate resource)
- `500 Internal Server Error`: Server error

## Endpoints

---

## 1. Block a Country

Adds a country to the blocked list permanently.

### Endpoint

```
POST /api/countries/block
```

### Request Body

```json
{
  "countryCode": "US"
}
```

### Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| countryCode | string | Yes | Two-letter ISO 3166-1 alpha-2 country code |

### Response

```json
{
  "data": true,
  "message": "Country blocked successfully.",
  "isSuccess": true,
  "errorCode": null
}
```

### Error Responses

**400 Bad Request** - Invalid country code:
```json
{
  "data": null,
  "message": "Invalid Country Code",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

**400 Bad Request** - Country already blocked:
```json
{
  "data": null,
  "message": "Country is already blocked.",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

### Example

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US"}'
```

### Valid Country Codes

The API validates against ISO 3166-1 alpha-2 country codes:
- US, GB, CA, FR, DE, IT, ES, JP, CN, AU, BR, IN, MX, etc.
- Total of 249 valid codes supported

---

## 2. Unblock a Country

Removes a country from the blocked list.

### Endpoint

```
DELETE /api/countries/block/{countryCode}
```

### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| countryCode | string | Yes | Two-letter country code |

### Response

```json
{
  "data": true,
  "message": "Country Unblocked successfully.",
  "isSuccess": true,
  "errorCode": null
}
```

### Error Responses

**400 Bad Request** - Country not blocked:
```json
{
  "data": null,
  "message": "Country already does not exist.",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

### Example

```bash
curl -X DELETE "http://localhost:5000/api/countries/block/US"
```

---

## 3. Get All Blocked Countries

Retrieves a paginated list of blocked countries with optional filtering.

### Endpoint

```
GET /api/countries
```

### Query Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageNumber | integer | No | 1 | Page number (starts at 1) |
| pageSize | integer | No | 5 | Items per page (max: 30) |
| countryCode | string | No | - | Filter by country code |

### Response

```json
{
  "data": {
    "currentPage": 1,
    "pageSize": 5,
    "totalCount": 3,
    "totalPages": 1,
    "items": [
      {
        "countryName": "",
        "accessCount": 0,
        "countryCode": "US",
        "blockedAt": "2024-01-15T10:30:00Z",
        "isTemporarilyBlocked": false,
        "blockedDuration": "02:00:00"
      },
      {
        "countryName": "",
        "accessCount": 0,
        "countryCode": "GB",
        "blockedAt": "2024-01-15T10:31:00Z",
        "isTemporarilyBlocked": false,
        "blockedDuration": "02:00:00"
      }
    ]
  },
  "message": "Success",
  "isSuccess": true,
  "errorCode": null
}
```

### Response Headers

- `X-Pagination`: Pagination metadata
- `X-Page-Number`: Current page number
- `X-Page-Size`: Page size
- `X-Total-Count`: Total number of items
- `X-Total-Pages`: Total number of pages

### Example

```bash
# Get all blocked countries (page 1, 10 items)
curl "http://localhost:5000/api/countries?pageNumber=1&pageSize=10"

# Search for specific country
curl "http://localhost:5000/api/countries?countryCode=US"
```

---

## 4. IP Geolocation Lookup

Fetches country details for an IP address.

### Endpoint

```
GET /api/ip/lookup
```

### Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| ipAddress | string | No | IP address to lookup. If omitted, uses caller's IP |

### Response

```json
{
  "data": {
    "ip": "8.8.8.8",
    "country_name": "United States",
    "country_code2": "US",
    "country_code3": "USA",
    "isp": "Google LLC"
  },
  "message": "Success",
  "isSuccess": true,
  "errorCode": null
}
```

### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| ip | string | IP address |
| country_name | string | Full country name |
| country_code2 | string | Two-letter country code |
| country_code3 | string | Three-letter country code |
| isp | string | Internet Service Provider |

### Error Responses

**400 Bad Request** - Invalid IP format:
```json
{
  "data": null,
  "message": "Invalid IP Address",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

### Example

```bash
# Lookup specific IP
curl "http://localhost:5000/api/ip/lookup?ipAddress=8.8.8.8"

# Lookup caller's IP
curl "http://localhost:5000/api/ip/lookup"
```

---

## 5. Check if IP is Blocked

Verifies if the caller's IP address belongs to a blocked country.

### Endpoint

```
GET /api/ip/check-block
```

### Description

This endpoint:
1. Automatically detects the caller's external IP address
2. Fetches the country code using the geolocation API
3. Checks if the country is in the blocked list
4. Logs the attempt if the country is blocked

### Response

**If blocked:**
```json
{
  "data": true,
  "message": "Country is blocked.",
  "isSuccess": true,
  "errorCode": null
}
```

**If not blocked:**
```json
{
  "data": false,
  "message": "Country is not blocked.",
  "isSuccess": true,
  "errorCode": null
}
```

### Error Responses

**400 Bad Request** - Invalid IP address:
```json
{
  "data": null,
  "message": "Invalid IP Address",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

### Example

```bash
curl "http://localhost:5000/api/ip/check-block"
```

**Note**: This endpoint logs blocked attempts. Check the logs endpoint to view them.

---

## 6. Get Blocked Attempt Logs

Returns a paginated log of blocked access attempts.

### Endpoint

```
GET /api/logs/blocked-attempts
```

### Query Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| pageNumber | integer | No | 1 | Page number (starts at 1) |
| pageSize | integer | No | 5 | Items per page (max: 30) |

### Response

```json
{
  "data": {
    "currentPage": 1,
    "pageSize": 5,
    "totalCount": 10,
    "totalPages": 2,
    "items": [
      {
        "ipAddress": "203.0.113.1",
        "countryCode": "US",
        "isBlocked": true,
        "timestampUtc": "2024-01-15T12:30:00Z",
        "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
      },
      {
        "ipAddress": "198.51.100.42",
        "countryCode": "GB",
        "isBlocked": true,
        "timestampUtc": "2024-01-15T12:29:00Z",
        "userAgent": "PostmanRuntime/7.32.3"
      }
    ]
  },
  "message": "Success",
  "isSuccess": true,
  "errorCode": null
}
```

### Log Entry Fields

| Field | Type | Description |
|-------|------|-------------|
| ipAddress | string | IP address that was checked |
| countryCode | string | Country code detected |
| isBlocked | boolean | Whether access was blocked |
| timestampUtc | datetime | UTC timestamp of the attempt |
| userAgent | string | User agent from the request |

### Example

```bash
# Get first page with 10 items
curl "http://localhost:5000/api/logs/blocked-attempts?pageNumber=1&pageSize=10"
```

---

## 7. Temporarily Block a Country

Blocks a country for a specific duration. The block will automatically expire.

### Endpoint

```
POST /api/countries/temporal-block
```

### Request Body

```json
{
  "countryCode": "US",
  "durationMinutes": 120
}
```

### Parameters

| Parameter | Type | Required | Constraints | Description |
|-----------|------|----------|-------------|-------------|
| countryCode | string | Yes | Valid ISO country code | Two-letter country code |
| durationMinutes | integer | Yes | 1-1440 | Duration in minutes (1 minute to 24 hours) |

### Response

```json
{
  "data": true,
  "message": "Country US blocked for 120 minutes.",
  "isSuccess": true,
  "errorCode": null
}
```

### Error Responses

**400 Bad Request** - Invalid country code:
```json
{
  "data": null,
  "message": "Invalid Country Code",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

**400 Bad Request** - Invalid duration:
```json
{
  "data": null,
  "message": "Duration must be less than or equal to 1440 minutes (24 hours).",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

**409 Conflict** - Country already temporarily blocked:
```json
{
  "data": null,
  "message": "Country US is already blocked.",
  "isSuccess": false,
  "errorCode": "CONFLICT"
}
```

### Background Cleanup

A Hangfire recurring job runs every 5 minutes to automatically remove expired temporal blocks.

### Example

```bash
# Block US for 2 hours
curl -X POST "http://localhost:5000/api/countries/temporal-block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US", "durationMinutes": 120}'
```

---

## Error Codes

| Error Code | Description |
|------------|-------------|
| VALIDATION_ERROR | Input validation failed |
| NOT_FOUND | Resource not found |
| CONFLICT | Resource conflict (e.g., duplicate) |
| INTERNAL_ERROR | Internal server error |

## Rate Limiting

The IPGeolocation.io API has rate limits:
- **Free tier**: 1,000 requests/month
- **Upgrade available** at [ipgeolocation.io/pricing](https://ipgeolocation.io/pricing)

The application will return appropriate error responses when rate limits are exceeded.

## Best Practices

1. **Always check API responses** for `isSuccess` field before processing data
2. **Use pagination** for large result sets to improve performance
3. **Handle errors gracefully** and provide meaningful user feedback
4. **Monitor blocked attempt logs** to identify security threats
5. **Use temporal blocks** for temporary restrictions
6. **Verify IP addresses** before blocking countries

## Testing Tips

### Test Flow

1. **Block a country**: `POST /api/countries/block`
2. **View blocked countries**: `GET /api/countries`
3. **Check IP**: `GET /api/ip/check-block`
4. **View logs**: `GET /api/logs/blocked-attempts`
5. **Temporarily block**: `POST /api/countries/temporal-block`
6. **Wait and verify** automatic unblock (up to 5 minutes)
7. **Unblock manually**: `DELETE /api/countries/block/{countryCode}`

### Common Test IPs

- `8.8.8.8` - Google DNS (United States)
- `1.1.1.1` - Cloudflare DNS (Australia)
- `208.67.222.222` - OpenDNS (United States)

## Swagger UI

Interactive API documentation is available at:

```
http://localhost:5000/swagger
```

Use the Swagger UI to explore endpoints, test requests, and view response schemas.

