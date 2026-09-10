/*
    Tabla de auditoría para asignación / agendamiento de audiencias a abogados.
    Se alimenta desde POST api/Proveedor/ActualizarAudicion

    Ejecutar en la base de datos [help] antes de publicar el API.
*/

IF OBJECT_ID(N'dbo.help_Log_AsignacionAudienciaAbogado', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.help_Log_AsignacionAudienciaAbogado
    (
        Id                              INT IDENTITY(1,1) NOT NULL,
        FechaRegistro                   DATETIME          NOT NULL CONSTRAINT DF_help_Log_AsignacionAudienciaAbogado_FechaRegistro DEFAULT (GETDATE()),

        -- Contexto del intento
        Endpoint                        VARCHAR(100)      NOT NULL,
        UsuarioApi                      VARCHAR(150)      NULL,
        IpCliente                       VARCHAR(50)       NULL,
        UserAgent                       VARCHAR(500)      NULL,

        -- Resultado del intento
        HttpStatusCode                  INT               NOT NULL,
        EsExitoso                       BIT               NOT NULL,
        Etapa                           VARCHAR(50)       NOT NULL,   -- VALIDACION_FECHA | AGENDAMIENTO_BD | VALIDACION_ATENCION | NOTIFICACION | EXITO | EXCEPCION
        Mensaje                         VARCHAR(MAX)      NULL,
        JsonRequest                     VARCHAR(MAX)      NULL,
        JsonResponse                    VARCHAR(MAX)      NULL,

        -- Referencias internas HELP
        RefAjustadorAudienciaId         INT               NULL,
        RefAtencionId                   INT               NULL,
        RefProveedorAgenteAbogadoId     INT               NULL,
        IdAgente                        INT               NULL,
        AbogadoNombre                   VARCHAR(250)      NULL,
        NotificacionPushEnviada         BIT               NULL,

        -- Campos de la solicitud (desnormalizados)
        AgendarAudiencia                INT               NULL,
        FechaHora                       DATETIME          NULL,
        Fecha                           VARCHAR(20)       NULL,
        Hora                            VARCHAR(20)       NULL,
        Lugar                           VARCHAR(500)      NULL,
        FechaHoraCalculada              DATETIME          NULL,

        CONSTRAINT PK_help_Log_AsignacionAudienciaAbogado PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_help_Log_AsignacionAudienciaAbogado_FechaRegistro'
      AND object_id = OBJECT_ID(N'dbo.help_Log_AsignacionAudienciaAbogado')
)
    CREATE NONCLUSTERED INDEX IX_help_Log_AsignacionAudienciaAbogado_FechaRegistro
        ON dbo.help_Log_AsignacionAudienciaAbogado (FechaRegistro DESC);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_help_Log_AsignacionAudienciaAbogado_RefAjustadorAudienciaId'
      AND object_id = OBJECT_ID(N'dbo.help_Log_AsignacionAudienciaAbogado')
)
    CREATE NONCLUSTERED INDEX IX_help_Log_AsignacionAudienciaAbogado_RefAjustadorAudienciaId
        ON dbo.help_Log_AsignacionAudienciaAbogado (RefAjustadorAudienciaId, FechaRegistro DESC);
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_help_Log_AsignacionAudienciaAbogado_RefAtencionId'
      AND object_id = OBJECT_ID(N'dbo.help_Log_AsignacionAudienciaAbogado')
)
    CREATE NONCLUSTERED INDEX IX_help_Log_AsignacionAudienciaAbogado_RefAtencionId
        ON dbo.help_Log_AsignacionAudienciaAbogado (RefAtencionId, FechaRegistro DESC);
GO

IF OBJECT_ID(N'dbo.help_P_Log_AsignacionAudienciaAbogado_Insert', N'P') IS NOT NULL
    DROP PROCEDURE dbo.help_P_Log_AsignacionAudienciaAbogado_Insert;
GO

CREATE PROCEDURE dbo.help_P_Log_AsignacionAudienciaAbogado_Insert
    @Endpoint                        VARCHAR(100),
    @UsuarioApi                      VARCHAR(150)    = NULL,
    @IpCliente                       VARCHAR(50)     = NULL,
    @UserAgent                       VARCHAR(500)    = NULL,
    @HttpStatusCode                  INT,
    @EsExitoso                       BIT,
    @Etapa                           VARCHAR(50),
    @Mensaje                         VARCHAR(MAX)    = NULL,
    @JsonRequest                     VARCHAR(MAX)    = NULL,
    @JsonResponse                    VARCHAR(MAX)    = NULL,
    @RefAjustadorAudienciaId         INT             = NULL,
    @RefAtencionId                   INT             = NULL,
    @RefProveedorAgenteAbogadoId     INT             = NULL,
    @IdAgente                        INT             = NULL,
    @AbogadoNombre                   VARCHAR(250)    = NULL,
    @NotificacionPushEnviada         BIT             = NULL,
    @AgendarAudiencia                INT             = NULL,
    @FechaHora                       DATETIME        = NULL,
    @Fecha                           VARCHAR(20)     = NULL,
    @Hora                            VARCHAR(20)     = NULL,
    @Lugar                           VARCHAR(500)    = NULL,
    @FechaHoraCalculada              DATETIME        = NULL,
    @NuevoId                         INT             OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.help_Log_AsignacionAudienciaAbogado
    (
        Endpoint, UsuarioApi, IpCliente, UserAgent,
        HttpStatusCode, EsExitoso, Etapa, Mensaje,
        JsonRequest, JsonResponse,
        RefAjustadorAudienciaId, RefAtencionId, RefProveedorAgenteAbogadoId, IdAgente,
        AbogadoNombre, NotificacionPushEnviada,
        AgendarAudiencia, FechaHora, Fecha, Hora, Lugar, FechaHoraCalculada
    )
    VALUES
    (
        @Endpoint, @UsuarioApi, @IpCliente, @UserAgent,
        @HttpStatusCode, @EsExitoso, @Etapa, @Mensaje,
        @JsonRequest, @JsonResponse,
        @RefAjustadorAudienciaId, @RefAtencionId, @RefProveedorAgenteAbogadoId, @IdAgente,
        @AbogadoNombre, @NotificacionPushEnviada,
        @AgendarAudiencia, @FechaHora, @Fecha, @Hora, @Lugar, @FechaHoraCalculada
    );

    SET @NuevoId = SCOPE_IDENTITY();
END
GO
