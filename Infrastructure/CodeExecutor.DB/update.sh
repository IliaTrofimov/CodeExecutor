#!/bin/zsh

if [[ $# -eq 1 && $1 -eq 'prod' ]]; then 
  export DOTNET_ENVIRONMENT=Production
  echo "[INFO] Applying migration to prod"
else  
  export DOTNET_ENVIRONMENT=Development
  echo "[INFO] Applying migration to dev"
fi

dotnet dotnet-ef database update \
    --startup-project "../../Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Host.csproj"