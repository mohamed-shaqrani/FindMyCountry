# Testing Guide

Complete guide for testing the IP Geolocation and Country Blocking API.

## Table of Contents

1. [Quick Start](#quick-start)
2. [Test Scenarios](#test-scenarios)
3. [Using Swagger UI](#using-swagger-ui)
4. [Using cURL](#using-curl)
5. [Using Postman](#using-postman)
6. [Integration Tests](#integration-tests)
7. [Common Issues](#common-issues)

## Quick Start

### Prerequisites

- Application running on `http://localhost:5000`
- Swagger UI accessible
- cURL or Postman installed (optional)

### Verify Application is Running

```bash
curl http://localhost:5000/swagger
```

## Test Scenarios

### Scenario 1: Basic Country Blocking

**Goal**: Test adding, viewing, and removing blocked countries.

#### Step 1: Block a Country

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US"}'
```

**Expected Response**:
```json
{
  "data": true,
  "message": "Country blocked successfully.",
  "isSuccess": true,
  "errorCode": null
}
```

#### Step 2: View Blocked Countries

```bash
curl "http://localhost:5000/api/countries?pageNumber=1&pageSize=10"
```

**Expected Response**:
```json
{
  "data": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1,
    "items": [
      {
        "countryCode": "US",
        "blockedAt": "2024-01-15T10:30:00Z",
        "isTemporarilyBlocked": false
      }
    ]
  },
  "message": "Success",
  "isSuccess": true,
  "errorCode": null
}
```

#### Step 3: Unblock the Country

```bash
curl -X DELETE "http://localhost:5000/api/countries/block/US"
```

**Expected Response**:
```json
{
  "data": true,
  "message": "Country Unblocked successfully.",
  "isSuccess": true,
  "errorCode": null
}
```

#### Step 4: Verify Removed

```bash
curl "http://localhost:5000/api/countries"
```

**Expected**: Empty list or 404

---

### Scenario 2: IP Geolocation Lookup

**Goal**: Test IP address lookup and geolocation.

#### Step 1: Lookup Specific IP

```bash
curl "http://localhost:5000/api/ip/lookup?ipAddress=8.8.8.8"
```

**Expected Response**:
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

#### Step 2: Lookup Without IP (Use Caller IP)

```bash
curl "http://localhost:5000/api/ip/lookup"
```

**Expected**: Response with your IP's country details

#### Step 3: Test Invalid IP

```bash
curl "http://localhost:5000/api/ip/lookup?ipAddress=invalid"
```

**Expected Error**:
```json
{
  "data": null,
  "message": "Invalid IP Address",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

---

### Scenario 3: IP Blocking Verification

**Goal**: Test automatic IP blocking check and logging.

#### Step 1: Block a Country

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US"}'
```

#### Step 2: Check if IP is Blocked

```bash
curl "http://localhost:5000/api/ip/check-block"
```

**Expected Response** (if your IP is from US):
```json
{
  "data": true,
  "message": "Country is blocked.",
  "isSuccess": true,
  "errorCode": null
}
```

**Expected Response** (if your IP is not from US):
```json
{
  "data": false,
  "message": "Country is not blocked.",
  "isSuccess": true,
  "errorCode": null
}
```

#### Step 3: View Blocked Attempt Logs

```bash
curl "http://localhost:5000/api/logs/blocked-attempts?pageNumber=1&pageSize=10"
```

**Expected Response**:
```json
{
  "data": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1,
    "items": [
      {
        "ipAddress": "your-ip",
        "countryCode": "US",
        "isBlocked": true,
        "timestampUtc": "2024-01-15T10:30:00Z",
        "userAgent": "curl/7.xx.x"
      }
    ]
  },
  "message": "Success",
  "isSuccess": true,
  "errorCode": null
}
```

---

### Scenario 4: Temporal Country Blocking

**Goal**: Test time-limited country blocking with automatic expiration.

#### Step 1: Temporarily Block a Country

```bash
curl -X POST "http://localhost:5000/api/countries/temporal-block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "GB", "durationMinutes": 2}'
```

**Expected Response**:
```json
{
  "data": true,
  "message": "Country GB blocked for 2 minutes.",
  "isSuccess": true,
  "errorCode": null
}
```

#### Step 2: Verify Temporal Block

```bash
curl "http://localhost:5000/api/countries?countryCode=GB"
```

**Expected**: Country should appear in blocked list

#### Step 3: Wait and Check Expiration

Wait 2 minutes + 5 minutes (for Hangfire cleanup), then:

```bash
curl "http://localhost:5000/api/countries?countryCode=GB"
```

**Expected**: Country should be automatically removed

#### Step 4: Test Duplicate Temporal Block

```bash
# Try to add same country again
curl -X POST "http://localhost:5000/api/countries/temporal-block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "GB", "durationMinutes": 5}'
```

**Expected Error**:
```json
{
  "data": null,
  "message": "Country GB is already blocked.",
  "isSuccess": false,
  "errorCode": "CONFLICT"
}
```

---

### Scenario 5: Pagination and Filtering

**Goal**: Test pagination and search functionality.

#### Step 1: Add Multiple Countries

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US"}'

curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "GB"}'

curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "CA"}'
```

#### Step 2: Test Pagination

```bash
# Page 1 with 2 items
curl "http://localhost:5000/api/countries?pageNumber=1&pageSize=2"

# Page 2 with 2 items
curl "http://localhost:5000/api/countries?pageNumber=2&pageSize=2"
```

#### Step 3: Test Filtering

```bash
# Search for specific country
curl "http://localhost:5000/api/countries?countryCode=US"
```

---

### Scenario 6: Error Handling

**Goal**: Test validation and error responses.

#### Test Invalid Country Code

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "XX"}'
```

**Expected Error**:
```json
{
  "data": null,
  "message": "Invalid Country Code",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

#### Test Invalid Duration

```bash
curl -X POST "http://localhost:5000/api/countries/temporal-block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US", "durationMinutes": 9999}'
```

**Expected Error**:
```json
{
  "data": null,
  "message": "Duration must be less than or equal to 1440 minutes (24 hours).",
  "isSuccess": false,
  "errorCode": "VALIDATION_ERROR"
}
```

#### Test Empty Request

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{}'
```

**Expected Error**: Validation error for required field

---

## Using Swagger UI

### Access Swagger UI

1. Navigate to `http://localhost:5000`
2. You'll see the interactive API documentation
3. Click "Try it out" on any endpoint
4. Fill in parameters
5. Click "Execute"
6. View response

### Tips

- Use "Authorize" button for authentication (if configured)
- Expand/collapse schemas to view request/response models
- Test different scenarios easily

---

## Using cURL

### Basic cURL Commands

#### GET Request

```bash
curl "http://localhost:5000/api/countries"
```

#### POST Request

```bash
curl -X POST "http://localhost:5000/api/countries/block" \
  -H "Content-Type: application/json" \
  -d '{"countryCode": "US"}'
```

#### PUT Request

```bash
curl -X PUT "http://localhost:5000/api/endpoint" \
  -H "Content-Type: application/json" \
  -d '{"key": "value"}'
```

#### DELETE Request

```bash
curl -X DELETE "http://localhost:5000/api/countries/block/US"
```

#### With Query Parameters

```bash
curl "http://localhost:5000/api/countries?pageNumber=1&pageSize=10"
```

#### With Custom Headers

```bash
curl "http://localhost:5000/api/endpoint" \
  -H "Authorization: Bearer token"
```

#### Save Response to File

```bash
curl "http://localhost:5000/api/countries" > response.json
```

#### Verbose Mode (Debug)

```bash
curl -v "http://localhost:5000/api/countries"
```

---

## Using Postman

### Setup

1. Open Postman
2. Create new collection: "IP Geolocation API"
3. Add environment variables:
   - `base_url`: `http://localhost:5000`

### Import Collection

Create a Postman collection with these requests:

#### Request 1: Block Country

- Method: POST
- URL: `{{base_url}}/api/countries/block`
- Headers:
  - `Content-Type: application/json`
- Body (raw JSON):
```json
{
  "countryCode": "US"
}
```

#### Request 2: Get Blocked Countries

- Method: GET
- URL: `{{base_url}}/api/countries?pageNumber=1&pageSize=10`

#### Request 3: IP Lookup

- Method: GET
- URL: `{{base_url}}/api/ip/lookup?ipAddress=8.8.8.8`

#### Request 4: Check Blocked

- Method: GET
- URL: `{{base_url}}/api/ip/check-block`

#### Request 5: View Logs

- Method: GET
- URL: `{{base_url}}/api/logs/blocked-attempts?pageNumber=1&pageSize=10`

#### Request 6: Unblock Country

- Method: DELETE
- URL: `{{base_url}}/api/countries/block/US`

#### Request 7: Temporal Block

- Method: POST
- URL: `{{base_url}}/api/countries/temporal-block`
- Body (raw JSON):
```json
{
  "countryCode": "GB",
  "durationMinutes": 120
}
```

### Run Collection

1. Select collection
2. Click "Run"
3. View results
4. Save test results

---

## Integration Tests

### Using xUnit (Example)

```csharp
public class CountryBlockingTests
{
    private readonly HttpClient _client;

    public CountryBlockingTests()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BlockCountry_Success()
    {
        var response = await _client.PostAsync("/api/countries/block",
            new StringContent(
                JsonSerializer.Serialize(new { countryCode = "US" }),
                Encoding.UTF8,
                "application/json"));

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task BlockCountry_Duplicate_ReturnsError()
    {
        // Block country
        await _client.PostAsync("/api/countries/block",
            new StringContent(
                JsonSerializer.Serialize(new { countryCode = "US" }),
                Encoding.UTF8,
                "application/json"));

        // Try to block again
        var response = await _client.PostAsync("/api/countries/block",
            new StringContent(
                JsonSerializer.Serialize(new { countryCode = "US" }),
                Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
```

---

## Common Issues

### Issue: 404 Not Found

**Solution**: 
- Check endpoint URL
- Ensure application is running
- Verify route configuration

### Issue: API Key Not Working

**Solution**:
- Verify API key in appsettings.json
- Check rate limits at ipgeolocation.io
- Test API key directly

### Issue: CORS Error

**Solution**:
- Add CORS policy in Program.cs
- Configure allowed origins

### Issue: Localhost Connection Refused

**Solution**:
- Ensure application is running
- Check port number
- Verify firewall settings

---

## Performance Testing

### Load Testing with Apache Bench

```bash
# 100 requests, 10 concurrent
ab -n 100 -c 10 http://localhost:5000/api/countries
```

### Load Testing with Artillery

```yaml
config:
  target: http://localhost:5000
  phases:
    - duration: 60
      arrivalRate: 10
scenarios:
  - name: Test API
    flow:
      - get:
          url: /api/countries
```

---

## Success Criteria

✅ All endpoints return appropriate status codes
✅ Validation errors are caught and returned
✅ Pagination works correctly
✅ Filtering works as expected
✅ Temporal blocks expire automatically
✅ Blocked attempts are logged
✅ Thread-safe operations
✅ Error handling works properly

---

**Happy Testing!** 🚀

