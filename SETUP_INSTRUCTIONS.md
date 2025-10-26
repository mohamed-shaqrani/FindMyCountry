# Setup Instructions

Complete guide to setting up and running the IP Geolocation and Country Blocking API.

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

1. **.NET 9.0 SDK**
   - Download from: [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
   - Verify installation:
     ```bash
     dotnet --version
     ```
   - Should show: `9.0.x` or higher

2. **API Key from IPGeolocation.io**
   - Sign up at: [ipgeolocation.io](https://ipgeolocation.io)
   - Get your free API key (1,000 requests/month)
   - Note: Free tier is sufficient for testing

3. **IDE (Optional but Recommended)**
   - Visual Studio 2022
   - Visual Studio Code with C# extension
   - Rider

## Installation Steps

### Step 1: Clone or Download the Project

If you have a Git repository:
```bash
git clone <repository-url>
cd "Interview Question/Main"
```

Or extract the project files to your local directory.

### Step 2: Navigate to Project Directory

```bash
cd Main
```

Project structure:
```
Main/
├── Feature/
│   └── IpGeoLocation/
├── Common/
├── Helpers/
├── Services/
├── Program.cs
├── Main.csproj
└── appsettings.json
```

### Step 3: Configure API Key

Edit `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "IpGeolocation": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  },
  "AllowedHosts": "*"
}
```

**Important**: Replace `YOUR_API_KEY_HERE` with your actual API key from IPGeolocation.io.

### Step 4: Restore NuGet Packages

```bash
dotnet restore
```

This will download all required packages including:
- Microsoft.Extensions.Http
- FluentValidation
- Hangfire
- MediatR
- Autofac
- Serilog
- Swashbuckle (Swagger)

### Step 5: Build the Project

```bash
dotnet build
```

You should see:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Step 6: Run the Application

```bash
dotnet run
```

Or use the project file:
```bash
dotnet run --project Main.csproj
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started.
```

### Step 7: Verify the Setup

1. **Open Swagger UI** in your browser:
   ```
   http://localhost:5000
   ```

2. **Access Hangfire Dashboard**:
   ```
   http://localhost:5000/hangfire
   ```

3. **Test an endpoint** using Swagger UI:
   - Find "GET /api/countries"
   - Click "Try it out"
   - Click "Execute"
   - Verify you get a response

## Port Configuration

If port 5000 is already in use, you can:

### Option 1: Change in launchSettings.json

Edit `Properties/launchSettings.json`:

```json
{
  "applicationUrl": "http://localhost:5001",
  ...
}
```

### Option 2: Use Command Line

```bash
dotnet run --urls "http://localhost:5001"
```

## Development Configuration

### appsettings.Development.json

For development-specific settings, create or edit `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "IpGeolocation": {
    "ApiKey": "YOUR_DEV_API_KEY",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  }
}
```

### Seq Logging (Optional)

The application is configured to use Seq for structured logging:

1. **Install Seq**:
   - Download from: [getseq.net](https://getseq.net/Download)
   - Install and run

2. **Configure in appsettings.json**:
   ```json
   {
     "Serilog": {
       "WriteTo": [
         {
           "Name": "Seq",
           "Args": {
             "serverUrl": "http://localhost:5341"
           }
         }
       ]
     }
   }
   ```

3. **Access Seq UI**:
   ```
   http://localhost:5341
   ```

## Running with Visual Studio

1. Open the solution file: `Template.sln`
2. Set `Main` as the startup project
3. Press `F5` or click the Run button
4. Swagger UI will open automatically

## Running with Visual Studio Code

1. Open the project folder
2. Install recommended extensions:
   - C# (by Microsoft)
   - C# Dev Kit
3. Press `F5` to start debugging
4. Choose ".NET 5+ and .NET Core"

## Docker Setup (Optional)

### Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Main/Main.csproj", "Main/"]
RUN dotnet restore "Main/Main.csproj"
COPY . .
WORKDIR "/src/Main"
RUN dotnet build "Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Main.dll"]
```

### Build and Run

```bash
# Build image
docker build -t ip-geolocation-api .

# Run container
docker run -p 5000:80 -e IpGeolocation__ApiKey=YOUR_KEY ip-geolocation-api
```

## Troubleshooting

### Issue: Port Already in Use

**Error**: `Failed to bind to address http://localhost:5000`

**Solution**:
1. Find and kill the process:
   ```bash
   # Windows
   netstat -ano | findstr :5000
   taskkill /PID <process_id> /F
   
   # Linux/Mac
   lsof -i :5000
   kill -9 <process_id>
   ```
2. Or use a different port (see Port Configuration)

### Issue: API Key Not Working

**Error**: Unauthorized or rate limit errors

**Solution**:
1. Verify your API key at [ipgeolocation.io](https://ipgeolocation.io/account)
2. Check request quota
3. Ensure the key is correctly configured in appsettings.json
4. Test the key manually:
   ```bash
   curl "https://api.ipgeolocation.io/ipgeo?apiKey=YOUR_KEY&ip=8.8.8.8"
   ```

### Issue: NuGet Package Restore Failed

**Error**: `NU1101: Unable to find package`

**Solution**:
1. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   ```
2. Restore packages again:
   ```bash
   dotnet restore
   ```
3. Check your internet connection

### Issue: Build Errors

**Error**: Build fails with compilation errors

**Solution**:
1. Ensure you have .NET 9.0 SDK installed
2. Delete `bin/` and `obj/` folders:
   ```bash
   dotnet clean
   ```
3. Rebuild:
   ```bash
   dotnet build
   ```

### Issue: Swagger UI Not Showing

**Error**: Cannot access Swagger UI

**Solution**:
1. Ensure you're accessing the correct URL
2. Check if the application is running
3. Verify `MapOpenApi()` is called in `Program.cs`
4. Check browser console for errors

### Issue: Hangfire Dashboard Not Accessible

**Error**: 404 on Hangfire dashboard

**Solution**:
1. Verify Hangfire is configured in `Program.cs`
2. Check authentication settings (if any)
3. Ensure the application is running

## Next Steps

After successful setup:

1. **Explore the API** using Swagger UI
2. **Read the API documentation** in `API_DOCUMENTATION.md`
3. **Test the endpoints** with the provided examples
4. **Review the code structure** to understand the implementation

## Additional Resources

- [.NET 9.0 Documentation](https://docs.microsoft.com/en-us/dotnet/core/)
- [IPGeolocation.io Documentation](https://ipgeolocation.io/documentation)
- [Swagger/OpenAPI](https://swagger.io/specification/)
- [Hangfire Documentation](https://docs.hangfire.io/)
- [FluentValidation](https://docs.fluentvalidation.net/)

## Contact

For issues or questions regarding setup, please refer to:
- README.md for general information
- API_DOCUMENTATION.md for API usage
- GitHub repository issues (if applicable)

---

**Happy Coding!** 🚀

