# BelanjaYuk - E-Commerce API

BelanjaYuk adalah platform e-commerce berbasis .NET 8 dengan fitur lengkap untuk buyer dan seller.

## Tech Stack

- **.NET 8** - Framework backend
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **Docker & Docker Compose** - Containerization
- **JWT Authentication** - Security
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=belanjayuk;..."
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "http://api-dev.drian.my.id",
    "Audience": "http://api-dev.drian.my.id"
  }
}
```

## API Documentation

Swagger UI tersedia di:

- **Development**: `https://api-dev.drian.my.id/swagger`
- **Production**: Swagger disabled (production mode)

## Domain

- Repository: https://github.com/IT-DIV/BelanjaYuk-BE
- API Dev: https://api-dev.drian.my.id
- API Prod: https://api-prod.drian.my.id
