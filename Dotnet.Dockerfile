FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /src
COPY ["server-monitoring.csproj", "./"]
RUN dotnet restore "./server-monitoring.csproj"
COPY . .
WORKDIR /src/.
RUN dotnet build "server-monitoring.csproj" -c Release -o /app/build

FROM build AS publish
RUN apk add --update npm
RUN dotnet publish "server-monitoring.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "server-monitoring.dll"]
