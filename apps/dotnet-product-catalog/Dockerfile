# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ProductCatalog.Api/ProductCatalog.Api.csproj src/ProductCatalog.Api/
RUN dotnet restore src/ProductCatalog.Api/ProductCatalog.Api.csproj
COPY src/ProductCatalog.Api/ src/ProductCatalog.Api/
WORKDIR /src/src/ProductCatalog.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN adduser --disabled-password --gecos "" appuser
WORKDIR /app
COPY --from=build /app/publish .
USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
HEALTHCHECK --interval=30s --timeout=3s CMD wget -qO- http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]
