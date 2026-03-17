# Order Management API

API REST para gestión de pedidos con validación externa, transacciones y auditoría.

## Requisitos

- .NET 8 SDK
- SQL Server (local o Docker)
- Docker (opcional)

## Estructura del Proyecto

```
prueba-pedidos/
├── src/
│   ├── OrderManagement.API/           # Capa de presentación (Controllers, Middlewares)
│   ├── OrderManagement.Application/   # Capa de aplicación (Services, DTOs)
│   ├── OrderManagement.Domain/        # Capa de dominio (Entities, Interfaces)
│   └── OrderManagement.Infrastructure/# Capa de infraestructura (Repositories, ExternalServices)
├── data/
│   └── db.sql                         # Script de base de datos
├── docker-compose.yml                  # Contenedor SQL Server
├── appsettings.json                     # Configuración
└── prueba-pedidos.csproj               # Proyecto principal
```

## Configuración

### Variables de Entorno (appsettings.json)

| Variable | Descripción | Valor por defecto |
|----------|-------------|-------------------|
| `ConnectionStrings:DefaultConnection` | Cadena de conexión SQL Server | `localhost,1433` |
| `ExternalServices:ValidationUrl` | URL de validación de cliente | `https://jsonplaceholder.typicode.com/users/1` |

## Instalación y Ejecución

### 1. Levantar SQL Server con Docker

```bash
docker-compose up -d
```

### 2. Ejecutar Script de Base de Datos

Conectar a SQL Server y ejecutar `data/db.sql`:

### 3. Compilar y Ejecutar

```bash
# Restaurar dependencias
dotnet restore

# Ejecutar
dotnet run --project prueba-pedidos.csproj
```

La API estará disponible en: `http://localhost:5287`

### 4. Documentación Swagger

En desarrollo: `http://localhost:5287/swagger`

---

## Uso de la API

### Crear Pedido

**Endpoint:** `POST /api/pedidos`

**Request:**
```json
{
  "clienteId": 1,
  "usuario": "agustinmites",
  "items": [
    {
      "productoId": 1,
      "cantidad": 2,
      "precio": 1500.00
    },
    {
      "productoId": 2,
      "cantidad": 1,
      "precio": 89.99
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "clienteId": 1,
  "usuario": "agustinmites",
  "fecha": "2026-03-17T10:30:00Z",
  "total": 3089.99,
  "items": [
    {
      "productoId": 1,
      "cantidad": 2,
      "precio": 1500.00
    },
    {
      "productoId": 2,
      "cantidad": 1,
      "precio": 89.99
    }
  ]
}
```

### Códigos de Respuesta

| Código | Descripción |
|--------|-------------|
| 201 | Pedido creado exitosamente |
| 400 | Solicitud inválida (datos mal formateados) |
| 422 | Error de validación (cliente no válido o productos no existen) |
| 502 | Error al conectar con servicio externo |
| 500 | Error interno del servidor |

---

## Arquitectura

### Capas

1. **API Layer** - Controllers y Middlewares
2. **Application Layer** - Servicios y DTOs
3. **Domain Layer** - Entidades e Interfaces
4. **Infrastructure Layer** - Repositorios y Servicios Externos

### Flujo de Creación de Pedido

```
1. Cliente → POST /api/pedidos
2. PedidosController → IOrderService.CreateOrderAsync()
3. OrderService:
   ├── Inicia transacción
   ├── LogAuditory (INICIO)
   ├── Valida cliente con servicio externo (JSONPlaceholder)
   ├── Verifica existencia de productos
   ├── Crea pedido en BD
   ├── Commit transacción
   └── LogAuditory (EXITO)
4. Response 201 Created
```

### Manejo de Errores

- **Validación externa falla** → 422 UnprocessableEntity
- **Productos no existen** → 422 UnprocessableEntity  
- **Error de conexión servicio** → 502 BadGateway
- **Error inesperado** → 500 InternalServerError

---

## Base de Datos

### Tablas

| Tabla | Descripción |
|-------|-------------|
| `OrderHeaders` | Cabecera de pedidos |
| `OrderDetails` | Detalle de items por pedido |
| `Products` | Catálogo de productos |
| `LogAuditory` | Auditoría de operaciones |

### Productos de Prueba

Al ejecutar `db.sql` se insertan 5 productos:

| ID | Nombre | Precio |
|----|--------|--------|
| 1 | Laptop HP Pavilion | $1,500.00 |
| 2 | Mouse Logitech MX Master | $89.99 |
| 3 | Teclado Mecánico Corsair K70 | $179.99 |

---

## Validación Externa

El sistema valida clientes consumiendo: `https://jsonplaceholder.typicode.com/users/1`

- **Respuesta exitosa (200)** → Cliente válido
- **Cualquier error** → Cliente inválido (retorna 422)

---

## Logging

La aplicación registra:
- Inicio de operaciones
- Validaciones (éxito/error)
- Confirmaciones de pedido
- Errores inesperados

Los logs se muestran en consola y se almacenan en `LogAuditory` (BD).

---

## Tech Stack

- **.NET 8** - Framework
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **Swagger/OpenAPI** - Documentación
- **HTTP Client** - Consumo de servicios externos
