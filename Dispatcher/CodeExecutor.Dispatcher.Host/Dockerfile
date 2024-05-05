FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app 
EXPOSE 80 
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Common/CodeExecutor.Common/CodeExecutor.Common.csproj", "Common/CodeExecutor.Common/"]
COPY ["Common/CodeExecutor.Common.Models/CodeExecutor.Common.Models.csproj", "Common/CodeExecutor.Common.Models/"]
COPY ["Infrastructure/CodeExecutor.DB/CodeExecutor.DB.csproj", "Infrastructure/CodeExecutor.DB/"]
COPY ["Infrastructure/CodeExecutor.DB.Abstractions/CodeExecutor.DB.Abstractions.csproj", "Infrastructure/CodeExecutor.DB.Abstractions/"]
COPY ["Infrastructure/CodeExecutor.Telemetry/CodeExecutor.Telemetry.csproj", "Infrastructure/CodeExecutor.Telemetry/"]
COPY ["Infrastructure/CodeExecutor.Messaging/CodeExecutor.Messaging.csproj", "Infrastructure/CodeExecutor.Messaging/"]
COPY ["Dispatcher/CodeExecutor.Dispatcher.Contracts/CodeExecutor.Dispatcher.Contracts.csproj", "Dispatcher/CodeExecutor.Dispatcher.Contracts/"]
COPY ["Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Host.csproj", "Dispatcher/CodeExecutor.Dispatcher.Host/"]
COPY ["Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Services.csproj", "Dispatcher/CodeExecutor.Dispatcher.Services/"]
RUN dotnet restore "Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Host.csproj"
COPY . .
WORKDIR "/src/Dispatcher/CodeExecutor.Dispatcher.Host"
RUN dotnet build "CodeExecutor.Dispatcher.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeExecutor.Dispatcher.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN dotnet dev-certs https --trust
ENTRYPOINT ["dotnet", "CodeExecutor.Dispatcher.Host.dll"]
