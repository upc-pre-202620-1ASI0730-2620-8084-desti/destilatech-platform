FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY DestilatechPlatform.API/*.csproj DestilatechPlatform.API/
RUN dotnet restore ./DestilatechPlatform.API
COPY . .
RUN dotnet publish ./DestilatechPlatform.API -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DestilatechPlatform.API.dll"]
