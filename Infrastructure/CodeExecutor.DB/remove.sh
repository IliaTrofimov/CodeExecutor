#!/bin/zsh

if [[ $# -eq 1 && $1 -eq 'prod' ]]; then 
  $ export DOTNET_ENVIRONMENT=Production
  echo "[INFO] Removing migration from prod"
else  
  $ export DOTNET_ENVIRONMENT=Development
  echo "[INFO] Removing migration from dev"
fi

dotnet dotnet-ef database update \
    --startup-project "../../Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Host.csproj"