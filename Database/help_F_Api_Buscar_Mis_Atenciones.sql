USE [help]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Descripción: Búsqueda de atenciones del agente más allá del listado
--              universal (25). Filtros opcionales por rango de fechas
--              y aproximación de nombre de asegurado (Cliente).
-- Endpoint API: POST /api/Proveedor/BuscarMisAtenciones
-- =============================================
CREATE FUNCTION [dbo].[help_F_Api_Buscar_Mis_Atenciones]
(
	@UsuarioCabina     INT,
	@FechaDesde        DATETIME      = NULL,
	@FechaHasta        DATETIME      = NULL,
	@NombreAsegurado   NVARCHAR(200) = NULL,
	@Offset            INT           = 0,
	@PageSize          INT           = 25
)
RETURNS TABLE
AS
RETURN
(
	SELECT DISTINCT
		 A.[Id]
		,A.[Poliza]
		,A.[Plan]
		,A.[Certificado]
		,A.[RefIdentificacionTipo]
		,A.[Identificacion]
		,A.[TelefonoOrigen]
		,A.[Cliente]
		,A.[RefTipoServicioId]
		,A.[FechaInicio]
		,A.[FechaFin]
		,A.[RefAgenteId]
		,A.[RefProveedorId]
		,A.[RefClienteId]
		,A.[RefEstadoId]
		,A.[RefCiudadId]
		,A.[RefColoniaId]
		,A.[FechaPagoCliente]
		,A.[IdPoliza]
		,A.[Direccion]
		,A.[Observaciones]
		,A.[LatitudCliente]
		,A.[LongitudCliente]
		,A.[RefGestorId]
		,S.[TiempoDeEsperaMaximo] AS TiempoMaximoEspera
		,E.Id AS IdEstadoAtencion
		,ISNULL(E.EstadoAtencion, '') AS LbEstado
		,ISNULL(E.Color, '') AS ColorEstado
		,[TipoAtencion] AS LbTipoServicio
		,[RefCategoriaServiciosId] AS IdCategoriaServicio
		,CAT.Id AS IdCategoria
		,ISNULL(CAT.NombreCategoria, '') AS LbCategoriaServicio
		,CAT.[IconClassName]
		,U.Nombre AS UsuarioRegistroNombre
		,ISNULL(A.NombreConvenio, 'N/D') AS LbConvenio
		,ISNULL(A.[Plan], 'N/D') AS LbPlan
		,ISNULL((SELECT pr.NombreProveedor FROM help..help_Proveedor pr WHERE pr.Id = ATS.[RefProveedorId]), 'N/D') AS NombreProveedor
		,Pai.Pais
		,A.RefPaisId
		,A.Zona
		,A.DireccionDestino
		,EAS.Estado AS EstadoServicio
		,ISNULL((SELECT Ciudad FROM sophia..Ciudad C WHERE C.Id = A.RefCiudadId), 'N/D') AS Ciudad
		,ISNULL((SELECT Colonia FROM sophia..Colonia Col WHERE Col.Id = A.RefColoniaId), 'N/D') AS Colonia
	FROM [dbo].[help_Atenciones] A WITH (NOLOCK)
	INNER JOIN [Sophia].[dbo].[Usuario] U WITH (NOLOCK) ON U.Id = A.RefGestorId
	INNER JOIN [help].[dbo].[help_Servicios] S WITH (NOLOCK) ON S.Id = A.RefTipoServicioId
	INNER JOIN [help].[dbo].[help_RefServiciosCategoria] CAT WITH (NOLOCK) ON CAT.Id = S.RefCategoriaServiciosId
	INNER JOIN [Sophia].[dbo].[Pais] Pai WITH (NOLOCK) ON Pai.Id = A.RefPaisId
	INNER JOIN [help].[dbo].[help_RefEstadoAtencion] E WITH (NOLOCK) ON E.Id = A.RefEstadoId
	INNER JOIN [help].[dbo].[help_Atenciones_Servicios] ATS WITH (NOLOCK) ON ATS.RefAtencionId = A.Id
	INNER JOIN [help].[dbo].[help_RefEstadoAtencionServicio] EAS WITH (NOLOCK) ON EAS.Id = ATS.RefIdEstado
	WHERE A.RefProveedorId = @UsuarioCabina
	  AND A.[RefPlanId] NOT IN (762)
	  AND (
			@FechaDesde IS NOT NULL
		 OR @FechaHasta IS NOT NULL
		 OR (NULLIF(LTRIM(RTRIM(@NombreAsegurado)), '') IS NOT NULL)
	  )
	  AND (@FechaDesde IS NULL OR A.FechaInicio >= @FechaDesde)
	  AND (@FechaHasta IS NULL OR A.FechaInicio < DATEADD(DAY, 1, CAST(@FechaHasta AS DATE)))
	  AND (
			NULLIF(LTRIM(RTRIM(@NombreAsegurado)), '') IS NULL
		 OR A.Cliente LIKE '%' + LTRIM(RTRIM(@NombreAsegurado)) + '%'
	  )
	ORDER BY A.FechaInicio DESC
	OFFSET CASE WHEN ISNULL(@Offset, 0) < 0 THEN 0 ELSE ISNULL(@Offset, 0) END ROWS
	FETCH NEXT CASE
		WHEN ISNULL(@PageSize, 25) < 1 THEN 25
		WHEN ISNULL(@PageSize, 25) > 25 THEN 25
		ELSE ISNULL(@PageSize, 25)
	END ROWS ONLY
)
GO
