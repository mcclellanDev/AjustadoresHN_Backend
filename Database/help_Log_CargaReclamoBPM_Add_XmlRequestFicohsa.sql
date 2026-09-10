USE [help]
GO

-- Columna de XML request (si aún no existe)
IF COL_LENGTH('dbo.help_Log_CargaReclamoBPM', 'XmlRequestFicohsa') IS NULL
BEGIN
    ALTER TABLE dbo.help_Log_CargaReclamoBPM
    ADD XmlRequestFicohsa VARCHAR(MAX) NULL;
END
GO

-- Recrear SP para incluir XmlRequestFicohsa
IF OBJECT_ID(N'dbo.help_P_Log_CargaReclamoBPM_Insert', N'P') IS NOT NULL
    DROP PROCEDURE dbo.help_P_Log_CargaReclamoBPM_Insert;
GO

CREATE PROCEDURE dbo.help_P_Log_CargaReclamoBPM_Insert
    @Ambiente                   VARCHAR(10),
    @Endpoint                   VARCHAR(100),
    @UsuarioApi                 VARCHAR(150)    = NULL,
    @IpCliente                  VARCHAR(50)     = NULL,
    @UserAgent                  VARCHAR(500)    = NULL,
    @HttpStatusCode             INT,
    @EsExitoso                  BIT,
    @Etapa                      VARCHAR(50),
    @Mensaje                    VARCHAR(MAX)    = NULL,
    @CodigoRespuestaBpm         VARCHAR(20)     = NULL,
    @DescripcionRespuestaBpm    VARCHAR(MAX)    = NULL,
    @NumeroReclamo              VARCHAR(50)     = NULL,
    @SolicitudBpm               VARCHAR(50)     = NULL,
    @FechaOcurrenciaEnviada     VARCHAR(50)     = NULL,
    @JsonRequest                VARCHAR(MAX)    = NULL,
    @JsonResponse               VARCHAR(MAX)    = NULL,
    @XmlRequestFicohsa          VARCHAR(MAX)    = NULL,
    @XmlRespuestaFicohsa        VARCHAR(MAX)    = NULL,
    @RefAjustadorAudienciaId    INT             = NULL,
    @RefAtencionId              INT             = NULL,
    @Chasis                     VARCHAR(50)     = NULL,
    @PuntoServicio              VARCHAR(20)     = NULL,
    @Poliza                     VARCHAR(30)     = NULL,
    @Certificado                VARCHAR(20)     = NULL,
    @NombreAsegurado            VARCHAR(250)    = NULL,
    @Sucursal                   VARCHAR(20)     = NULL,
    @Producto                   VARCHAR(20)     = NULL,
    @Cobertura                  VARCHAR(20)     = NULL,
    @Ramo                       VARCHAR(20)     = NULL,
    @FechaOcurrencia            VARCHAR(50)     = NULL,
    @Causa                      VARCHAR(20)     = NULL,
    @ValorReserva               VARCHAR(30)     = NULL,
    @UsuarioBPM                 VARCHAR(50)     = NULL,
    @Latitud                    VARCHAR(50)     = NULL,
    @Longitud                   VARCHAR(50)     = NULL,
    @NombreConductor            VARCHAR(250)    = NULL,
    @Genero                     VARCHAR(10)     = NULL,
    @Parentesco                 VARCHAR(20)     = NULL,
    @Observacion                VARCHAR(50)     = NULL,
    @NuevoId                    INT             OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.help_Log_CargaReclamoBPM
    (
        Ambiente, Endpoint, UsuarioApi, IpCliente, UserAgent,
        HttpStatusCode, EsExitoso, Etapa, Mensaje,
        CodigoRespuestaBpm, DescripcionRespuestaBpm, NumeroReclamo, SolicitudBpm,
        FechaOcurrenciaEnviada, JsonRequest, JsonResponse, XmlRequestFicohsa, XmlRespuestaFicohsa,
        RefAjustadorAudienciaId, RefAtencionId,
        Chasis, PuntoServicio, Poliza, Certificado, NombreAsegurado, Sucursal,
        Producto, Cobertura, Ramo, FechaOcurrencia, Causa, ValorReserva, UsuarioBPM,
        Latitud, Longitud, NombreConductor, Genero, Parentesco, Observacion
    )
    VALUES
    (
        @Ambiente, @Endpoint, @UsuarioApi, @IpCliente, @UserAgent,
        @HttpStatusCode, @EsExitoso, @Etapa, @Mensaje,
        @CodigoRespuestaBpm, @DescripcionRespuestaBpm, @NumeroReclamo, @SolicitudBpm,
        @FechaOcurrenciaEnviada, @JsonRequest, @JsonResponse, @XmlRequestFicohsa, @XmlRespuestaFicohsa,
        @RefAjustadorAudienciaId, @RefAtencionId,
        @Chasis, @PuntoServicio, @Poliza, @Certificado, @NombreAsegurado, @Sucursal,
        @Producto, @Cobertura, @Ramo, @FechaOcurrencia, @Causa, @ValorReserva, @UsuarioBPM,
        @Latitud, @Longitud, @NombreConductor, @Genero, @Parentesco, @Observacion
    );

    SET @NuevoId = SCOPE_IDENTITY();
END
GO
