# Distribuidora Don Pepe API

Sistema de gestión para distribuidora desarrollado con .NET 9 y Entity Framework Core con SQLite.

## Descripción

Esta API REST proporciona funcionalidades completas para la gestión de una distribuidora, incluyendo:

- **Gestión de Productos**: Control de inventario, precios, categorías y stock
- **Gestión de Clientes**: Información completa de clientes y historial
- **Gestión de Pedidos**: Creación, seguimiento y procesamiento de órdenes
- **Base de Datos**: SQLite integrada con datos de ejemplo

## Estructura del Proyecto

```
DistribuidoraDonPepe.API/
├── Controllers/           # Controladores de la API
│   ├── ProductsController.cs
│   ├── CustomersController.cs
│   └── OrdersController.cs
├── Models/               # Modelos de dominio
│   ├── Product.cs
│   ├── Customer.cs
│   ├── Order.cs
│   └── OrderItem.cs
├── Data/                 # Contexto de base de datos
│   └── DistribuidoraDonPepeContext.cs
└── Program.cs           # Configuración de la aplicación
```

## Requisitos

- .NET 9.0 SDK
- SQLite (incluido automáticamente)

## Instalación y Configuración

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/Jackyjhs/proyecto-distribuidora-don-pepe.git
   cd proyecto-distribuidora-don-pepe
   ```

2. **Restaurar dependencias**:
   ```bash
   dotnet restore
   ```

3. **Compilar el proyecto**:
   ```bash
   dotnet build
   ```

4. **Ejecutar la aplicación**:
   ```bash
   cd DistribuidoraDonPepe.API
   dotnet run
   ```

La API estará disponible en: `http://localhost:5143`

## Documentación de la API

### Swagger UI
Accede a la documentación interactiva en: `http://localhost:5143/swagger`

### Endpoints Principales

#### Productos
- `GET /api/products` - Obtener todos los productos activos
- `GET /api/products/{id}` - Obtener un producto específico
- `POST /api/products` - Crear nuevo producto
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/products/{id}` - Eliminar producto (soft delete)
- `GET /api/products/category/{category}` - Productos por categoría
- `GET /api/products/low-stock` - Productos con stock bajo

#### Clientes
- `GET /api/customers` - Obtener todos los clientes activos
- `GET /api/customers/{id}` - Obtener un cliente específico
- `POST /api/customers` - Crear nuevo cliente
- `PUT /api/customers/{id}` - Actualizar cliente
- `DELETE /api/customers/{id}` - Eliminar cliente

#### Pedidos
- `GET /api/orders` - Obtener todos los pedidos
- `GET /api/orders/{id}` - Obtener un pedido específico
- `POST /api/orders` - Crear nuevo pedido
- `PUT /api/orders/{id}` - Actualizar pedido
- `DELETE /api/orders/{id}` - Eliminar pedido (solo pendientes)
- `PATCH /api/orders/{id}/status` - Actualizar estado del pedido
- `GET /api/orders/customer/{customerId}` - Pedidos de un cliente

## Ejemplos de Uso

### Crear un nuevo producto
```bash
curl -X POST "http://localhost:5143/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Azúcar Refinada",
    "description": "Azúcar refinada de alta calidad",
    "price": 1.50,
    "stock": 200,
    "category": "Endulzantes",
    "brand": "Don Pepe"
  }'
```

### Crear un nuevo cliente
```bash
curl -X POST "http://localhost:5143/api/customers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ana López",
    "email": "ana.lopez@email.com",
    "phone": "555-0103",
    "address": "Calle Nueva 789",
    "city": "Alajuela",
    "state": "Alajuela",
    "postalCode": "20101"
  }'
```

### Crear un pedido
```bash
curl -X POST "http://localhost:5143/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 1,
    "shippingAddress": "Calle Principal 123, San José",
    "notes": "Entrega en horario de oficina",
    "orderItems": [
      {
        "productId": 1,
        "quantity": 5
      },
      {
        "productId": 2,
        "quantity": 3
      }
    ]
  }'
```

## Características Técnicas

### Modelos de Datos

**Product** (Producto):
- Control de inventario automático
- Categorización y marca
- Soft delete para mantener historial

**Customer** (Cliente):
- Validación de email único
- Información completa de contacto
- Historial de pedidos

**Order** (Pedido):
- Estados: Pendiente, Confirmado, Enviado, Entregado, Cancelado
- Cálculo automático de totales e impuestos (13%)
- Control de stock automático

**OrderItem** (Item de Pedido):
- Relación con productos y órdenes
- Precios históricos mantenidos

### Base de Datos

- **Motor**: SQLite (archivo local)
- **Migraciones**: Automáticas al iniciar la aplicación
- **Datos de prueba**: Se cargan automáticamente
- **Integridad**: Restricciones de clave foránea configuradas

### Funcionalidades

- **Validación**: Modelos con anotaciones de validación
- **CORS**: Configurado para desarrollo
- **Documentación**: Swagger/OpenAPI integrado
- **Logging**: Entity Framework con queries detalladas
- **Manejo de errores**: Respuestas HTTP apropiadas
- **Lógica de negocio**: Control de stock, cálculo de totales, validaciones

## Datos de Ejemplo

La aplicación incluye datos de prueba:

### Productos
- Arroz Premium (Granos) - $2.50 - Stock: 100
- Frijoles Negros (Granos) - $1.80 - Stock: 150  
- Aceite de Cocina (Aceites) - $3.20 - Stock: 75

### Clientes
- María García - maria.garcia@email.com - San José
- Juan Pérez - juan.perez@email.com - Cartago

## Tecnologías Utilizadas

- **.NET 9.0**: Framework principal
- **ASP.NET Core**: API Web
- **Entity Framework Core 9.0**: ORM
- **SQLite**: Base de datos
- **Swashbuckle**: Documentación API (Swagger)
- **System.Text.Json**: Serialización JSON

## Estructura de Respuesta de la API

Todas las respuestas incluyen códigos de estado HTTP apropiados:
- `200 OK`: Operación exitosa
- `201 Created`: Recurso creado exitosamente
- `400 Bad Request`: Datos de entrada inválidos
- `404 Not Found`: Recurso no encontrado
- `409 Conflict`: Conflicto (ej. email duplicado)

## Contribución

1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crea un Pull Request

## Licencia

Este proyecto es de código abierto y está disponible bajo la [Licencia MIT](LICENSE).

## Soporte

Para reportar problemas o solicitar nuevas funcionalidades, crea un [issue](https://github.com/Jackyjhs/proyecto-distribuidora-don-pepe/issues) en el repositorio.
