using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Results;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Clases
{
    /// <summary>
    /// Autorización por recurso: el IdProveedorAgente del request debe coincidir con el del JWT.
    /// </summary>
    public static class AgenteAccessHelper
    {
        public const string ProveedorAgenteIdClaimType = "ProveedorAgenteId";

        public static bool TryResolveProveedorAgenteId(IPrincipal principal, helpEntities db, out int proveedorAgenteId)
        {
            proveedorAgenteId = 0;
            if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
            {
                return false;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal != null)
            {
                Claim claim = claimsPrincipal.FindFirst(ProveedorAgenteIdClaimType);
                if (claim != null && int.TryParse(claim.Value, out proveedorAgenteId) && proveedorAgenteId > 0)
                {
                    return true;
                }
            }

            if (db == null || string.IsNullOrWhiteSpace(principal.Identity.Name))
            {
                return false;
            }

            string usuario = principal.Identity.Name.Trim();
            int? id = db.help_ProveedorAgentes
                .Where(x => x.Usuario == usuario)
                .Select(x => (int?)x.Id)
                .FirstOrDefault();

            if (!id.HasValue || id.Value <= 0)
            {
                return false;
            }

            proveedorAgenteId = id.Value;
            return true;
        }

        public static IHttpActionResult RequireMatchingAgente(
            ApiController controller,
            helpEntities db,
            int requestedProveedorAgenteId,
            out int authenticatedProveedorAgenteId)
        {
            authenticatedProveedorAgenteId = 0;

            if (!TryResolveProveedorAgenteId(controller.User, db, out authenticatedProveedorAgenteId))
            {
                return new NegotiatedContentResult<object>(
                    HttpStatusCode.Unauthorized,
                    new { Message = "No se pudo resolver el agente autenticado." },
                    controller);
            }

            if (requestedProveedorAgenteId <= 0)
            {
                return new BadRequestErrorMessageResult("IdProveedorAgente invalido.", controller);
            }

            if (requestedProveedorAgenteId != authenticatedProveedorAgenteId)
            {
                return new NegotiatedContentResult<object>(
                    HttpStatusCode.Forbidden,
                    new { Message = "No autorizado para consultar o modificar datos de otro agente." },
                    controller);
            }

            return null;
        }

        public static IHttpActionResult RequireAtencionOwnedByAgente(
            ApiController controller,
            helpEntities db,
            int idAtencion,
            out int authenticatedProveedorAgenteId)
        {
            authenticatedProveedorAgenteId = 0;

            if (!TryResolveProveedorAgenteId(controller.User, db, out authenticatedProveedorAgenteId))
            {
                return new NegotiatedContentResult<object>(
                    HttpStatusCode.Unauthorized,
                    new { Message = "No se pudo resolver el agente autenticado." },
                    controller);
            }

            if (idAtencion <= 0)
            {
                return new BadRequestErrorMessageResult("IdAtencion invalido.", controller);
            }

            var atencion = db.help_Atenciones
                .Where(a => a.Id == idAtencion)
                .Select(a => new { a.Id, a.RefProveedorId })
                .FirstOrDefault();

            if (atencion == null)
            {
                return new BadRequestErrorMessageResult("Atencion no encontrada.", controller);
            }

            if (!atencion.RefProveedorId.HasValue || atencion.RefProveedorId.Value != authenticatedProveedorAgenteId)
            {
                return new NegotiatedContentResult<object>(
                    HttpStatusCode.Forbidden,
                    new { Message = "No autorizado para acceder a esta atencion." },
                    controller);
            }

            return null;
        }
    }
}
