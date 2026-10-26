/*
  Reportería analítica: embudo App HELP (InterAuto / Ficohsa)
  Base: atenciones de ajuste + indicios de inicio / proceso / finalización en app.

  Indicios (según esquema real):
    1) Inicio app     : help_LogAtencionesNuevas.status = 2
                        (en listados API aparece como LogEstado; columna física = status)
                        PK/join: idAtencion ; fecha: fechaRegistro
    2) Proceso app    : existe help_CacheCliente.IdAtencion
                        (sin fecha propia; referencia = help_Atenciones.FechaInicio
                         vía IdAtencion = Id; también disponibles FechaFin / FechaCierre)
    3) Fin app exitoso: Atencion EstadoId = 7 (CERRADO)
                        + BPM CodigoRespuestaBpm = 0   -- INT
                        + BPM Mensaje = 'PROCESO EXITOSO'
                        + NumeroReclamo / SolicitudBpm / RefAjustadorAudienciaId / FechaRegistro no nulos

  status en LogAtencionesNuevas (negocio):
    1 = atención nueva (aún no tomada en app)  -- muestra tu TOP 5 actual
    2 = tomada / inicio flujo app
*/

;WITH Base AS (
    SELECT
         A.Id                              AS AtencionId
        ,A.FechaInicio                     AS FechaAtencion
        ,A.FechaFin                        AS FechaFinAtencion
        ,A.FechaCierre                     AS FechaCierreAtencion
        ,A.RefPlanId                       AS PlanId
        ,A.[Plan]                          AS PlanNombre
        ,A.RefEstadoId                     AS AtencionEstadoId
        ,EA.EstadoAtencion                 AS AtencionEstado
        ,A.PolizaExterna
        ,ATS.Id                            AS AtencionServicioId
        ,ATS.FechaRegistro                 AS FechaServicio
        ,ATS.RefIdEstado                   AS AtencionServicioEstadoId
        ,EAS.Estado                        AS AtencionServicioEstado
        ,ATS.RefProveedorId                AS ProveedorId
        ,P.NombreProveedor                 AS ProveedorNombre
        ,ATS.RefProveedorAgenteId          AS AgenteId
        ,PA.NombreAgente                   AS AgenteNombre
        ,ATS.RefTipoServicioId             AS ServicioId
        ,S.TipoAtencion                    AS ServicioNombre
        ,AA.Id                             AS AjustadorAudienciaId
    FROM help.dbo.help_Atenciones_Servicios ATS
    LEFT JOIN help.dbo.help_Atenciones A
        ON ATS.RefAtencionId = A.Id
    LEFT JOIN help.dbo.help_RefEstadoAtencion EA
        ON A.RefEstadoId = EA.Id
    LEFT JOIN help.dbo.help_RefEstadoAtencionServicio EAS
        ON ATS.RefIdEstado = EAS.Id
    LEFT JOIN help.dbo.help_ProveedorAgentes PA
        ON ATS.RefProveedorAgenteId = PA.Id
    LEFT JOIN help.dbo.help_Proveedor P
        ON ATS.RefProveedorId = P.Id
    LEFT JOIN help.dbo.help_Servicios S
        ON ATS.RefTipoServicioId = S.Id
    OUTER APPLY (
        SELECT TOP (1) AA0.Id
        FROM help.dbo.help_AjustadorAudiencia AA0
        WHERE AA0.RefAtencionId = ATS.RefAtencionId
        ORDER BY AA0.Id DESC
    ) AA
    WHERE ATS.RefTipoServicioId = 32              -- Ajuste
      AND ATS.FechaRegistro >= '2026-08-15'
      AND ATS.RefProveedorId = 444                -- Ficohsa
      AND A.RefEstadoId IN (6, 7)                 -- Asignada / Cerrada
      AND ATS.RefProveedorAgenteId NOT IN (3974, 2535)
      AND ATS.RefIdEstado NOT IN (12)             -- Eliminado por autorización plan
      AND A.RefPlanId = 753                       -- InterAuto
      AND A.PolizaExterna NOT LIKE '%1092249%'
),
Indicios AS (
    SELECT
         B.*

        -- 1) Inicio en app (status = 2)
        ,CASE WHEN LN.idAtencion IS NOT NULL THEN 1 ELSE 0 END AS Flag_InicioApp
        ,LN.[status]                           AS LogStatusNuevas   -- equivalente a LogEstado del API
        ,LN.fechaRegistro                      AS FechaInicioApp

        -- 2) Proceso (formulario cliente)
        --    help_CacheCliente no tiene fecha propia: se usa FechaInicio de help_Atenciones
        --    como fecha de referencia de la atención (no es timestamp exacto del guardado del cache).
        ,CASE WHEN CC.IdAtencion IS NOT NULL THEN 1 ELSE 0 END AS Flag_ProcesoApp_CacheCliente
        ,CASE WHEN CC.IdAtencion IS NOT NULL THEN B.FechaAtencion ELSE NULL END AS FechaCache_RefAtencion
        ,CC.Nombre                             AS CacheCliente_Nombre
        ,CC.Poliza                             AS CacheCliente_Poliza
        ,CC.ChasisVehiculo                     AS CacheCliente_Chasis

        -- Refuerzo: informe ajustador
        ,CASE WHEN B.AjustadorAudienciaId IS NOT NULL THEN 1 ELSE 0 END AS Flag_TieneInformeAjustador

        -- BPM éxito / errores
        ,BPM.Id                                AS LogIdExito
        ,BPM.NumeroReclamo
        ,BPM.SolicitudBpm
        ,BPM.RefAjustadorAudienciaId           AS IdAtencionAudiencia
        ,BPM.FechaRegistro                     AS FechaBpmExitoso
        ,BPM.Mensaje                           AS MensajeBpmExitoso
        ,BPM.CodigoRespuestaBpm                AS CodigoRespuestaBpmExitoso
        ,BPM.Ambiente                          AS AmbienteBpmExitoso

        ,ERR.ConteoErroresBpm
        ,ERR.FechaUltimoError
        ,ERR.UltimoMensajeError
        ,ERR.UltimoCodigoError

        ,CASE
            WHEN B.AtencionEstadoId = 7
             AND BPM.Id IS NOT NULL
             AND BPM.CodigoRespuestaBpm = 0
             AND BPM.Mensaje = 'PROCESO EXITOSO'
             AND BPM.NumeroReclamo IS NOT NULL
             AND BPM.SolicitudBpm IS NOT NULL
             AND BPM.RefAjustadorAudienciaId IS NOT NULL
             AND BPM.FechaRegistro IS NOT NULL
            THEN 1 ELSE 0
         END AS Flag_FinalizadaApp_Exitosa

        -- SLA parcial: solo inicio app -> BPM (cache no tiene fecha)
        ,CASE
            WHEN LN.fechaRegistro IS NOT NULL AND BPM.FechaRegistro IS NOT NULL
            THEN DATEDIFF(MINUTE, LN.fechaRegistro, BPM.FechaRegistro)
            ELSE NULL
         END AS Minutos_InicioApp_a_BpmExitoso
    FROM Base B
    OUTER APPLY (
        SELECT TOP (1)
             L.idAtencion
            ,L.[status]
            ,L.fechaRegistro
        FROM help.dbo.help_LogAtencionesNuevas L
        WHERE L.idAtencion = B.AtencionId
          AND L.[status] = 2
        ORDER BY L.fechaRegistro DESC
    ) LN
    OUTER APPLY (
        SELECT TOP (1)
             C.IdAtencion
            ,C.Nombre
            ,C.Poliza
            ,C.ChasisVehiculo
        FROM help.dbo.help_CacheCliente C
        WHERE C.IdAtencion = B.AtencionId
    ) CC
    OUTER APPLY (
        SELECT TOP (1)
             L.Id
            ,L.NumeroReclamo
            ,L.SolicitudBpm
            ,L.RefAjustadorAudienciaId
            ,L.FechaRegistro
            ,L.Mensaje
            ,L.CodigoRespuestaBpm
            ,L.Ambiente
        FROM help.dbo.help_Log_CargaReclamoBPM L
        WHERE L.RefAjustadorAudienciaId = B.AjustadorAudienciaId
          AND L.Mensaje = 'PROCESO EXITOSO'
          AND L.CodigoRespuestaBpm = 0
        ORDER BY L.FechaRegistro DESC
    ) BPM
    OUTER APPLY (
        SELECT
             COUNT(*) AS ConteoErroresBpm
            ,MAX(L.FechaRegistro) AS FechaUltimoError
            ,(
                SELECT TOP (1) L2.Mensaje
                FROM help.dbo.help_Log_CargaReclamoBPM L2
                WHERE L2.RefAjustadorAudienciaId = B.AjustadorAudienciaId
                  AND NOT (
                        L2.Mensaje = 'PROCESO EXITOSO'
                    AND L2.CodigoRespuestaBpm = 0
                  )
                ORDER BY L2.FechaRegistro DESC
             ) AS UltimoMensajeError
            ,(
                SELECT TOP (1) L2.CodigoRespuestaBpm
                FROM help.dbo.help_Log_CargaReclamoBPM L2
                WHERE L2.RefAjustadorAudienciaId = B.AjustadorAudienciaId
                  AND NOT (
                        L2.Mensaje = 'PROCESO EXITOSO'
                    AND L2.CodigoRespuestaBpm = 0
                  )
                ORDER BY L2.FechaRegistro DESC
             ) AS UltimoCodigoError
        FROM help.dbo.help_Log_CargaReclamoBPM L
        WHERE L.RefAjustadorAudienciaId = B.AjustadorAudienciaId
          AND NOT (
                L.Mensaje = 'PROCESO EXITOSO'
            AND L.CodigoRespuestaBpm = 0
          )
    ) ERR
)
,
Detalle AS (
    SELECT
         I.*
        ,CASE
            WHEN I.Flag_FinalizadaApp_Exitosa = 1
                THEN 'APP_FINALIZADA_EXITOSA'
            WHEN I.Flag_InicioApp = 1
             AND I.Flag_ProcesoApp_CacheCliente = 0
                THEN 'APP_INICIADA_SIN_FORMULARIOS'
            WHEN I.Flag_ProcesoApp_CacheCliente = 1
             AND I.AtencionEstadoId = 7
             AND I.Flag_FinalizadaApp_Exitosa = 0
             AND I.LogIdExito IS NULL
                THEN 'APP_PROCESADA_CIERRE_MANUAL_SIN_BPM'
            WHEN I.Flag_ProcesoApp_CacheCliente = 1
             AND I.AtencionEstadoId = 7
             AND ISNULL(I.ConteoErroresBpm, 0) > 0
             AND I.Flag_FinalizadaApp_Exitosa = 0
                THEN 'APP_CERRADA_CON_ERROR_BPM'
            WHEN I.Flag_ProcesoApp_CacheCliente = 1
             AND I.AtencionEstadoId = 6
                THEN 'APP_EN_PROCESO_FORM_CLIENTE'
            WHEN I.Flag_InicioApp = 0
             AND I.Flag_ProcesoApp_CacheCliente = 0
             AND I.AtencionEstadoId = 7
             AND I.Flag_FinalizadaApp_Exitosa = 1
                THEN 'CERRADA_EXITOSA_SIN_INDICIO_APP'
            WHEN I.Flag_InicioApp = 0
             AND I.Flag_ProcesoApp_CacheCliente = 0
                THEN 'FUERA_DE_APP_O_SIN_RASTRO'
            ELSE 'REVISAR_CASO_MIXTO'
         END AS CategoriaEmbudoApp
        ,(I.Flag_InicioApp
        + I.Flag_ProcesoApp_CacheCliente
        + I.Flag_FinalizadaApp_Exitosa) AS ScoreEmbudoApp_0a3
        /* ¿Procesada por app? Exitosa / con error BPM / no por app */
        ,CASE
            WHEN I.Flag_FinalizadaApp_Exitosa = 1
                THEN 'APP_EXITOSA'
            WHEN (I.Flag_InicioApp = 1 OR I.Flag_ProcesoApp_CacheCliente = 1)
             AND ISNULL(I.ConteoErroresBpm, 0) > 0
             AND I.Flag_FinalizadaApp_Exitosa = 0
                THEN 'APP_CON_ERROR_BPM'
            WHEN (I.Flag_InicioApp = 1 OR I.Flag_ProcesoApp_CacheCliente = 1)
             AND I.Flag_FinalizadaApp_Exitosa = 0
                THEN 'APP_USADA_SIN_CIERRE_BPM'  -- usó app pero no hay éxito ni error BPM registrado
            ELSE 'NO_PROCESADA_POR_APP'
         END AS ResultadoProcesoApp
    FROM Indicios I
)
SELECT *
INTO #EmbudoApp
FROM Detalle;

