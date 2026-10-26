# Prueba push desde la API

Publicar la solucion compilada y conservar en el servidor los valores reales de
`OneSignalAppId` y `OneSignalRestApiKey`. El marcador local no es una clave valida.
No guardar secretos en Git ni sustituirlos por la Key ID de OneSignal.

1. Obtener un JWT vigente mediante `POST /api/Login/Autenticacion` con la cuenta
   cuyo dispositivo recibira la prueba (para Edwyn: agente 3974).
2. En Swagger buscar el controlador `PushDiagnostico` y ejecutar:

   ```http
   POST /api/Proveedor/ProbarNotificacionPush
   Authorization: Bearer <JWT vigente>
   ```

   No enviar cuerpo, IdProveedorAgente, TokenPush ni clave OneSignal.
3. La API resuelve la identidad con la misma funcion de base de datos que el
   login y utiliza el TokenPush almacenado del agente. Si la identidad es ambigua
   rechaza la peticion. Solo permite probar el dispositivo propio.
4. HTTP 200 con `AceptadaPorOneSignal: true` y `NotificacionId` significa que
   OneSignal creo el mensaje. `EntregaConfirmada: false` significa que esta API
   no ha verificado la entrega; confirmar la notificacion en el telefono.

Respuestas de diagnostico:

- 401: falta autenticacion valida.
- 403: la identidad no corresponde univocamente a un agente.
- 409: el agente no tiene TokenPush registrado.
- 429: esperar un minuto; limite en memoria por usuario y proceso IIS.
- 503: configuracion OneSignal ausente, App ID invalido o clave de ejemplo.
- 502: OneSignal rechazo la solicitud o no devolvio un ID valido de notificacion.
  `OneSignalHttpStatus` informa el estado del proveedor. No se expone su respuesta cruda.
- 500: error interno; consultar el Trace del servidor con `DiagnosticoId`.

La prueba no crea atenciones ni audiencias y no inserta registros en
`help_NotificacionesPush` (esa tabla corresponde a eventos de atenciones).
El diagnostico se devuelve en la respuesta y se escribe mediante Trace;
su persistencia depende de los listeners configurados en el servidor.
No reintenta los envios historicos fallidos.

Validacion local sin SQL ni envios reales:

```powershell
powershell.exe -NoProfile -File Tests/PushDiagnostico.Smoke.ps1
```
