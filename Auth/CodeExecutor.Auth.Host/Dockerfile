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
COPY ["Infrastructure/CodeExecutor.Telemetry/CodeExecutor.Telemetry.csproj", "Infrastructure/CodeExecutor.DB.Telemetry/"]
COPY ["Auth/CodeExecutor.Auth.Host/CodeExecutor.Auth.Host.csproj", "Auth/CodeExecutor.Auth.Host/"]
COPY ["Auth/CodeExecutor.Auth.Contracts/CodeExecutor.Auth.Contracts.csproj", "Auth/CodeExecutor.Auth.Contracts/"]
RUN dotnet restore "Auth/CodeExecutor.Auth.Host/CodeExecutor.Auth.Host.csproj"
COPY . .
WORKDIR "/src/Auth/CodeExecutor.Auth.Host"
RUN dotnet build "CodeExecutor.Auth.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeExecutor.Auth.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeExecutor.Auth.Host.dll"]
