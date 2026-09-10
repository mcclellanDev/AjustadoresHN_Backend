# Reglas WAF / ApiProtection — WSRestApiHelp

**API base:** `/outer/wsrestapihelp`  
**Fecha:** 10 de septiembre de 2026  
**Estado:** playbook listo para aplicar en gateway (ops). El rate limit en app ya está activo por nodo IIS.

## Objetivo

Cerrar hallazgos **UNLIMITED** del discovery ApiProtection con límites **globales por IP** (multi-nodo), alineados al `RateLimitHandler` de la API.

| Capa | Alcance | Estado |
|------|---------|--------|
| App (`RateLimitHandler`) | Por worker IIS | Publicado / verificado (429) |
| Gateway / WAF / ApiProtection | Cluster completo | **Aplicar con este playbook** |

## Alcance del discovery (19 endpoints HELP)

Solo estos paths del CSV original (scope re-scan a `/outer/wsrestapihelp`):

| Path | Prioridad WAF |
|------|----------------|
| `/api/login/autenticacion` | P0 Auth |
| `/api/login/getappversion` | P2 Bajo |
| `/api/proveedor/obtenermisatenciones` | P0 PII |
| `/api/proveedor/obtenermisatencionesactivas` | P0 PII |
| `/api/proveedor/obtenerdatosexpedientes` | P0 PII |
| `/api/proveedor/obtenerdatosdeatencion` | P0 PII |
| `/api/proveedor/obtenercachecliente` | P0 PII |
| `/api/proveedor/vectorgpsatencion` | P0 PII |
| `/api/proveedor/listadoajustadores` | P1 |
| `/api/proveedor/obtenerfiniquito` | P1 |
| `/api/proveedor/listafotografiasfirmasatencion` | P1 |
| `/api/proveedor/actualizalogatencion` | P2 |
| `/api/proveedor/actualizarcordenadasajustador` | P2 |
| `/api/proveedor/insertarcoordenadasatencionagenteproveedor` | P2 |
| `/api/proveedor/subirfirmas` | P1 Upload |
| `/api/proveedor/tokenpushonesignal` | P2 |
| `/api/ficohsahn/valida_lista_coberturas` | P1 |
| `/api/seleccionmultiple/tipodefotografia` | P2 Catálogo |
| `/api/seleccionmultiple/tipodelicencia` | P2 Catálogo |

Además (no siempre en el CSV, sí en app): recuperación de password, `BuscarMisAtenciones`, carga BPM, `Cliente/*`.

---

## Set de reglas (aplicar en orden)

Prefijo de match:  
`*/outer/wsrestapihelp` o host `testportal.porsalud.net` / `portal.porsalud.net` + path.

**Clave de cliente:** IP origen real (`X-Forwarded-For` primer hop confiable del LB).  
**Ventana:** 1 minuto (fixed o sliding).  
**Acción:** bloquear / challenge → HTTP **429**, header **`Retry-After: 60`**.  
**OPTIONS:** no contar preflight CORS (o límite alto ≥ 120/min) para no romper apps web.

### RL-AUTH — Abuso de autenticación (P0)

| Campo | Valor |
|-------|--------|
| Nombre | `WSRestApiHelp-Auth-20pm` |
| Match path (OR) | `.../api/login/autenticacion`<br>`.../api/login/recuperarpassword`<br>`.../api/login/validarclave`<br>`.../api/login/actualizarpassword`<br>`.../api/cliente/autenticacion`<br>`.../api/cliente/actualizarpasswordcliente` |
| Métodos | POST (y ALL si el producto no filtra) |
| Límite | **20 req / min / IP** |
| Acción | 429 |

### RL-PII — Listados y datos personales (P0)

| Campo | Valor |
|-------|--------|
| Nombre | `WSRestApiHelp-PII-60pm` |
| Match path (OR / wildcard) | `.../api/proveedor/obtenermisatenciones*`<br>`.../api/proveedor/buscarmisatenciones`<br>`.../api/proveedor/obtenerdatos*`<br>`.../api/proveedor/obtenercachecliente`<br>`.../api/proveedor/obtenerfiniquito`<br>`.../api/proveedor/vectorgpsatencion`<br>`.../api/proveedor/listadoajustadores`<br>`.../api/proveedor/listafotografiasfirmasatencion`<br>`.../api/cliente/obtenermisatenciones*` |
| Métodos | GET, POST |
| Límite | **60 req / min / IP** |
| Acción | 429 |

### RL-FICO — Consultas aseguradora (P1)

