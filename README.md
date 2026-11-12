# ZenithSky-API

Backend ASP.NET Core Web API (.NET 10) — Proyecto 2 IF0009 / IF4101.

## Estructura

```
ZenithSky.slnx
├── ZenithSky.Domain      → clases de dominio (Airport, Airline, AirplaneType, Airplane)
├── ZenithSky.Data        → acceso a datos con ADO.NET (SqlConnection / SqlCommand)
├── ZenithSky.Business    → reglas de negocio + Exceptions (NotFound 404, Conflict 409, BusinessRule 400)
├── ZenithSky.API         → Controllers, DTOs, Middleware, Program.cs, Swagger
├── ZenithSky.Test        → pruebas NUnit
└── Database              → ZenithSky.sql
```

## Cómo correrlo

1. Ejecutar `Database/ZenithSky.sql` en SQL Server.
2. Si tu servidor no es `localhost\SQLEXPRESS`, crea `ZenithSky.API/appsettings.Development.json` (no se sube a git):
   ```json
   { "ConnectionStrings": { "ZenithSky": "Data Source=TU_SERVIDOR;Initial Catalog=ZenithSky;Integrated Security=True;Encrypt=False;Trust Server Certificate=True" } }
   ```
3. Proyecto de inicio: `ZenithSky.API` → perfil `http` → Swagger en http://localhost:5080/swagger
4. Desde el emulador Android la API es `http://10.0.2.2:5080/`

## Patrón para cada entidad

`Domain/X.cs` → `Data/XData.cs` → `Business/XBusiness.cs` → `API/DTOs/XRequest.cs` → `API/Controllers/XsController.cs` → registrar Data y Business en `Program.cs`.

Errores: en Business se lanza `NotFoundException`, `ConflictException` o `BusinessRuleException`; el middleware responde el código HTTP con `{ "message": "..." }`.

## Responsables

| Entidad | Persona |
|---|---|
| Airport | Luis |
| Airline | Jefferson |
| Airplane + AirplaneType + base del proyecto | Kevin |

## Ramas

`main` (entregas) ← `develop` ← `feature/airports`, `feature/airlines`, `feature/airplanes`
