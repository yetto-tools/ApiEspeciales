using ApiEspeciales.Modules.Especiales2.Models;

namespace ApiEspeciales.Modules.Especiales2.Interfaces
{

    internal interface IEspecieles2Statement
    {
        string CambiarEstado(int codigoTraslado, int estadoTraslado);
        string DetalleTraslado(int codigoTraslado);
        string DetalleTrasladoEdicion(int codigoTraslado);
        Dictionary<string, string> EliminarTraslado(int codigoTraslado);
        string EliminarDetalleTraslado(string codigoEmpresa, string codigoSerie, string codigoPedido);
        string Generar(DateTime fechaOperacion, string usuario, int codigoRutaEspeciales = 332);
        string Importaciones();
        string ImportacionPorfecha(string fechaOperacion);
        string InsertDetalle(int codigoTraslado, string usuario, string fechaOperacionOperacion, int codigoRutaEspeciales = 332);
        string InsertEncabezado();

        Dictionary<string, string> ModicacionDetalleTraslado(int codigo, string usuario, int codigoRutaEspeciales = 332);
        string NumeroRegistros(string fechaOperacion, int codigoRutaEspeciales = 322);
        string TrasladosGenerados();
        string TrasladosGeneradosConta();
        string TrasladosPorDia(DateTime fechaOperacion, int estadoTraslado = EstadoTraslado.ANULADO);
    }
}
