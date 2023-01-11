using ApiEspeciales.Modules.Especiales2.Models;

namespace ApiEspeciales.Modules.Pedidos.Interfaces
{
    internal interface IPedidosStatement
    {
        string VentasAlCreditoPorFecha(string fecha, int estado = EstadoTraslado.ANULADO);
    }
}
