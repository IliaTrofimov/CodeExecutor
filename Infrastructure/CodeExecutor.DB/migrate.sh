#!/bin/zsh

if [ $# -eq 0 ]; then
    echo "[ERROR] Missing migration name"
    exit 1
fi

echo "[INFO] Building migration files"
dotnet dotnet-ef migrations add "$1" \
    --output-dir "./Migrations" \
    --startup-project "../../Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Host.csproj"


if [[ $# -ge 2 && $2 -eq 'update' ]]; then 
  if [[ $# -eq 3 && $3 -eq 'prod' ]]; then 
    export DOTNET_ENVIRONMENT=Production
    echo "[INFO] Applying migration to prod"
  else  
    export DOTNET_ENVIRONMENT=Development
    echo "[INFO] Applying migration to dev"
  fi
  
  dotnet dotnet-ef database update \
      --startup-project "../../Dispatcher/CodeExecutor.Dispatcher.Host/CodeExecutor.Dispatcher.Host.csproj"
fi