/* --------------------------------------------------------------------------
   1) Conteos por ScoreEmbudo (0..3)
   -------------------------------------------------------------------------- */
SELECT
     ScoreEmbudoApp_0a3
    ,CASE ScoreEmbudoApp_0a3
        WHEN 0 THEN 'Sin indicios app (o fuera de app)'
        WHEN 1 THEN 'Solo 1 indicio (inicio O proceso O fin)'
        WHEN 2 THEN 'Dos indicios (embudo parcial)'
        WHEN 3 THEN 'Embudo completo (inicio+proceso+fin OK)'
        ELSE 'N/A'
     END AS InterpretacionScore
    ,COUNT(*) AS Cantidad
    ,SUM(CASE WHEN AtencionEstadoId = 7 THEN 1 ELSE 0 END) AS Cerradas
    ,SUM(CASE WHEN AtencionEstadoId = 6 THEN 1 ELSE 0 END) AS Abiertas
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (), 0) AS DECIMAL(9,2)) AS PorcentajeDelTotal
FROM #EmbudoApp
GROUP BY ScoreEmbudoApp_0a3
ORDER BY ScoreEmbudoApp_0a3;

/* --------------------------------------------------------------------------
   1b) Procesadas por app: exitosas vs con error vs no por app
   -------------------------------------------------------------------------- */
