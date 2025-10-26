# Deployment Guide

Comprehensive guide for deploying the IP Geolocation and Country Blocking API to different environments.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Local Development](#local-development)
3. [Docker Deployment](#docker-deployment)
4. [Production Deployment](#production-deployment)
5. [Configuration Management](#configuration-management)
6. [Monitoring and Logging](#monitoring-and-logging)
7. [Troubleshooting](#troubleshooting)

## Prerequisites

### Hardware Requirements

- **CPU**: 1 core minimum (2+ recommended)
- **Memory**: 512MB minimum (1GB+ recommended)
- **Storage**: 100MB for application + logs

### Software Requirements

- .NET 9.0 Runtime (Linux/Windows)
- Docker (optional)
- Nginx or IIS (production reverse proxy)

### API Requirements

- IPGeolocation.io API key
- Valid network access for API calls

## Local Development

### Running Locally

```bash
# Navigate to project directory
cd Main

# Restore packages
dotnet restore

# Run application
dotnet run
```

### Development Configuration

**appsettings.Development.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "IpGeolocation": {
    "ApiKey": "your-dev-key",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  }
}
```

## Docker Deployment

### Building Docker Image

```bash
# Build image
docker build -t ip-geolocation-api:latest .

# Tag for registry
docker tag ip-geolocation-api:latest your-registry/ip-geolocation-api:latest
```

### Running Docker Container

```bash
# Run with environment variables
docker run -d \
  -p 5000:80 \
  -e IpGeolocation__ApiKey="your-api-key" \
  -e IpGeolocation__BaseUrl="https://api.ipgeolocation.io/ipgeo" \
  --name ip-geolocation-api \
  ip-geolocation-api:latest
```

### Docker Compose

**docker-compose.yml**:
```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - IpGeolocation__ApiKey=${API_KEY}
      - IpGeolocation__BaseUrl=https://api.ipgeolocation.io/ipgeo
    restart: unless-stopped
    networks:
      - app-network

networks:
  app-network:
    driver: bridge
```

**Run with Docker Compose**:
```bash
docker-compose up -d
```

### Multi-Stage Dockerfile

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Main/Main.csproj", "Main/"]
RUN dotnet restore "Main/Main.csproj"
COPY . .
WORKDIR "/src/Main"
RUN dotnet build "Main.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Main.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "Main.dll"]
```

## Production Deployment

### 1. Azure App Service

#### Create App Service

```bash
# Create resource group
az group create --name rg-ipgeolocation --location eastus

# Create app service plan
az appservice plan create \
  --name plan-ipgeolocation \
  --resource-group rg-ipgeolocation \
  --sku B1

# Create web app
az webapp create \
  --name api-ipgeolocation \
  --resource-group rg-ipgeolocation \
  --plan plan-ipgeolocation
```

#### Configure Settings

```bash
# Set API key
az webapp config appsettings set \
  --name api-ipgeolocation \
  --resource-group rg-ipgeolocation \
  --settings IpGeolocation__ApiKey="your-api-key"
```

#### Deploy Application

```bash
# Deploy using ZIP
az webapp deployment source config-zip \
  --name api-ipgeolocation \
  --resource-group rg-ipgeolocation \
  --src app.zip
```

### 2. AWS Elastic Beanstalk

#### Prerequisites

- AWS CLI installed and configured
- EB CLI installed

#### Deploy

```bash
# Initialize EB
eb init -p dotnet:9.0

# Create environment
eb create production

# Deploy
eb deploy
```

#### Configure Environment

```bash
# Set environment variables
eb setenv IpGeolocation__ApiKey="your-api-key"
```

### 3. Linux VM Deployment

#### Install .NET Runtime

```bash
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0

# Add to PATH
export PATH=$PATH:$HOME/.dotnet
```

#### Create Systemd Service

**/etc/systemd/system/ipgeolocation.service**:
```ini
[Unit]
Description=IP Geolocation API
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/ipgeolocation/Main.dll
Restart=on-failure
RestartSec=10
User=www-data
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=IpGeolocation__ApiKey="your-api-key"
Environment=IpGeolocation__BaseUrl="https://api.ipgeolocation.io/ipgeo"

[Install]
WantedBy=multi-user.target
```

#### Enable and Start

```bash
sudo systemctl enable ipgeolocation
sudo systemctl start ipgeolocation
sudo systemctl status ipgeolocation
```

### 4. Nginx Reverse Proxy

#### Install Nginx

```bash
sudo apt-get update
sudo apt-get install nginx
```

#### Configure Nginx

**/etc/nginx/sites-available/ipgeolocation**:
```nginx
server {
    listen 80;
    server_name api.yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

#### Enable Site

```bash
sudo ln -s /etc/nginx/sites-available/ipgeolocation /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### 5. IIS Deployment (Windows)

#### Install .NET Hosting Bundle

Download and install from: https://dotnet.microsoft.com/download

#### Create Application Pool

1. Open IIS Manager
2. Create new Application Pool
   - Name: `ipgeolocation`
   - .NET CLR Version: No Managed Code
   - Managed Pipeline Mode: Integrated

#### Deploy Application

1. Create site in IIS
2. Point to published application folder
3. Configure application pool
4. Set environment variables in web.config

**web.config**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\Main.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout">
        <environmentVariables>
          <environmentVariable name="IpGeolocation__ApiKey" value="your-api-key" />
          <environmentVariable name="IpGeolocation__BaseUrl" value="https://api.ipgeolocation.io/ipgeo" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

## Configuration Management

### Environment Variables

Use environment variables for sensitive data:

```bash
# Linux/Mac
export IpGeolocation__ApiKey="your-api-key"

# Windows PowerShell
$env:IpGeolocation__ApiKey="your-api-key"

# Windows CMD
set IpGeolocation__ApiKey=your-api-key
```

### Configuration Hierarchy

1. Environment variables
2. `appsettings.{Environment}.json`
3. `appsettings.json`

### Production Configuration

**appsettings.Production.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "IpGeolocation": {
    "ApiKey": "",
    "BaseUrl": "https://api.ipgeolocation.io/ipgeo"
  }
}
```

## Monitoring and Logging

### Application Insights (Azure)

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-instrumentation-key"
  }
}
```

### Serilog Configuration

**appsettings.Production.json**:
```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/ipgeolocation/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

### Health Checks

Add to `Program.cs`:

```csharp
builder.Services.AddHealthChecks();

app.MapHealthChecks("/health");
```

## Troubleshooting

### Common Deployment Issues

#### Issue: Application Won't Start

**Solution**:
```bash
# Check logs
docker logs ip-geolocation-api

# Or systemd
journalctl -u ipgeolocation -f

# Check port availability
netstat -an | grep 5000
```

#### Issue: API Key Not Working

**Solution**:
```bash
# Verify environment variable
echo $IpGeolocation__ApiKey

# Test API key
curl "https://api.ipgeolocation.io/ipgeo?apiKey=YOUR_KEY&ip=8.8.8.8"
```

#### Issue: Port Already in Use

**Solution**:
```bash
# Find process
lsof -i :5000

# Kill process
kill -9 <PID>
```

### Production Checklist

- [ ] Environment variables configured
- [ ] API key set
- [ ] SSL/TLS configured
- [ ] Firewall rules configured
- [ ] Monitoring enabled
- [ ] Logging configured
- [ ] Backup strategy in place
- [ ] Health checks working
- [ ] Load balancer configured (if needed)
- [ ] Rate limiting configured
- [ ] Error tracking enabled

## Scaling Considerations

### Horizontal Scaling

- Use load balancer
- Shared state in Redis (if needed)
- Stateless design supports scaling

### Vertical Scaling

- Increase VM resources
- Optimize memory usage
- Monitor performance

### Database Migration

For production persistence, consider:
- SQL Server
- PostgreSQL
- Azure Cosmos DB

Update `CountryIPTracker` and `BlockedAttemptStore` to use database instead of in-memory storage.

## Security Recommendations

1. **Enable HTTPS**: Use SSL/TLS certificates
2. **Add Authentication**: Implement JWT or API keys
3. **Rate Limiting**: Prevent abuse
4. **Input Validation**: Already implemented with FluentValidation
5. **CORS**: Configure appropriately
6. **Secrets Management**: Use Azure Key Vault, AWS Secrets Manager, etc.

## Maintenance

### Update Application

```bash
# Stop service
sudo systemctl stop ipgeolocation

# Deploy new version
# ...deploy new files...

# Restart service
sudo systemctl start ipgeolocation
```

### View Logs

```bash
# Application logs
tail -f /var/log/ipgeolocation/log-*.txt

# System logs
journalctl -u ipgeolocation -f

# Docker logs
docker logs -f ip-geolocation-api
```

### Backup and Restore

Since data is in-memory, implement:
- Periodic snapshots (if needed)
- Database persistence (recommended for production)
- Configuration backups

---

**For additional deployment scenarios or questions, refer to the main documentation or contact the development team.**

