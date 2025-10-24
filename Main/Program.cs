using Autofac;
using Autofac.Extensions.DependencyInjection;
using Main.Common.Config;
using Main.Extensions;
using Main.Helpers;
using Main.Services;
using MediatR;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCompressionServices();
builder.Services.Configure<IpGeolocationOptions>(
    builder.Configuration.GetSection("IpGeolocation"));

// Register HttpClient for the IPGeolocation service
builder.Services.AddHttpClient<IpGeolocationService>();
builder.Services.AddMediatR(AssemblyReference.Assembly);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new AutofacModule());
});
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = ""; // Set Swagger at root (optional)
});

app.UseAuthentication();
app.UseDeveloperExceptionPage();

app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
