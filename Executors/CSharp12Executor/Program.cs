using BaseCSharpExecutor;
using BaseCSharpExecutor.Api;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging;
using CodeExecutor.Messaging.Abstractions.Services;


var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;


builder.Services.AddSingleton<IConfiguration, ConfigurationManager>();
builder.Services.AddSingleton(new MessageReceiverConfig(config.GetSection("RabbitMq")));
builder.Services.AddSingleton(new DispatcherApiConfig(config.GetSection("Api")));

builder.Services.AddSingleton<IMessageReceiver<ExecutionStartMessage>, ExecutionMessageReceiver>();
builder.Services.AddSingleton<ICodeExecutionDispatcherClient, CodeExecutionDispatcherClient>();
builder.Services.AddSingleton<BaseExecutor, CSharp12Executor.CSharpExecutor>();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<ExecutionWorker>();

var host = builder.Build();
host.Run();