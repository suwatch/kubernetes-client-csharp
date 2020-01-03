FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
WORKDIR /app
COPY bin/Debug/netcoreapp2.2/publish/ ./
ENTRYPOINT ["dotnet", "controller.dll"]