| Campo | Valor |
|-------|--------|
| Nombre | `WSRestApiHelp-Ficohsa-60pm` |
| Match | `.../api/ficohsahn/*`<br>`.../api/ficohsagt/*` |
| Excluir de este bucket | paths de **RL-HEAVY** (carga/upload) |
| Límite | **60 req / min / IP** |
| Acción | 429 |

### RL-HEAVY — Cargas y uploads (P1)

| Campo | Valor |
|-------|--------|
| Nombre | `WSRestApiHelp-Heavy-15pm` |
| Match | `.../api/ficohsahn/carga_reclamo_sinau_bpm_fico*`<br>`.../api/ficohsahn/subir_archivos_wsfh`<br>`.../api/proveedor/subirfirmas`<br>`.../api/proveedor/subirfotos*`<br>`.../api/proveedor/subirvideo*`<br>`.../api/login/subirvideo`<br>`.../api/login/enviarnotificacionaccidente` |
| Límite | **15 req / min / IP** |
| Acción | 429 |
| Nota | Body grande: preferir límite por request count, no solo bandwidth |

### RL-DEFAULT — Resto HELP (P2)

| Campo | Valor |
|-------|--------|
| Nombre | `WSRestApiHelp-Default-120pm` |
| Match | `.../outer/wsrestapihelp/api/*` |
| Excluir | ya cubiertos por RL-AUTH / RL-PII / RL-FICO / RL-HEAVY |
| Límite | **120 req / min / IP** |
| Acción | 429 |
| Incluye | `getappversion`, `seleccionmultiple/*`, coordenadas, OneSignal, etc. |

### Prioridad de evaluación

1. RL-AUTH  
2. RL-HEAVY  
3. RL-PII  
4. RL-FICO  
5. RL-DEFAULT  

(Primera regla que matchee gana; evita doble conteo si el producto lo permite.)

---

## Equivalencia con la app

| Regla WAF | App setting | App default |
|-----------|-------------|-------------|
| RL-AUTH | `RateLimit_Login_PerMinute` | 20 |
| RL-PII / RL-FICO | `RateLimit_SensitiveGet_PerMinute` | 60 |
| RL-HEAVY | `RateLimit_HeavyWrite_PerMinute` | 15 |

WAF puede ir **ligeramente por encima** de la app (ej. auth 25) para que la evidencia de 429 en auditoría salga primero del gateway sin falsear la app; o **igual** para comportamiento uniforme.

---

## Si el gateway es IIS ARR / Dynamic IP Restrictions

Cuando no haya consola ApiProtection de rate-limit por path:

1. **ARR** → Server Farms / URL Rewrite: reglas por patrón de path que respondan 429 (custom module) **o**
2. **Dynamic IP Restrictions** (IIS): Deny after **N** concurrent / **M** requests in X ms — más grueso; usar como red de seguridad:
   - Maximum concurrent: 20–40  
   - Request limit: ~100 / 200 ms window (ajustar en staging)  
3. Preferible: módulo/WAF delante (Imperva / Cloudflare / Azure Front Door / APIM) con las 5 reglas de arriba.

---

## Verificación (evidencia)

Tras activar reglas en **testportal**:

```text
1) 21x POST /api/Login/Autenticacion (user inválido)
   → ≥1 respuesta 429 (WAF o app)

2) Con JWT: 61x GET ObtenerMisAtenciones?IdProveedorAgente={propio}
   → 429

3) OPTIONS preflight Origin=https://testportal.porsalud.net
   → 200 + ACAO (no bloqueado por rate limit)

4) Tráfico legítimo app (login + 5 listados) → 200
```

Adjuntar a auditoría: captura/regla ID + CSV re-scan sin `UNLIMITED` (o con límites ACTIVE).

---

## Checklist ops

- [ ] Scope ApiProtection = solo `/outer/wsrestapihelp`
- [ ] Creada `WSRestApiHelp-Auth-20pm`
- [ ] Creada `WSRestApiHelp-PII-60pm`
- [ ] Creada `WSRestApiHelp-Ficohsa-60pm`
- [ ] Creada `WSRestApiHelp-Heavy-15pm`
- [ ] Creada `WSRestApiHelp-Default-120pm`
- [ ] IP key = cliente real (no IP del LB interno)
- [ ] OPTIONS excluido o límite alto
- [ ] Probado 429 en testportal
- [ ] Mismas reglas en producción (portal) tras OK en test
- [ ] Re-escaneo ApiProtection + CSV guardado

**Contacto app:** límites también en `WSRestApiHelp/Web.config` (`RateLimit_*`).
