# Quick Reference Guide

Quick reference for common operations and endpoints.

## Base URL

```
http://localhost:5000
```

## Swagger UI

```
http://localhost:5000
```

## Common Endpoints

| Purpose | Method | Endpoint |
|---------|--------|----------|
| Block country | POST | `/api/countries/block` |
| Unblock country | DELETE | `/api/countries/block/{code}` |
| List blocked | GET | `/api/countries` |
| IP lookup | GET | `/api/ip/lookup` |
| Check blocked | GET | `/api/ip/check-block` |
| View logs | GET | `/api/logs/blocked-attempts` |
| Temporal block | POST | `/api/countries/temporal-block` |

## Quick Commands

### Block a Country

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US"}'
```

### Get Blocked Countries

```bash
curl "http://localhost:5000/api/countries?pageNumber=1&pageSize=10"
```

### IP Lookup

```bash
curl "http://localhost:5000/api/ip/lookup?ipAddress=8.8.8.8"
```

### Check if Blocked

```bash
curl "http://localhost:5000/api/ip/check-block"
```

### Unblock Country

```bash
curl -X DELETE "http://localhost:5000/api/countries/block/US"
```

### Temporarily Block (2 hours)

```bash
curl -X POST "http://localhost:5000/api/countries/temporal-block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US", "durationMinutes": 120}'
```

### View Logs

```bash
curl "http://localhost:5000/api/logs/blocked-attempts?pageNumber=1&pageSize=10"
```

## Response Format

```json
{
  "data": { /* response data */ },
  "message": "Success message",
  "isSuccess": true,
  "errorCode": null
}
```

## Common Error Codes

- `VALIDATION_ERROR` - Invalid input
- `CONFLICT` - Duplicate resource
- `NOT_FOUND` - Resource not found

## Test IP Addresses

- `8.8.8.8` - Google DNS (United States)
- `1.1.1.1` - Cloudflare (Australia)
- `208.67.222.222` - OpenDNS (United States)

## Configuration

### appsettings.json

```json
{
  "IpGeolocation": {
    "ApiKey": "your-key",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  }
}
```

## Useful Country Codes

- US - United States
- GB - United Kingdom
- CA - Canada
- AU - Australia
- DE - Germany
- FR - France
- JP - Japan
- CN - China

## Background Jobs

- **Name**: CleanupBlockedAttempts
- **Schedule**: Every 5 minutes
- **Purpose**: Remove expired temporal blocks
- **Dashboard**: `/hangfire`

## Pagination

- Default page size: 5
- Maximum page size: 30
- Query params: `pageNumber`, `pageSize`

## Notes

- All timestamps are in UTC
- Data stored in-memory (lost on restart)
- Temporal blocks expire automatically
- Rate limit: 1,000 requests/month (free tier)

