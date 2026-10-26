# Ejecutar con Windows PowerShell (.NET Framework), despues de compilar Release.
# No conecta a SQL ni envia notificaciones.
$ErrorActionPreference = 'Stop'
$binPath = Join-Path (Split-Path $PSScriptRoot -Parent) 'WSRestApiHelp\bin'
Add-Type -AssemblyName System.Net.Http
Add-Type -AssemblyName System.Web
[void][Reflection.Assembly]::LoadFrom((Join-Path $binPath 'Newtonsoft.Json.dll'))
[void][Reflection.Assembly]::LoadFrom((Join-Path $binPath 'System.Net.Http.Formatting.dll'))
[void][Reflection.Assembly]::LoadFrom((Join-Path $binPath 'System.Web.Http.dll'))
$assembly = [Reflection.Assembly]::LoadFrom((Join-Path $binPath 'WSRestApiHelp.dll'))
$controllerType = $assembly.GetType('WSRestApiHelp.Controllers.PushDiagnosticoController', $true)
$parser = $controllerType.GetMethod('TryGetNotificationId', [Reflection.BindingFlags]'NonPublic,Static')
$cases = @(
    @{ Body = '{"id":"b45594fa-274f-4a37-b324-77761cbb46f6"}'; Expected = $true },
    @{ Body = '{"id":"b45594fa-274f-4a37-b324-77761cbb46f6","errors":{"invalid_player_ids":["other"]}}'; Expected = $true },
    @{ Body = '{"id":"","errors":["All included players are not subscribed"]}'; Expected = $false },
    @{ Body = '{"errors":["Access denied"]}'; Expected = $false },
    @{ Body = '{"id":null}'; Expected = $false },
    @{ Body = '{"id":123}'; Expected = $false },
    @{ Body = '{"id":"00000000-0000-0000-0000-000000000000"}'; Expected = $false },
    @{ Body = '<html>Gateway error</html>'; Expected = $false },
    @{ Body = ''; Expected = $false }
)
foreach ($case in $cases) {
    $arguments = [object[]]@($case.Body, $null)
    $actual = $parser.Invoke($null, $arguments)
    if ($actual -ne $case.Expected) { throw 'Clasificacion de respuesta OneSignal incorrecta.' }
    if (!$actual -and $null -ne $arguments[1]) { throw 'Un rechazo devolvio identificador.' }
}
if (!$controllerType.IsDefined([System.Web.Http.AuthorizeAttribute], $true)) {
    throw 'Falta Authorize.'
}
$action = $controllerType.GetMethod('ProbarNotificacionPush')
if ($action.GetParameters().Count -ne 0) { throw 'La prueba no debe aceptar destinatarios.' }
if (!$action.IsDefined([System.Web.Http.HttpPostAttribute], $true)) { throw 'Debe requerir POST.' }

# Solo descubrir el controlador nuevo para evitar dependencias ajenas a esta prueba.
Add-Type -ReferencedAssemblies @(
    (Join-Path $binPath 'System.Web.Http.dll'),
    (Join-Path $binPath 'WSRestApiHelp.dll')
) -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;
public class PushTestControllerResolver : IHttpControllerTypeResolver
{
    public PushTestControllerResolver()
    {
        // El host PowerShell no aplica los bindingRedirect de Web.config.
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
            if (new System.Reflection.AssemblyName(args.Name).Name != "Newtonsoft.Json") return null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                if (assembly.GetName().Name == "Newtonsoft.Json") return assembly;
            return null;
        };
    }
    public ICollection<Type> GetControllerTypes(IAssembliesResolver resolver)
    {
        return new[] { typeof(WSRestApiHelp.Controllers.PushDiagnosticoController) };
    }
}
'@
$resolver = New-Object PushTestControllerResolver
$configuration = New-Object System.Web.Http.HttpConfiguration
$configuration.Services.Replace([System.Web.Http.Dispatcher.IHttpControllerTypeResolver], $resolver)
[System.Web.Http.HttpConfigurationExtensions]::MapHttpAttributeRoutes($configuration)
$configuration.EnsureInitialized()
$server = New-Object System.Web.Http.HttpServer($configuration)
$client = New-Object System.Net.Http.HttpClient($server)
try {
    [Threading.Thread]::CurrentPrincipal = New-Object Security.Principal.GenericPrincipal(
        (New-Object Security.Principal.GenericIdentity('')), [string[]]@())
    $response = $client.PostAsync('http://localhost/api/Proveedor/ProbarNotificacionPush',
        (New-Object System.Net.Http.StringContent(''))).GetAwaiter().GetResult()
    if ([int]$response.StatusCode -ne 401) { throw ('Esperado 401; recibido '+$response.StatusCode) }
    Write-Output 'PASS: 9 respuestas OneSignal; POST sin destinatario; ruta HTTP anonima rechazada con 401.'
} finally {
    $client.Dispose()
    $server.Dispose()
    $configuration.Dispose()
}
