# Product Catalog (ASP.NET Core)

CRUD REST API for a product catalog. .NET 8 Web API, in-memory repository,
Swagger UI in development.

## Run locally
```bash
cd src/ProductCatalog.Api
dotnet run
```

## Endpoints
| Method | Path                  | Description         |
|--------|-----------------------|----------------------|
| GET    | /api/products         | List all products    |
| GET    | /api/products/{id}    | Get one product      |
| POST   | /api/products         | Create a product     |
| PUT    | /api/products/{id}    | Update a product     |
| DELETE | /api/products/{id}    | Delete a product      |
| GET    | /health                | Health check          |

## Test
```bash
dotnet test
```

## Build image
```bash
docker build -t product-catalog .
docker run -p 8080:8080 product-catalog
```
