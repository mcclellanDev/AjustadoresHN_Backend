# Evidencia de remediación — ApiProtection / WSRestApiHelp

**Fecha:** 10 de septiembre de 2026  
**API:** `/outer/wsrestapihelp`  
**Objetivo:** cerrar hallazgos críticos de acceso y poner rate limit en endpoints sensibles del CSV.

## Cambios aplicados (código)

| # | Hallazgo | Remediación | Archivo |
|---|----------|-------------|---------|
| 1 | `[Authorize]` comentado en Proveedor | Reactivado `[Authorize]` a nivel de controlador. `DescargarAudienciaIcs` permanece `[AllowAnonymous]`. | `Controllers/ProveedorController.cs` |
| 2 | GETs/login UNLIMITED en discovery | `RateLimitHandler`: login 20/min/IP; GETs sensibles 60/min/IP. Responde HTTP 429. | `Clases/RateLimitHandler.cs` |
| 3 | Registro del handler | Rate limit → JWT (orden de MessageHandlers). | `App_Start/WebApiConfig.cs` |
| 4 | Configuración | `RateLimit_Enabled`, `RateLimit_Login_PerMinute`, `RateLimit_SensitiveGet_PerMinute` | `Web.config` |
| 5 | JWT / OPTIONS | Preflight OPTIONS sin JWT; evita doble `ValidateToken`; 401/500 con respuesta HTTP correcta. | `Clases/TokenValidator.cs` |
| 6 | `ClienteController` anónimo completo | `[Authorize]` a nivel controller; solo `Autenticacion` queda `[AllowAnonymous]`. | `Controllers/ClienteController.cs` |
| 7 | CORS `origins: "*"` | Orígenes desde `Cors_AllowedOrigins` (global); se quitó `EnableCors("*")` de los controllers. | `WebApiConfig.cs`, `Web.config`, controllers |
| 8 | Rate limit Cliente | Login cliente + GETs de atenciones cliente en el mismo handler. | `Clases/RateLimitHandler.cs` |
| 9 | Rate limit ampliado | Auth abuse (login/recuperación), POST `BuscarMisAtenciones`, consultas Ficohsa, heavy-write BPM/uploads. | `Clases/RateLimitHandler.cs`, `Web.config` |
| 10 | Playbook WAF | Reglas RL-AUTH/PII/FICO/HEAVY/DEFAULT + checklist ops. | `Database/WAF_ApiProtection_Reglas_WSRestApiHelp.md` |
| 11 | `LoginController` anónimo | `[Authorize]` + `[AllowAnonymous]` solo en login/versión/recuperación password. `EnviarNotificacionAccidente` y `SubirVideo` exigen JWT. | `Controllers/LoginController.cs` |
| 12 | Password en response Cliente | Se dejó de devolver `Contrasena` en login Cliente. | `Controllers/ClienteController.cs` |

## Paths con rate limit

**Auth (20/min/IP)** — `RateLimit_Login_PerMinute`  
`/api/login/autenticacion`, `recuperarpassword`, `validarclave`, `actualizarpassword`  
`/api/cliente/autenticacion`, `actualizarpasswordcliente`

**Sensibles (60/min/IP)** — `RateLimit_SensitiveGet_PerMinute` (cualquier método)  
Proveedor listados/expediente/GPS/buscar; Cliente atenciones; FicohsaHN/GT consultas y validación BPM.

**Heavy write (15/min/IP)** — `RateLimit_HeavyWrite_PerMinute`  
Carga reclamo BPM PROD/QA, subir archivos Ficohsa, notificación accidente y subir video (Login anónimo).

## Gateway / WAF

Playbook operativo listo: **`Database/WAF_ApiProtection_Reglas_WSRestApiHelp.md`**

Incluye 5 reglas (`RL-AUTH`, `RL-PII`, `RL-FICO`, `RL-HEAVY`, `RL-DEFAULT`), los 19 paths del CSV, checklist ops y verificación 429.  
**Pendiente:** aplicar en consola ApiProtection / WAF (test → prod).

Scope discovery ApiProtection a `/outer/wsrestapihelp` y re-escanear tras publicar reglas.

## Cómo verificar (evidencia para auditoría)

1. **Sin token, GET crítico**  
   `GET /api/Proveedor/ObtenerMisAtenciones?IdProveedorAgente=1`  
   Esperado: **401 Unauthorized** (antes podía responder 200).

2. **Con JWT válido**  
   Mismo GET con `Authorization: Bearer {token}`  
   Esperado: **200** (comportamiento funcional).

3. **Rate limit login**  
   Más de 20 `POST /api/Login/Autenticacion` desde la misma IP en 1 minuto  
   Esperado: **429 Too Many Requests** + header `Retry-After: 60`.

4. **ICS anónimo**  
   `GET /api/Proveedor/DescargarAudienciaIcs?...`  
   Esperado: sigue accesible sin JWT (AllowAnonymous).

5. **Cliente sin token**  
   `GET /api/Cliente/ObtenerMisAtencionesActivas?...`  
   Esperado: **401** (antes podía responder sin JWT).

6. **CORS**  
   Desde un origen no listado en `Cors_AllowedOrigins`, el navegador no debe recibir `Access-Control-Allow-Origin: *`.

7. **Re-escaneo ApiProtection**  
   Tras publicar, limitar discovery a `/outer/wsrestapihelp` y adjuntar nuevo CSV como evidencia de cierre (idealmente con reglas activas, no solo PASSIVE).

## Pendiente (siguiente iteración)

- ~~Revisar `ClienteController` (`[AllowAnonymous]` a nivel controller completo).~~ **Hecho.**
- ~~Restringir CORS `origins: "*"` a orígenes conocidos.~~ **Hecho** (`Cors_AllowedOrigins`).
- ~~Ampliar rate limit app (auth + POST buscar + Ficohsa + heavy write).~~ **Hecho** (publicado + 429 verificado).
- ~~Playbook reglas WAF.~~ **Hecho** — falta **aplicar** en gateway (`WAF_ApiProtection_Reglas_WSRestApiHelp.md`).
- ~~`LoginController` Authorize selectivo.~~ **Hecho en código** — publicar.
- ~~No devolver `Contrasena` en login Cliente.~~ **Hecho en código** — publicar; validar app Cliente.
- Re-escaneo ApiProtection + CSV evidencia.
- IDOR Cliente (token ↔ dueño) si el modelo de claims lo permite.

## Autorización por recurso (detalle)

- Login emite claim `ProveedorAgenteId`.
- Endpoints con `IdProveedorAgente`: solo permiten el id del token (403 si no coincide).
- Endpoints con `IdAtencion`: verifican `help_Atenciones.RefProveedorId` = agente autenticado.
- Tokens antiguos sin claim: fallback por `Usuario` en `help_ProveedorAgentes` (hasta re-login).

## Nota operativa

En servidores con **varios nodos**, el contador en memoria es por instancia. Para límite global usar ApiProtection / ARR / API Management además de este handler.
