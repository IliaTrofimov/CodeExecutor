FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Executors/CSharpCodeExecutor/CSharpCodeExecutor.csproj", "Executors/CSharpCodeExecutor/"]
COPY ["Common/CodeExecutor.Common.Models/CodeExecutor.Common.Models.csproj", "Common/CodeExecutor.Common.Models/"]
COPY ["Infrastructure/CodeExecutor.Messaging/CodeExecutor.Messaging.csproj", "Infrastructure/CodeExecutor.Messaging/"]
COPY ["Infrastructure/CodeExecutor.Messaging.Abstractions/CodeExecutor.Messaging.Abstractions.csproj", "Infrastructure/CodeExecutor.Messaging.Abstractions/"]
COPY ["Executors/BaseCSharpExecutor/BaseCSharpExecutor.csproj", "Executors/BaseCSharpExecutor/"]
COPY ["Dispatcher/CodeExecutor.Dispatcher.Contracts/CodeExecutor.Dispatcher.Contracts.csproj", "Dispatcher/CodeExecutor.Dispatcher.Contracts/"]
RUN dotnet restore "Executors/CSharpCodeExecutor/CSharpCodeExecutor.csproj"
COPY . .
WORKDIR "/src/Executors/CSharpCodeExecutor"
RUN dotnet build "CSharpCodeExecutor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CSharpCodeExecutor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpCodeExecutor.dll"]