SELECT
     ResultadoProcesoApp
    ,CASE ResultadoProcesoApp
        WHEN 'APP_EXITOSA' THEN 'Finalizó por app (BPM éxito)'
        WHEN 'APP_CON_ERROR_BPM' THEN 'Usó app pero BPM con error'
        WHEN 'APP_USADA_SIN_CIERRE_BPM' THEN 'Usó app (inicio/cache) sin cierre BPM éxito ni error'
        WHEN 'NO_PROCESADA_POR_APP' THEN 'Sin rastro de proceso por app'
        ELSE ResultadoProcesoApp
     END AS Interpretacion
    ,COUNT(*) AS Cantidad
    ,SUM(CASE WHEN AtencionEstadoId = 7 THEN 1 ELSE 0 END) AS Cerradas
    ,SUM(CASE WHEN AtencionEstadoId = 6 THEN 1 ELSE 0 END) AS Abiertas
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (), 0) AS DECIMAL(9,2)) AS PorcentajeDelTotal
FROM #EmbudoApp
GROUP BY ResultadoProcesoApp
ORDER BY
    CASE ResultadoProcesoApp
        WHEN 'APP_EXITOSA' THEN 1
        WHEN 'APP_CON_ERROR_BPM' THEN 2
        WHEN 'APP_USADA_SIN_CIERRE_BPM' THEN 3
        WHEN 'NO_PROCESADA_POR_APP' THEN 4
        ELSE 9
    END;

