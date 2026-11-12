# Destilatech Platform

RESTful API de **Destilatech** (startup **FuturosSeniors**), plataforma SaaS B2B para el ecosistema peruano del pisco:
producción y monitoreo IoT, inventario, pedidos y reposición, alertas, estimaciones e indicadores, suscripciones y usuarios.

Curso: Aplicaciones Web (1ASI0730) — UPC, ciclo 2026-20.

- Reporte: [destilatech-report](https://github.com/upc-pre-202620-1ASI0730-2620-8084-desti/destilatech-report)
- Landing Page: [destilatech-website](https://github.com/upc-pre-202620-1ASI0730-2620-8084-desti/destilatech-website)
- Web Application: [destilatech-webapp](https://github.com/upc-pre-202620-1ASI0730-2620-8084-desti/destilatech-webapp)

## Tecnologías

| Herramienta | Uso |
| :--- | :--- |
| .NET 10 · ASP.NET Core Web API | API REST con controllers |
| Entity Framework Core 10 + MySQL (`MySql.EntityFrameworkCore`) | Persistencia, migraciones y convención snake_case |
| Cortex.Mediator | Domain events y pipeline de commands (logging, excepciones, transacción) |
| Microsoft.AspNetCore.OpenApi + Scalar | Documentación interactiva de la API |
| Humanizer | Rutas en kebab-case y nombres en snake_case |
| Stripe.net | Pasarela de pagos (modo prueba) del bounded context Billing |

IDE recomendado: **JetBrains Rider**.

## Bounded contexts

| Bounded context | Carpeta | Endpoints principales |
| :--- | :--- | :--- |
| Shared Kernel | `Shared/` | — (`AggregateRoot`, auditoría, base repository, unit of work, `Result<T>`, errores, interceptores, mediator) |
| IAM | `Iam/` | `/api/v1/users` |
| Billing | `Billing/` | `/api/v1/plans`, `/checkout-sessions`, `/subscriptions/{id}`, `/tenants/{tenantId}/subscriptions`, `/tenants/{tenantId}/payments` |
| Alerts & Notifications | `AlertsNotifications/` | `/api/v1/alerts`, `/tenants/{tenantId}/alerts` |
| Inventory & Stock | `InventoryStock/` | `/api/v1/products`, `/stock-items`, `/stock-movements` y sus listas en `/tenants/{tenantId}/...` |
| Production & Monitoring | `ProductionMonitoring/` | `/api/v1/batches`, `/process-variables`, `/sensor-readings` y sus listas en `/tenants/{tenantId}/...` |
| Orders & Replenishment | `OrdersReplenishment/` | `/api/v1/customers`, `/orders`, `/replenishment-orders` y sus listas en `/tenants/{tenantId}/...` |
| Analytics & Estimations | `AnalyticsEstimations/` | `/api/v1/tenants/{tenantId}/dashboard-summary`, `/historical-indicators`, `/replenishment-estimates` |

Cada bounded context sigue las capas de Domain-Driven Design: `Domain` (aggregates, value objects, commands, queries,
repositorios), `Application` (command y query services, ACL), `Infrastructure` (EF Core) e `Interfaces` (REST y ACL).

### Rutas por tenant

Cada cuenta (productor o comercializador) es un **tenant** y es dueña de sus datos. Igual que
`/categories/{categoryId}/tutorials` en el Learning Center, los datos de un tenant se leen como sub-recursos suyos,
sin filtros como `?userId=`:

| Operación | Ruta | Dónde va el tenant |
| :--- | :--- | :--- |
| Listar o leer los datos de un tenant | `GET /api/v1/tenants/{tenantId}/products`, `/tenants/{tenantId}/sensor-readings`, `/tenants/{tenantId}/customers/{customerId}/orders`... (controllers `Tenant...Controller`) | En la ruta |
| Crear o modificar | `POST /api/v1/products`, `PUT /products/{id}`, `PATCH /batches/{id}/status`... | En el cuerpo: `"tenantId": 1` |

Los filtros propios de cada consulta siguen como query (`?days=30`, `?type=LOW_STOCK`, `?limit=100`).

### Auditoría y eventos de dominio

Todos los agregados heredan de `AggregateRoot` (eventos de dominio) e implementan `IAuditableEntity` en un archivo
parcial `XxxAudit.cs` con `CreatedAt` y `UpdatedAt`. Dos interceptores de EF Core (`Shared/Infrastructure/Persistence/
EntityFrameworkCore/Interceptors`) se encargan de ellos al guardar: `AuditableEntityInterceptor` llena las fechas y
`DispatchDomainEventsInterceptor` publica los eventos registrados.

## Opción rápida: Docker (MySQL + API con un comando)

Requisito: Docker Desktop abierto.

```bash
docker compose up -d --build
```

- API y documentación: `http://localhost:8080/scalar`
- MySQL: `localhost`, puerto **3307**, usuario `root`, contraseña `destilatech` y base de datos `destilatech`. Se puede abrir
  con MySQL Workbench creando una conexión con esos datos. Se usa el puerto 3307 para no chocar con un MySQL instalado en la PC.

```bash
docker compose logs -f api    # ver el log de la API
docker compose down           # detener todo (los datos se conservan)
docker compose down -v        # detener y borrar la base de datos
```

## Puesta en marcha (sin Docker)

Requisitos: .NET 10 SDK y MySQL 8 (por ejemplo, MySQL Server + MySQL Workbench).

1. Configura tu conexión a MySQL **sin subir tu contraseña** (user secrets):

   ```bash
   cd DestilatechPlatform.API
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;user=root;password=TU_CONTRASEÑA;database=destilatech"
   ```

2. Ejecuta la API (en Rider, perfil `http`, o con):

   ```bash
   dotnet run --project DestilatechPlatform.API
   ```

   Al iniciar se aplican las migraciones (`Database.Migrate()`): se crea el schema `destilatech` con sus tablas y se
   cargan los planes y usuarios de prueba.

3. Abre la documentación: `http://localhost:5273/scalar`.

Para usar Stripe en modo prueba, guarda tu clave secreta también con user secrets:

```bash
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
```

## Migraciones

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add NombreDeLaMigracion --project DestilatechPlatform.API
dotnet ef migrations has-pending-model-changes --project DestilatechPlatform.API
```

Antes de unir una rama con una migración nueva, actualízala con `develop` y vuelve a generar su migración, para que
`AppDbContextModelSnapshot.cs` no quede en conflicto.

## Despliegue

La imagen se construye con el `Dockerfile` de la raíz:

```bash
docker build -t destilatech-platform .
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production -e DATABASE_URL=... -e DATABASE_USER=... -e DATABASE_PASSWORD=... -e DATABASE_SCHEMA=destilatech destilatech-platform
```

En producción la cadena de conexión se arma con las variables de entorno `DATABASE_URL`, `DATABASE_USER`,
`DATABASE_PASSWORD` y `DATABASE_SCHEMA` (`appsettings.Production.json`).

## Conexión con la Web Application

La API permite peticiones de cualquier origen (política CORS `AllowAllPolicy`). En la web app basta con apuntar
`VITE_DESTILATECH_API_URL` a `http://localhost:5273/api/v1` (desarrollo) o a la URL del backend desplegado (producción).

## Convenciones

- Código, commits y nombres en inglés; mensajes al usuario localizados en `Resources/SharedResource.*.resx` (en, es).
- Git Flow: `main`, `develop`, `feature/*`, `release/*`. Commits con Conventional Commits.
