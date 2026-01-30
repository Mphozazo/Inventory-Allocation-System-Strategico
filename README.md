# Strategico - Inventory Allocation System

## Overview
This project implements a warehouse-aware, concurrency-safe inventory allocation system in .NET 9 using ASP.NET Core Web API, EF Core, and PostgreSQL, packaged with Docker.

### Tech Stack
- .NET 9
- ASP.NET Core Web API
- EF Core
- PostgreSQL / SQL Server
- Docker


### Key Concepts
- Transactional stock reservation
- Warehouse-as-tenant via HTTP header X-Warehouse-Id
- Optimistic concurrency
- Atomic updates
- SQL as consistency authority
- Atomic stock allocation
- Minimal, clean design
- Dockerized with Swagger UI

# Warehouse-as-Tenant Approach
- Warehouse ID is passed via header X-Warehouse-Id.
- Scoped WarehouseProvider reads the header and provides WarehouseId.
- EF Core global query filter restricts Stock queries to that warehouse.
- Controllers remain simple; no need to pass warehouse IDs in requests.

---

## System Architecture Diagram

```
Client (Web/Mobile) --> sends header X-Warehouse-Id
       |
       v
ASP.NET Core Web API (.NET 9)
       |
       v
OrdersController
       |
       v
InventoryDbContext (with WarehouseProvider query filter)
       |
       v
PostgreSQL Database
       |
       +--> Stocks Table (TotalQuantity, ReservedQuantity, WarehouseId)
       +--> Orders Table
       +--> OrderLines Table

Flow:
1. Client submits an order with X-Warehouse-Id header.
2. WarehouseProvider reads the header and exposes WarehouseId.
3. DbContext applies a global query filter on Stock based on WarehouseId.
4. PlaceOrder performs atomic update to reserve stock.
5. Order and OrderLine are created if stock is sufficient.
6. Transaction ensures atomicity and concurrency safety.
```
---


# Trade-offs

| Choice | Benefit | Trade-off |
|------|--------|-----------|
| SQL locking | Strong consistency | Lower throughput |
| Transactions | Data safety | Latency |
| Central allocation | Simple logic | Scalability |
| Warehouse-as-tenant | Clean separation per warehouse | Header required for all requests
| Optimistic concurrency | Performance 

---
## Setup and Run

### Clone the repository
```powershell
git clone <your-repo-url>
cd Inventory-Allocation-System-Strategico
```

### Run the application using Docker Compose
This command will build the images and start the containers:
```powershell
docker compose up --build -d
```

### Stop and remove containers, networks, and volumes (Clean Start)
```powershell
docker compose down -v --remove-orphans
```

## Database
The system uses PostgreSQL. Connection details are configured via `docker-compose.yml`:
```yaml
ConnectionStrings__DefaultConnection: "Host=db;Port=5432;Database=inventorydb;Username=inventory;Password=inventory"
```

### Seed Data
Seed data is automatically inserted on startup. This includes:
- One Warehouse (`Main Warehouse`)
- Products (`Laptop`, `Phone`)
- Stock for each product in the warehouse


## API Endpoints
### Place an Order
- **URL:** `/api/v1/orders`
- **Method:** `POST`
- **Body Example:**
```json
{
  "lines": [
    { "productId": "<LaptopId>", "quantity": 2 },
    { "productId": "<PhoneId>", "quantity": 3 }
  ]
}
```

