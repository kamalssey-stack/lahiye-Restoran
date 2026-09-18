# Restoran Sifariş İdarəetmə Sistemi (Restaurant Order Management System)

Bu layihə restoran daxilində menyu və sifarişlərin idarə edilməsi üçün hazırlanmış N-Tier arxitekturalı C# / .NET tətbiqidir.

## 🏗️ Arxitektura (N-Tier Layered Architecture)
- **RestaurantApp.Domain**: Entity modelləri (`Category`, `MenuItem`, `Order`, `OrderItem`).
- **RestaurantApp.Data**: EF Core DbContext, Fluent API konfiqurasiyaları, Generic Repository və Unit of Work pattern-ləri.
- **RestaurantApp.Business**: DTOs, AutoMapper profilləri, IMemoryCache keşləmə servisi və biznes məntiqi servisləri (`MenuItemService`, `OrderService`, `CategoryService`).
- **RestaurantApp.ConsoleApp**: İstifadəçi interfeysi və Dependency Injection quraşdırması.
- **RestaurantDb_Script.sql**: SQL Server üçün tam DDL/DML və Stored Procedure skriptləri.

## 🚀 Texnologiyalar
- .NET 10 / C# 13
- Entity Framework Core (SQL Server)
- Generic Repository & Unit of Work Patterns
- AutoMapper
- Memory Caching (IMemoryCache)
- Microsoft Dependency Injection
- Async / Await

## ⚙️ Quraşdırma və İşə Salma
1. SQL Server bağlantı sətrini `RestaurantApp.ConsoleApp/AppDbContextFactory.cs` faylında yoxlayın.
2. Tətbiqi işə salın:
```bash
dotnet run --project RestaurantApp.ConsoleApp/RestaurantApp.ConsoleApp.csproj
```
