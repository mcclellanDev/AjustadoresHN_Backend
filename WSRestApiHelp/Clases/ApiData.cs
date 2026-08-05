using Sophia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Clases
{
    public class ApiData
    {

        public class LoginDataRequest
        {
            public string User { get; set; }

            public string Password { get; set; }

        }

        public class LoginClienteDataRequest
        {
            public string User { get; set; }

            public string Password { get; set; }

            public string Host { get; set; }

            public string Dispositivo { get; set; }

        }

        public class datosFirmaCliente
        {
            public Nullable<int> IdAfiliado {  get; set; }
            public string Firma {  get; set; }
        }


        public class datosFotografiaCliente
        {
            public Nullable<int> IdAfiliado { get; set; }
            public string Fotografia { get; set; }
        }

        public class datosFotografiaClienteVehiculo
        {
            public string Identificacion { get; set; }
            public string Fotografia { get; set; }
        }

        public class KeyAccessResquest {

            public string KeyAccess { get; set; }

        }

        public class ConvenioReparacionRequest
        {
            public string CodigoDanioVehiculo { get; set; }

            public string Descripcion { get; set; }

            public string Fotografia { get; set; }

            public DateTime FechaRegistro { get; set; }

            public int IdAtencion { get; set; }

            public int RefTipoFotoId { get; set; }

            public string TipoEntidad { get; set; }

            public short TipoReparacion { get; set; }
        }

        public class DanioOtroData
        {
            public int RefAtencionId { get; set; }

            public string DescripcionDeDanio { get; set; }

            public DateTime FechaRegistro { get; set; }

            public int UsuarioId { get; set; }

            public string TipoEntidad { get; set; }

            public short TipoReparacion { get; set; }

            public string CodigoDanioVehiculo { get; set; }

            public int indexFront { get; set; }
        }

        public class CausaPorCoberturaData
        {
            public int Id { get; set; }
            public string COD_PRODUCTO { get; set; }
            public string DESCRIPCION_PROD { get; set; }
            public string COD_COBERTURA { get; set; }
            public string DESCRIPCION_COBE { get; set; }
            public string COD_CAUSA { get; set; }
            public string DESCRIPCION_CAUS { get; set; }
        }

        public partial class FotoSiniestroRequest {
            public string Foto { get; set; }

            public int IdAtencion { get; set; }

            public int RefTipoFotoId { get; set; }

            public string NombreFirmante { get; set; }

            public string Descripcion { get; set; }

            public DateTime FechaFirma { get; set; }
        }

        public partial class FotoFirmaAjustadorRequest
        {
            public string FotoFirma { get; set; }

            public int IdAgente { get; set; }
        }

        public partial class FotoAjustadorRequest
        {
            public string Fotografia { get; set; }

            public int IdAgente { get; set; }
        }

        public partial class ListaDatosFirma {

            public int IdAtencion { get; set; }

            public int TipoFotoFirma { get; set; }


        }

        public class FotoFirmaAtencionData
        {
            public byte[] FotoFirma { get; set; }
            public string NombreFoto { get; set; }
            public string Extension { get; set; }
        }

        public partial class SubirArchivosFicohsa {
            public string filetxt { get; set; }

            public string CodigoSolicitud_BPM { get; set; }

            public string NombreTipoExtensionFile { get; set; }


        }

        public class help_CacheCliente
        {
            public int IdAtencion { get; set; }
        public string Nombre { get; set; }
        public string AutoridadInvolucrada { get; set; }
        public string NombreConductor { get; set; }
        public string IdentidaConductor { get; set; }
        public string Poliza { get; set; }
        public string NombreAtribuyeAccidente { get; set; }
        public string TelefonoConductor { get; set; }
        public string CelularConductor { get; set; }
        public string CorreoElectronico { get; set; }
        public string Licencia { get; set; }
        public int Blindado { get; set; }
        public string FechaNacimientoConductor { get; set; }
        public int Edad { get; set; }
        public int AseguradoUsoPoliza { get; set; }
        public string PorqueNoUsoServicioAsistencia { get; set; }
        public int PruebaAlcoholemia { get; set; }
        public int AgendarAudiencia { get; set; }
        public int VehiculoDetenido { get; set; }
        public int ConductorDetenido { get; set; }
        public int VehiculoVolcado { get; set; }
        public string DireccionEnvioCorrespondencia { get; set; }
        public string DireccionConductor { get; set; }
        public string DondeSeEncuentraVehiculo { get; set; }
        public int TerceroResponsable { get; set; }
        public string UbicacionVehiculoDetenido { get; set; }
        public string DescripcionAudiencia { get; set; }
        public int LicenciaEstado { get; set; }
        public string Vigencia { get; set; }
        public int RefTipoLicenciaId { get; set; }
        public string TipoLicencia { get; set; }
        public string NumeroUnidad { get; set; }
        public string DescripcionTercerosMuertos { get; set; }
        public string DescripcionTercerosHeridos { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionDanio { get; set; }
        public int RefTipoConductorId { get; set; }
        public int TercerosMuertos { get; set; }
        public int TercerosHeridos { get; set; }
        public string Parentesco { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int AnioVehiculo { get; set; }
        public string ModeloVehiculo { get; set; }
        public string PlacaVehiculo { get; set; }
        public string ColorVehiculo { get; set; }
        public string MarcaVehiculo { get; set; }
        public string ChasisVehiculo { get; set; }
        public string Motor { get; set; }
        }

            public class help_ProveedorAgentesRequest {

            public int Id { get; set; }
            public int RefProveedorId { get; set; }
            public string NombreAgente { get; set; }
            public string MovilServicio { get; set; }
            public string Usuario { get; set; }
            public string Contrasena { get; set; }
            public string Fotografia { get; set; }
            public Nullable<double> LatitudActual { get; set; }
            public Nullable<double> LongitudActual { get; set; }
            public Nullable<int> EstadoId { get; set; }
            public Nullable<int> RefUsuarioRegistro { get; set; }
            public Nullable<System.DateTime> FechaLocation { get; set; }
            public string TypoAgenteString { get; set; }
            public string Correo { get; set; }
            public Nullable<int> Admin { get; set; }
            public Nullable<int> RefFicohsaId { get; set; }
            public string Token { get; set; }
            public string TokenPush { get; set; }
            public string ClaveRecuperarPassword { get; set; }
            public Nullable<int> Intentos { get; set; }

        }

        public class help_DatosNotificacionCLiente
        {
            public string titulo {  get; set; }
            public string mensaje { get; set; }
            public Nullable<short> esAtencion { get; set; }
            public Nullable<int> IdAtencion { get; set; }
	        public Nullable<short> esServicio {  get; set; }
            public string Servicio {  get; set; }
            public string Direccion {  get; set; }
            public string Ciudad {  get; set; }
            public string Colonia {  get; set; }
            public string Cliente {  get; set; }
            public string Plan {  get; set; }
            public Nullable<int> Estado {  get; set; }
            public string ColorEstado {  get; set; }
	        public Nullable<short> esProveedor {  get; set; }
            public Nullable<short> ProveedorAgenteId {  get; set; }
        }

        public class help_AjustadorAudienciaRequest
        {
            public int RefAtencionId { get; set; }
            public Nullable<int> RefProveedorAgenteId { get; set; }
            //public Nullable<int> RefProveedorAgenteAbogadoId { get; set; }
            public Nullable<int> AgendarAudiencia { get; set; }
            public Nullable<int> AseguradoUsoPoliza { get; set; }
            public Nullable<int> TerceroResponsable { get; set; }
            public Nullable<int> LesionadosSinAudiencia { get; set; }
            public string DescripcionAudiencia { get; set; }
            public Nullable<int> RefEntidadComunicativaId { get; set; }
            public Nullable<int> RefTipoAudienciaId { get; set; }
            public string Poliza { get; set; }
            public string Identificacion { get; set; }
            public string Nombre { get; set; }
            public Nullable<int> ConductorAfiliado { get; set; }
            public Nullable<int> ConductorDetenido { get; set; }
            public string Descripcion { get; set; }
            public string MarcaVehiculo { get; set; }
            public string ModeloVehiculo { get; set; }
            public Nullable<int> AnioVehiculo { get; set; }
            public string PlacaVehiculo { get; set; }
            public string ChasisVehiculo { get; set; }
            public string ColorVehiculo { get; set; }
            public Nullable<int> VehiculoDetenido { get; set; }
            public string DescripcionVehiculo { get; set; }
            public Nullable<int> TercerosHeridos { get; set; }
            public Nullable<int> TercerosMuertos { get; set; }
            public string DescripcionTercerosHeridos { get; set; }
            public string DescripcionTercerosMuertos { get; set; }
            public Nullable<int> DanioFrontal { get; set; }
            public Nullable<int> DanioTrasero { get; set; }
            public Nullable<int> DanioLateralDerecho { get; set; }
            public Nullable<int> DanioLataralIzquierdo { get; set; }
            public Nullable<int> VehiculoVolcado { get; set; }
            public string DescripcionDanio { get; set; }
            public Nullable<int> RefPaisId { get; set; }
            public Nullable<int> RefCiudadId { get; set; }
            public Nullable<int> RefDeptoId { get; set; }
            public Nullable<int> RefMunicipioId { get; set; }
            public Nullable<System.DateTime> FechaHora { get; set; }
            public string Lugar { get; set; }
            public Nullable<int> RefUsuarioId { get; set; }
            //public Nullable<System.DateTime> FechaRegistro { get; set; }
            public Nullable<int> TallerMecanicoId { get; set; }
            public string Blindado { get; set; }
            public string ObservacionTaller { get; set; }
            public string ReclamoAsegurado { get; set; }
            public string Observaciones { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string NombreConductor { get; set; }
            public string IdentidaConductor { get; set; }
            public string DPI_Pasaporte { get; set; }
            public string TelefonoConductor { get; set; }
            public string CelularConductor { get; set; }
            public Nullable<int> Edad { get; set; }
            public string Licencia { get; set; }
            public string TipoLicencia { get; set; }
            public string Vigencia { get; set; }
            public string DireccionConductor { get; set; }
            public Nullable<int> Sexo { get; set; }
            public Nullable<int> RefTipoConductorId { get; set; }
            //public string NumeroReclamoFicohsa { get; set; }

            public string JSON_PropiedadDaniadaSiniestro { get; set; }

            public string JSON_TercerosSiniestro { get; set; }

            public string CorreoElectronico { get; set; }

        }

        public partial class help_RefTablaDeAjusteAtencion
        {
            public Nullable<int> RefAtencionId { get; set; }
            public Nullable<int> RefProveedorAgenteId { get; set; }
        }

        public partial class help_AjustadorAudienciaUpdate
        {
            public Nullable<int>  IdAjustadorAudiencia { get; set; }
            public Nullable<int> RefProveedorAgenteAbogadoId { get; set; }
            public Nullable<int> AgendarAudiencia { get; set; }
            public DateTime FechaHora { get; set; }
            public string Fecha {  get; set; }
            public string Hora { get; set; }
            public string Lugar { get; set; }
        }

        public partial class help_AjustadorAudienciaRequestHN
        {
            public Nullable<int> RefAtencionId { get; set; }
            public Nullable<int> RefProveedorAgenteId { get; set; }
            public Nullable<int> RefProveedorAgenteAbogadoId { get; set; }
            public Nullable<int> AgendarAudiencia { get; set; }
            public Nullable<int> AseguradoUsoPoliza { get; set; }
            public Nullable<int> TerceroResponsable { get; set; }
            public Nullable<int> LesionadosSinAudiencia { get; set; }
            public string DescripcionAudiencia { get; set; }
            public string Poliza { get; set; }
            public string Identificacion { get; set; }
            public string Nombre { get; set; }
            public Nullable<int> ConductorAfiliado { get; set; }
            public Nullable<int> ConductorDetenido { get; set; }
            public string Descripcion { get; set; }
            public string MarcaVehiculo { get; set; }
            public string ModeloVehiculo { get; set; }
            public Nullable<int> AnioVehiculo { get; set; }
            public string PlacaVehiculo { get; set; }
            public string ChasisVehiculo { get; set; }
            public string ColorVehiculo { get; set; }
            public Nullable<int> VehiculoDetenido { get; set; }
            public string DescripcionVehiculo { get; set; }
            public Nullable<int> TercerosHeridos { get; set; }
            public Nullable<int> TercerosMuertos { get; set; }
            public string DescripcionTercerosHeridos { get; set; }
            public string DescripcionTercerosMuertos { get; set; }
            public Nullable<int> DanioFrontal { get; set; }
            public Nullable<int> DanioTrasero { get; set; }
            public Nullable<int> DanioLateralDerecho { get; set; }
            public Nullable<int> DanioLataralIzquierdo { get; set; }
            public Nullable<int> VehiculoVolcado { get; set; }
            public string DescripcionDanio { get; set; }
            public Nullable<int> RefPaisId { get; set; }
            public Nullable<int> RefCiudadId { get; set; }
            public Nullable<int> RefDeptoId { get; set; }
            public Nullable<int> RefMunicipioId { get; set; }
            public Nullable<System.DateTime> FechaHora { get; set; }
            public string Lugar { get; set; }
            public Nullable<int> RefUsuarioId { get; set; }
            public Nullable<int> TallerMecanicoId { get; set; }
            public string Blindado { get; set; }
            public string ObservacionTaller { get; set; }
            public string ReclamoAsegurado { get; set; }
            public string Observaciones { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string NombreConductor { get; set; }
            public string IdentidaConductor { get; set; }
            public string DPI_Pasaporte { get; set; }
            public string TelefonoConductor { get; set; }
            public string CelularConductor { get; set; }
            public Nullable<int> Edad { get; set; }
            public string Licencia { get; set; }
            public string TipoLicencia { get; set; }
            public string Vigencia { get; set; }
            public string DireccionConductor { get; set; }
            public Nullable<int> Sexo { get; set; }
            public Nullable<int> RefTipoConductorId { get; set; }
            public string DireccionEnvioCorrespondencia { get; set; }
            public string CorreoElectronico { get; set; }
            public Nullable<int> RefTipoLicenciaId { get; set; }
            public string NombreAtribuyeAccidente { get; set; }
            public string AutoridadInvolucrada { get; set; }
            public string UbicacionVehiculoDetenido { get; set; }
            public Nullable<int> PruebaAlcoholemia { get; set; }
            public Nullable<int> RefTipoCombustibleId { get; set; }
            public Nullable<int> AC { get; set; }
            public string Rines { get; set; }
            public Nullable<int> BolsaAire { get; set; }
            public Nullable<int> CierreCentralizado { get; set; }
            public Nullable<int> Mecanico { get; set; }
            public Nullable<int> RetrovisorElectronico { get; set; }
            public Nullable<int> Overfenders { get; set; }
            public Nullable<int> ColaPato { get; set; }
            public Nullable<int> CintaDecorativa { get; set; }
            public Nullable<int> LicenciaEstado { get; set; }
            public string ObservacionesFinales { get; set; }
            public Nullable<int> RefTipoSolicitanteInformeAjusteId { get; set; }
            public string TipoAcuerdoFicohsa { get; set; }
            public string DondeSeEncuentraVehiculo { get; set; }

            public string NumeroUnidad { get; set; }

            public string Parentesco { get; set; }

            public Nullable<DateTime> FechaNacimientoConductor { get; set; }

            public Nullable<int> CulpableCompromisoPago { get; set; }

            public string ObservacionCompromisoPago { get; set; }

            public string PorqueNoUsoServicioAsistencia { get; set; }

            public Nullable<int> Kilometraje { get; set; }

            public string OtrosTalleres { get; set; }
        }

        public partial class help_AjustadorInformeAjustadortHN
        {
            public Nullable<int> RefAtencionId { get; set; }
            public Nullable<int> RefProveedorAgenteId { get; set; }
            public string Identificacion { get; set; }
            public string Descripcion { get; set; }
            public Nullable<int> DanioFrontal { get; set; }
            public Nullable<int> DanioTrasero { get; set; }
            public Nullable<int> DanioLateralDerecho { get; set; }
            public Nullable<int> DanioLataralIzquierdo { get; set; }
            public Nullable<int> VehiculoVolcado { get; set; }
            public string DescripcionDanio { get; set; }
            public Nullable<int> TallerMecanicoId { get; set; }
            public string Blindado { get; set; }
            public string ObservacionTaller { get; set; }
            public string Observaciones { get; set; }
            public string DPI_Pasaporte { get; set; }
            public Nullable<int> Sexo { get; set; }
            public Nullable<int> RefTipoCombustibleId { get; set; }
            public Nullable<int> AC { get; set; }
            public string Rines { get; set; }
            public Nullable<int> BolsaAire { get; set; }
            public Nullable<int> CierreCentralizado { get; set; }
            public Nullable<int> Mecanico { get; set; }
            public Nullable<int> RetrovisorElectronico { get; set; }
            public Nullable<int> Overfenders { get; set; }
            public Nullable<int> ColaPato { get; set; }
            public Nullable<int> CintaDecorativa { get; set; }
            public string ObservacionesFinales { get; set; }
            public Nullable<int> RefTipoSolicitanteInformeAjusteId { get; set; }
            public string TipoAcuerdoFicohsa { get; set; }

            public string Parentesco { get; set; }

            public Nullable<int> CompromisoPago { get; set; }

            public string ObservacionCompromisoPago { get; set; }

            public Nullable<int> Kilometraje { get; set; }
        }

        public class help_PropiedadDaniadaSiniestro
        {
            public Nullable<int> RefAjustadorAudienciaId { get; set; }
            public string Nombre { get; set; }
            public string Direccion { get; set; }
            public string DaniosPrivado { get; set; }
            public Nullable<int> RefAjustadorId { get; set; }
            public string Telefono { get; set; }
            public string DescripcionDelBien { get; set; }
            public string DireccionDelBien { get; set; }
            public string DescripcionDelDanio { get; set; }
            public string NombreDelBienAfectado { get; set; }
        }

        public class help_LogDatosGps
        {
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public int RefAtencionId { get; set; }
            public Nullable<int> RefUsuarioId { get; set; }
            public string Tipo { get; set; }
            public int Contador { get; set; }
        }

        public class SubirDatosVideo
        {
            public int IdAtencion { get; set; }
            public HttpPostedFileBase File { get; set; }
           
        }


        public class help_FiniquitoManual 
        {
            public string NumeroReclamo { get; set; }
            public Nullable<System.DateTime> FechaDesde { get; set; }
            public Nullable<System.DateTime> FechaHasta { get; set; }
            public string NombreCliente { get; set; }
            public string TipoCoberturaFicohsa { get; set; }
            public Nullable<System.DateTime> FechaFirma { get; set; }
            public string FirmaCliente { get; set; }
            public string NombreQuienRecibe { get; set; }
            public string IdentidadQuienRecibe { get; set; }
            public string NumeroCheque { get; set; }
            public Nullable<System.DateTime> FechaDelCheque { get; set; }
            public Nullable<decimal> ValorDelCheque { get; set; }
            public string NombreAFavor { get; set; }
            public string Poliza { get; set; }
            public Nullable<int> RefAtencionId { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public Nullable<int> Anio { get; set; }
            public string Placa { get; set; }
            public string Chasis { get; set; }
            public string Motor { get; set; }
        }
            
        public class help_TercerosSiniestros
        {
            public Nullable<int> RefIdAjustadorAudiencia { get; set; }
            public string NombrePropetario { get; set; }
            public string NombrePiloto { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Placa { get; set; }
            public string Chasis { get; set; }
            public string Telefono { get; set; }
            public string DaniosObservacion { get; set; }
            public Nullable<int> RefAgenteProveedorId { get; set; }

            public string Direccion { get; set; }
        }

        public class help_TercerosReclamo
        {
            public Nullable<int> RefAjustadorAudienciaId { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
            public string DPI_Pasaporte { get; set; }
            public string Telefono { get; set; }
            public string Celular { get; set; }
            public Nullable<System.DateTime> Vigencia { get; set; }
            public string TipoLicencia { get; set; }
            public string Licencia { get; set; }
            public Nullable<int> Edad { get; set; }
            public Nullable<int> Sexo { get; set; }
            public Nullable<int> Culpalble { get; set; }
            public Nullable<int> Afectado { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string TipoVehiculo { get; set; }
            public Nullable<int> Anio { get; set; }
            public string Placa { get; set; }
            public string Color { get; set; }
            public string Chasis { get; set; }
            public string Motor { get; set; }
            public Nullable<int> RefAgenteProveedorId { get; set; }
            public string TipoTercero { get; set; }
            public string Direccion { get; set; }
        }

        public class help_PersonasSiniestrosRequest
        {
            public string Nombre { get; set; }
            public string Telefono { get; set; }
            public string Direccion { get; set; }
            public string DescripcionLesion { get; set; }
            public string DireccionHospitalizacion { get; set; }
            public Nullable<int> TipoPersonaSiniestro { get; set; }
            public Nullable<int> RefAjustadorAudienciaId { get; set; }

        }

        public partial class help_CargaReclamoSinau {

            public string Chasis { get; set; }
            public string puntoServicio { get; set; }
            public string Poliza { get; set; }
            public string Certificado { get; set; }
            public string NombreAsegurado { get; set; }
            public string Sucursal { get; set; }
            public string Producto { get; set; }
            public string Cobertura { get; set; }
            public string Ramo { get; set; }
            public string FechaOcurrencia { get; set; }
            public string Causa { get; set; }
            public string ValorReserva { get; set; }
            public string UsuarioBPM { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string NombreConductor { get; set; }
            public string Genero { get; set; }
            public string Parentesco { get; set; }
            public string Observacion { get; set; }

        }

        public partial class help_CargaReclamoSinauQA
        {

            public string Chasis { get; set; }
            public string puntoServicio { get; set; }
            public string Poliza { get; set; }
            public string Certificado { get; set; }
            public string NombreAsegurado { get; set; }
            public string Sucursal { get; set; }
            public string Producto { get; set; }
            public string Cobertura { get; set; }
            public string Ramo { get; set; }
            public string FechaOcurrencia { get; set; }
            public string Causa { get; set; }
            public string ValorReserva { get; set; }
            public string UsuarioBPM { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public string NombreConductor { get; set; }
            public string Genero { get; set; }
            public string Parentesco { get; set; }
            public string Observacion { get; set; }

        }

        public partial class  CoberturaData 
        {
            public string pNumPoliza { get; set; }
            public string pNumSiniestro { get; set; }
            public string pNumCertificado { get; set; }
            public string pNumEndoso { get; set; }
            public string pNumAsegurado { get; set; }
        }

        public class ConsultaCausasData
        {
            public string pCodProd { get; set; }
            public string cCodCobert { get; set; }
        }

        public partial class help_Consultar_Asegurado 
        {
            public string Nombre { get; set; } 
            public string Apellido { get; set; }
            public string Poliza { get; set; }
            public string Certificado { get; set; }
            public string Identificacion { get; set; }
            public string Placa { get; set; }
            public string Chasis { get; set; }
        }

        public class Atencion_Reconocimiento_Deuda
        {
            public int Id { get; set; }

            public string NombreDeudor { get; set; }

            public string IdentidadDeudor { get; set; }

            public string Celular { get; set; }

            public string Domicilio { get; set; }

            public string CorreoElectronico { get; set; }

            public string LugarDeTrabajo { get; set; }

            public string NombreContacto { get; set; }

            public string CelularContacto { get; set; }

            public DateTime? FechaRegistroDocumento { get; set; }

            public string Marca { get; set; }

            public string Modelo { get; set; }

            public int? Anio { get; set; }

            public string Placa { get; set; }

            public string NombreAsegurado { get; set; }

            public string MarcaImplicado { get; set; }

            public string ModeloImplicado { get; set; }

            public string AnioImplicado { get; set; }

            public string PlacaImplicado { get; set; }

            public string MotorImplicado { get; set; }

            public string ChasisImplicado { get; set; }

            public string NombrePersonaPropiedadImplicada { get; set; }

            public string DanioCausadoObservacion { get; set; }

            public int? RefAtencionId { get; set; }

            public DateTime? FechaRegistro { get; set; }

            public string FirmaDeudor { get; set; }

            public string Ciudad { get; set; }

            public string TelefonoCulpable { get; set; }

            public string LicenciaCulpable { get; set; }

            public string TipoLicencia { get; set; }

            public DateTime? FechaVencimientoLicencia { get; set; }

            public string NombreDireccionPropietarioVehiculoImplicado { get; set; }

            public int? CompromisoDePago { get; set; }

            public string Observaciones { get; set; }

            public int? Edad { get; set; }

            public int? RefTipoFotografiaIdAdeudado { get; set; }

            public int? RefTipoFotografiaIdBeneficiario { get; set; }
        }

        public class GenericResponse
        {
            public int CodigoRespuesta { get; set; }
            public string Mensaje { get; set; }
            public List<object> Data { get; set; }
        }
    }
}
