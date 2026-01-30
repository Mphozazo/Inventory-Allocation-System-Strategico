# Strategico - Inventory Allocation System

## Overview
This project implements a concurrency-safe inventory allocation system in .NET 9 using ASP.NET Core Web API, EF Core, and PostgreSQL, packaged with Docker for easy deployment.

### Tech Stack
- .NET 9
- ASP.NET Core Web API
- EF Core
- PostgreSQL / SQL Server
- Docker


### Key Concepts
- Transactional stock reservation
- Optimistic concurrency
- Atomic updates
- SQL as consistency authority
- Atomic stock allocation
-  Minimal, clean design
-  Dockerized with Swagger UI

---

## System Architecture Diagram

```
Client (Web/Mobile)
       |
       v
ASP.NET Core Web API (.NET 9)
       |
       v
Controllers (OrdersController)
       |
       v
DbContext (InventoryDbContext)
       |
       v
PostgreSQL Database
       |
       +--> Stocks Table (TotalQuantity, ReservedQuantity)
       +--> Orders Table
       +--> OrderLines Table

Flow:
1. Client submits an order via API.
2. Controller handles the request.
3. DbContext performs atomic stock check & reservation.
4. If sufficient stock, order is created and committed.
5. Database enforces concurrency & transactional consistency.
```

---


# Trade-offs

| Choice | Benefit | Trade-off |
|------|--------|-----------|
| SQL locking | Strong consistency | Lower throughput |
| Transactions | Data safety | Latency |
| Central allocation | Simple logic | Scalability |
| Optimistic concurrency | Performance | Retry logic |

---
