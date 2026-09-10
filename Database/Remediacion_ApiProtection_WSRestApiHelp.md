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

## Paths con rate limit

**Login:** `/api/login/autenticacion`  
**GETs sensibles:**  
`obtenermisatenciones`, `obtenermisatencionesactivas`, `obtenerdatosexpedientes`, `obtenerdatosdeatencion`, `obtenercachecliente`, `vectorgpsatencion`, `listadoajustadores`, `buscarmisatenciones`

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

5. **Re-escaneo ApiProtection**  
   Tras publicar, limitar discovery a `/outer/wsrestapihelp` y adjuntar nuevo CSV como evidencia de cierre (idealmente con reglas activas, no solo PASSIVE).

## Pendiente (siguiente iteración)

- Autorización por recurso (el JWT debe corresponder al agente dueño de la atención) — mitiga IDOR aunque haya token robado/compartido.
- Revisar `ClienteController` (`[AllowAnonymous]` a nivel controller completo).
- Restringir CORS `origins: "*"` a orígenes conocidos de app/portal.
- Rate limit también en gateway/WAF (defensa en profundidad; el handler en memoria es por nodo IIS).

## Nota operativa

En servidores con **varios nodos**, el contador en memoria es por instancia. Para límite global usar ApiProtection / ARR / API Management además de este handler.
