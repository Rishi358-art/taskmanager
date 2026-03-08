FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY TaskManager.Web/bin/Release/net10.0/publish/ .
ENTRYPOINT ["dotnet", "TaskManager.Web.dll"]