/* Vista compacta 3 bloques: Exitosa | Con error | No finalizadas por app */
SELECT
     Bloque3
    ,COUNT(*) AS Cantidad
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (), 0) AS DECIMAL(9,2)) AS PorcentajeDelTotal
FROM (
    SELECT
         CASE
            WHEN ResultadoProcesoApp = 'APP_EXITOSA' THEN '1_EXITOSAS_APP'
            WHEN ResultadoProcesoApp = 'APP_CON_ERROR_BPM' THEN '2_CON_ERROR_APP'
            ELSE '3_NO_FINALIZADAS_POR_APP'
         END AS Bloque3
    FROM #EmbudoApp
) X
GROUP BY Bloque3
ORDER BY Bloque3;

/* --------------------------------------------------------------------------
   2) Conteos por categoría (más analítico)
   -------------------------------------------------------------------------- */
SELECT
     CategoriaEmbudoApp
    ,COUNT(*) AS Cantidad
    ,SUM(CASE WHEN AtencionEstadoId = 7 THEN 1 ELSE 0 END) AS Cerradas
    ,SUM(CASE WHEN AtencionEstadoId = 6 THEN 1 ELSE 0 END) AS Abiertas
    ,AVG(CAST(ScoreEmbudoApp_0a3 AS DECIMAL(9,2))) AS ScorePromedio
    ,AVG(CAST(Minutos_InicioApp_a_BpmExitoso AS DECIMAL(18,2))) AS MinutosPromedio_Inicio_a_BPM
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (), 0) AS DECIMAL(9,2)) AS PorcentajeDelTotal
FROM #EmbudoApp
GROUP BY CategoriaEmbudoApp
ORDER BY Cantidad DESC;

