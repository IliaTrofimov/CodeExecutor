using BaseCSharpExecutor;
using BaseCSharpExecutor.Api;

using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging;
using CodeExecutor.Messaging.Abstractions.Services;

using CSharpCodeExecutor;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args).AddEnvironmentVariables();


builder.Services.AddSingleton<IConfiguration, ConfigurationManager>();
builder.Services.AddSingleton(new MessageReceiverConfig(builder.Configuration.GetSection("RabbitMq")));
builder.Services.AddSingleton(new DispatcherApiConfig(builder.Configuration.GetSection("Api")));

builder.Services.AddSingleton<IMessageReceiver<ExecutionStartMessage>, ExecutionMessageReceiver>();
builder.Services.AddSingleton<ICodeExecutionDispatcherClient, CodeExecutionDispatcherClient>();
builder.Services.AddSingleton<BaseExecutor, CSharp12Executor>();
builder.Services.AddHttpClient();

builder.Services.AddHostedService<ExecutionWorker>();

IHost host = builder.Build();
host.Run();