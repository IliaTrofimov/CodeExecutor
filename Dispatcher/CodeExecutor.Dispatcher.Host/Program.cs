﻿using System.Reflection;
using CodeExecutor.Common.Logging;
using CodeExecutor.Common.Middleware;
using CodeExecutor.Common.Security;
using CodeExecutor.DB;
using CodeExecutor.Dispatcher.Host;
using Microsoft.OpenApi.Models;


var project = Assembly.GetCallingAssembly().GetName().Name!;
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddConfigs(config);

builder.Logging.ClearProviders();
builder.Services.AddConsoleLogger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = project.Replace(".Host", ""), Version = "v1" });
});

builder.Services.AddControllers(opt => opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddRouting(opt => opt.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwtBearer(config);

builder.Services.AddDataBase(config);
builder.Services.AddServices(config);


var app = builder.Build();

app.MapHealthChecks("/health");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<DefaultExceptionHandler>();
app.UseMiddleware<DefaultHttpLogger>();
//app.UseHttpsRedirection();
app.UseRouting();
app.UseDefaultCors(config);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();