/* --------------------------------------------------------------------------
   3) Cruce Score x Categoría (opcional gerencial)
   -------------------------------------------------------------------------- */
SELECT
     ScoreEmbudoApp_0a3
    ,CategoriaEmbudoApp
    ,COUNT(*) AS Cantidad
FROM #EmbudoApp
GROUP BY ScoreEmbudoApp_0a3, CategoriaEmbudoApp
ORDER BY ScoreEmbudoApp_0a3, Cantidad DESC;

/* --------------------------------------------------------------------------
   4) Por agente — volumen + score + % del total del corte
   -------------------------------------------------------------------------- */
SELECT
     AgenteId
    ,AgenteNombre
    ,COUNT(*) AS Cantidad
    ,SUM(CASE WHEN AtencionEstadoId = 7 THEN 1 ELSE 0 END) AS Cerradas
    ,SUM(CASE WHEN AtencionEstadoId = 6 THEN 1 ELSE 0 END) AS Abiertas
    ,SUM(Flag_InicioApp) AS Con_InicioApp
    ,SUM(Flag_ProcesoApp_CacheCliente) AS Con_CacheCliente
    ,SUM(Flag_FinalizadaApp_Exitosa) AS Con_FinalizadaApp_Exitosa
    ,AVG(CAST(ScoreEmbudoApp_0a3 AS DECIMAL(9,2))) AS ScorePromedio
    ,SUM(CASE WHEN ScoreEmbudoApp_0a3 = 0 THEN 1 ELSE 0 END) AS Score0
    ,SUM(CASE WHEN ScoreEmbudoApp_0a3 = 1 THEN 1 ELSE 0 END) AS Score1
    ,SUM(CASE WHEN ScoreEmbudoApp_0a3 = 2 THEN 1 ELSE 0 END) AS Score2
    ,SUM(CASE WHEN ScoreEmbudoApp_0a3 = 3 THEN 1 ELSE 0 END) AS Score3
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (), 0) AS DECIMAL(9,2)) AS PorcentajeDelTotal
    ,AVG(CAST(Minutos_InicioApp_a_BpmExitoso AS DECIMAL(18,2))) AS MinutosPromedio_Inicio_a_BPM
FROM #EmbudoApp
GROUP BY AgenteId, AgenteNombre
ORDER BY Cantidad DESC, AgenteNombre;

/* --------------------------------------------------------------------------
   5) Por agente x Score (cruce fino)
   -------------------------------------------------------------------------- */
SELECT
     AgenteId
    ,AgenteNombre
    ,ScoreEmbudoApp_0a3
    ,COUNT(*) AS Cantidad
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (PARTITION BY AgenteId), 0) AS DECIMAL(9,2)) AS PorcentajeDentroDelAgente
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (), 0) AS DECIMAL(9,2)) AS PorcentajeDelTotal
FROM #EmbudoApp
GROUP BY AgenteId, AgenteNombre, ScoreEmbudoApp_0a3
ORDER BY AgenteNombre, ScoreEmbudoApp_0a3;

/* --------------------------------------------------------------------------
   6) Por agente x Categoría
   -------------------------------------------------------------------------- */
SELECT
     AgenteId
    ,AgenteNombre
    ,CategoriaEmbudoApp
    ,COUNT(*) AS Cantidad
    ,CAST(100.0 * COUNT(*) / NULLIF(SUM(COUNT(*)) OVER (PARTITION BY AgenteId), 0) AS DECIMAL(9,2)) AS PorcentajeDentroDelAgente
FROM #EmbudoApp
GROUP BY AgenteId, AgenteNombre, CategoriaEmbudoApp
ORDER BY AgenteNombre, Cantidad DESC;

/* --------------------------------------------------------------------------
   7) Detalle fila a fila (comentar si solo quieres resúmenes)
   -------------------------------------------------------------------------- */
SELECT *
FROM #EmbudoApp
ORDER BY AtencionId DESC;

DROP TABLE #EmbudoApp;
GO
