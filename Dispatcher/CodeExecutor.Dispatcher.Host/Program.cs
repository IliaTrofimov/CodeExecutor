﻿using CodeExecutor.Common.Logging;
using CodeExecutor.Common.Middleware;
using CodeExecutor.Common.Security;
using CodeExecutor.DB;
using CodeExecutor.Dispatcher.Host;
using CodeExecutor.Telemetry;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddCommandLine(args).AddEnvironmentVariables();
builder.Logging.ClearProviders();

builder.Services.AddControllers(opt => opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddRouting(opt => opt.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = builder.Environment.ApplicationName, Version = "v1" });
});

builder.Services.AddConsoleLogger();
builder.Services.AddJwtBearer(builder.Configuration);
builder.Services.AddDataBase(builder.Configuration);
builder.Services.AddConfigs(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddTelemetry(builder.Configuration, builder.Environment);


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<DefaultExceptionHandler>();
app.UseMiddleware<DefaultHttpLogger>();
app.UseMiddleware<ServiceInfoMiddleware>(builder);
app.UseRouting();
app.UseDefaultCors(builder.Configuration);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();