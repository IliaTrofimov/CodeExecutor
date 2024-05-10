using BaseCSharpExecutor;
using BaseCSharpExecutor.Api;

using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging;
using CodeExecutor.Messaging.Abstractions;
using CodeExecutor.Messaging.Abstractions.Services;
using CSharpCodeExecutor;


var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args).AddEnvironmentVariables();


builder.Services.AddSingleton<IConfiguration, ConfigurationManager>();
builder.Services.AddSingleton<IMessageReceiverConfig>(new MessageReceiverConfig(builder.Configuration.GetSection("RabbitMq")));
builder.Services.AddSingleton<IDispatcherApiConfig>(new DispatcherApiConfig(builder.Configuration.GetSection("Api")));

builder.Services.AddSingleton<IMessageReceiver<ExecutionStartMessage>, ExecutionMessageReceiver>();
builder.Services.AddSingleton<ICodeExecutionDispatcherClient, CodeExecutionDispatcherClient>();
builder.Services.AddSingleton<BaseExecutor, CSharpExecutor>();
builder.Services.AddHttpClient<CodeExecutionDispatcherClient>();

builder.Services.AddHostedService<ExecutionWorker>();

var host = builder.Build();
host.Run();