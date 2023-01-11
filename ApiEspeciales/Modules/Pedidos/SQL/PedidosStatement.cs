using ApiEspeciales.Modules.Especiales2.Models;
using ApiEspeciales.Modules.Pedidos.Interfaces;

namespace ApiEspeciales.Modules.Pedidos.SQL
{
    public class PedidosStatement : IPedidosStatement
    {
        public string VentasAlCreditoPorFecha(string fecha, int estado = EstadoTraslado.ANULADO) => $@"
            SELECT empresa,
	           serie,
	           pedido, 
               DATE_FORMAT(fecha,'%d/%m/%Y') AS fecha_pedido,
	           monto, 
               vale,
               qsys_codigo_cliente AS codigo_cliente,
               nombre AS nombre_cliente,
               factura_serie,
               factura,
               qsys_pedido,
               observaciones,
               1 AS permiso_select
            FROM inf_pedido 
            WHERE credito = 1
                AND fecha = '{fecha}'
                AND estado <> {estado}                        
        ";
    }
}
