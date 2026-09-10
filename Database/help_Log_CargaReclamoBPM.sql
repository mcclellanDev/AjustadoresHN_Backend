/*
    Tabla de auditoría para intentos de carga de reclamo BPM (Ficohsa).
    Ambientes: PROD (Carga_Reclamo_Sinau_BPM_Fico) y QA (Carga_Reclamo_Sinau_BPM_FicoQA)

    Ejecutar en la base de datos [help] antes de publicar el API.
*/

IF OBJECT_ID(N'dbo.help_Log_CargaReclamoBPM', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.help_Log_CargaReclamoBPM
    (
        Id                          INT IDENTITY(1,1) NOT NULL,
        FechaRegistro               DATETIME          NOT NULL CONSTRAINT DF_help_Log_CargaReclamoBPM_FechaRegistro DEFAULT (GETDATE()),

        -- Contexto del intento
        Ambiente                    VARCHAR(10)       NOT NULL,   -- PROD | QA
        Endpoint                    VARCHAR(100)      NOT NULL,
        UsuarioApi                  VARCHAR(150)      NULL,       -- JWT / usuario autenticado
        IpCliente                   VARCHAR(50)       NULL,
        UserAgent                   VARCHAR(500)      NULL,

        -- Resultado del intento
        HttpStatusCode              INT               NOT NULL,
        EsExitoso                   BIT               NOT NULL,
        Etapa                       VARCHAR(50)       NOT NULL,   -- BINDING | VALIDACION_FECHA | VALIDACION_EXPEDIENTE | SOAP_FICOHSA | EXCEPCION
        Mensaje                     VARCHAR(MAX)      NULL,

        -- Respuesta Ficohsa / API
        CodigoRespuestaBpm          VARCHAR(20)       NULL,
        DescripcionRespuestaBpm     VARCHAR(MAX)      NULL,
        NumeroReclamo               VARCHAR(50)       NULL,
        SolicitudBpm                VARCHAR(50)       NULL,
        FechaOcurrenciaEnviada      VARCHAR(50)       NULL,       -- valor enviado a p_fecha_ocurrencia
        JsonRequest                 VARCHAR(MAX)      NULL,
        JsonResponse                VARCHAR(MAX)      NULL,
        XmlRequestFicohsa           VARCHAR(MAX)      NULL,
        XmlRespuestaFicohsa         VARCHAR(MAX)      NULL,

        -- Referencias internas HELP
        RefAjustadorAudienciaId     INT               NULL,       -- payload.Observacion
        RefAtencionId               INT               NULL,       -- resuelto desde help_AjustadorAudiencia

        -- Campos del JSON de solicitud (desnormalizados para consulta rápida)
        Chasis                      VARCHAR(50)       NULL,
        PuntoServicio               VARCHAR(20)       NULL,
        Poliza                      VARCHAR(30)       NULL,
        Certificado                 VARCHAR(20)       NULL,
        NombreAsegurado             VARCHAR(250)      NULL,
        Sucursal                    VARCHAR(20)       NULL,
        Producto                    VARCHAR(20)       NULL,
        Cobertura                   VARCHAR(20)       NULL,
        Ramo                        VARCHAR(20)       NULL,
        FechaOcurrencia             VARCHAR(50)       NULL,
        Causa                       VARCHAR(20)       NULL,
        ValorReserva                VARCHAR(30)       NULL,
        UsuarioBPM                  VARCHAR(50)       NULL,
        Latitud                     VARCHAR(50)       NULL,
        Longitud                    VARCHAR(50)       NULL,
        NombreConductor             VARCHAR(250)      NULL,
        Genero                      VARCHAR(10)       NULL,
        Parentesco                  VARCHAR(20)       NULL,
        Observacion                 VARCHAR(50)       NULL,

        CONSTRAINT PK_help_Log_CargaReclamoBPM PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_help_Log_CargaReclamoBPM_FechaRegistro'
      AND object_id = OBJECT_ID(N'dbo.help_Log_CargaReclamoBPM')
)
    CREATE NONCLUSTERED INDEX IX_help_Log_CargaReclamoBPM_FechaRegistro
        ON dbo.help_Log_CargaReclamoBPM (FechaRegistro DESC);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_help_Log_CargaReclamoBPM_RefAjustadorAudienciaId'
      AND object_id = OBJECT_ID(N'dbo.help_Log_CargaReclamoBPM')
)
    CREATE NONCLUSTERED INDEX IX_help_Log_CargaReclamoBPM_RefAjustadorAudienciaId
        ON dbo.help_Log_CargaReclamoBPM (RefAjustadorAudienciaId, FechaRegistro DESC);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_help_Log_CargaReclamoBPM_Ambiente_Fecha'
      AND object_id = OBJECT_ID(N'dbo.help_Log_CargaReclamoBPM')
)
    CREATE NONCLUSTERED INDEX IX_help_Log_CargaReclamoBPM_Ambiente_Fecha
        ON dbo.help_Log_CargaReclamoBPM (Ambiente, FechaRegistro DESC);
GO

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
