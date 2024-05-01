FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Executors/CSharp12Executor/CSharp12Executor.csproj", "Executors/CSharp12Executor/"]
COPY ["Dispatcher/CodeExecutor.Dispatcher.Contracts/CodeExecutor.Dispatcher.Contracts.csproj", "Dispatcher/CodeExecutor.Dispatcher.Contracts/"]
COPY ["Infrastructure/CodeExecutor.Messaging/CodeExecutor.Messaging.csproj", "Infrastructure/CodeExecutor.Messaging/"]
COPY ["Common/CodeExecutor.Common.Models/CodeExecutor.Common.Models.csproj", "Common/CodeExecutor.Common.Models/"]
COPY ["Common/CodeExecutor.Common/CodeExecutor.Common.csproj", "Common/CodeExecutor.Common/"]
COPY ["Executors/BaseCSharpExecutor/BaseCSharpExecutor.csproj", "Executors/BaseCSharpExecutor/"]
RUN dotnet restore "Executors/CSharp12Executor/CSharp12Executor.csproj"
COPY . .
WORKDIR "/src/Executors/CSharp12Executor"
RUN dotnet build "CSharp12Executor.csproj" -c Release -o /app/build

FROM build AS publish
ENV ASPNETCORE_ENVIRONMENT Production
RUN dotnet publish "CSharp12Executor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT Production
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharp12Executor.dll"]
