using ApiEspeciales.Modules.Especiales2.Interfaces;
using ApiEspeciales.Modules.Especiales2.Models;

namespace ApiEspeciales.Modules.Especiales2.SQL
{
    public class Especiales2Statement : IEspecieles2Statement
    {
        public string CambiarEstado(int codigoTraslado, int estadoTraslado)
        {
            return $@"
                UPDATE inf_traslado_especiales2
                    SET codigo_estado = {estadoTraslado}
                WHERE codigo_traslado = {codigoTraslado}
                UPDATE inf_traslado_especiales2
                    SET codigo_estado = {estadoTraslado}
                WHERE codigo_traslado = {codigoTraslado}
             ";
        }

        public string DetalleTraslado(int codigoTraslado)
        {

            return $@"
                SELECT
                    empresa,
		            serie,
                    pedido,
                    codigo_traslado,
                    codigo_cliente,
                    nombre_cliente,
                    nombre_cliente_depurado,
                    monto,
                    fecha_grabado
                FROM 
                    inf_traslado_detalle_especiales2
                WHERE codigo_traslado = {codigoTraslado}
                  AND estado = {EstadoTraslado.GENERADO}
            ";
        }

        public string DetalleTrasladoEdicion(int codigoTraslado)
        {
            return $@"
                SELECT
                    empresa,
		            serie,
                    pedido,
                    codigo_traslado,
                    codigo_cliente,
                    nombre_cliente,
                    nombre_cliente_depurado,
                    monto,
                    fecha_grabado,
                    1 AS permiso_anular
                FROM 
                    inf_traslado_detalle_especiales2
                WHERE codigo_traslado = {codigoTraslado}
                  AND estado = {EstadoTraslado.GENERADO}
                ORDER BY fecha_grabado DESC
            ";
        }

        public string EliminarDetalleTraslado(string codigoEmpresa, string codigoSerie, string codigoPedido)
        {
            return $@"
                DELETE 
                    FROM inf_traslado_detalle_especiales2 
                WHERE empresa = '{codigoEmpresa}' 
                    AND serie = '{codigoSerie}' 
                    AND pedido = {codigoPedido}
            ";
        }

        public Dictionary<string, string> EliminarTraslado(int codigoTraslado)
        {
            Dictionary<string, string> stmtDelete = new()
            {
                { "eliminarDetalle", $"DELETE FROM inf_traslado_detalle_especiales2 WHERE codigo_traslado = {codigoTraslado}" },
                { "eliminarEncabezado", $"DELETE FROM inf_traslado_especiales2 WHERE codigo_traslado = {codigoTraslado}" }
            };

            return stmtDelete;
        }

        public string Generar(DateTime fechaOperacion, string usuario, int codigoRutaEspeciales = 332)
        {
            throw new NotImplementedException();
        }

        public string Importaciones()
        {
            throw new NotImplementedException();
        }

        public string ImportacionPorfecha(string fechaOperacion)
        {
            throw new NotImplementedException();
        }

        public string InsertDetalle(int codigoTraslado, string usuario, string fechaOperacionOperacion, int codigoRutaEspeciales = 332)
        {
            throw new NotImplementedException();
        }

        public string InsertEncabezado()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ModicacionDetalleTraslado(int codigoTraslado, string usuario, int codigoRutaEspeciales = 332)
        {

            Dictionary<string, string> stmtModificarTraslado = new()
            {
                { "InsertarHistorialModiciaciones", @$"
                    INSERT INTO inf_traslado_especiales2_hist(
                        empresa,
                        serie,
                        pedido,
                        monto_modificado,
                        monto_correcto,
                        descripcion,
                        estado,
                        usuario_ing,
                        fecha_ing
                    )
                
                    SELECT x.empresa, 
	                    x.serie, 
	                    x.pedido,
	                    x.monto AS monto_modificado,
	                    cast(y.monto as decimal(18,2)) AS monto_correcto,
                        CASE 
                            WHEN y.estado = 9 THEN 'Pedido anulado'
                            ELSE 'Monto modificado'
                        END AS descripion,
	                    {EstadoTraslado.GENERADO} AS estado,
	                    '{usuario}' AS usuario_ing,
	                    NOW() AS fecha_ing
                    FROM inf_traslado_detalle_especiales2 x
                        INNER JOIN inf_pedido y
                            ON x.empresa = y.empresa AND x.serie = y.serie AND x.pedido = y.pedido
                    WHERE x.codigo_traslado = {codigoTraslado}
                        AND x.estado = 1
                        AND ((x.monto - cast(y.monto as decimal(18,2))) <> 0 
                        OR y.estado = 9 
                        OR y.qsys_vendedor <> {codigoRutaEspeciales} 
                        OR y.credito <> 0)
                "},
                { "ActualizarCambios", $@"
                    UPDATE inf_traslado_detalle_especiales2 a
                    INNER JOIN ( 
                        SELECT 
                            x.empresa, 
			 	            x.serie, 
				            x.pedido, 
					        cast(y.monto as decimal(18,2)) AS monto_actualizado,
                            CASE
                                WHEN (y.estado = 9 OR y.qsys_vendedor <> {codigoRutaEspeciales} OR y.credito <> 0)
                                THEN {EstadoTraslado.ANULADO}
                                ELSE {EstadoTraslado.GENERADO}
                            END AS codigo_estado
                        FROM inf_traslado_detalle_especiales2 x
                            INNER JOIN inf_pedido y
                            ON x.empresa = y.empresa AND x.serie = y.serie AND x.pedido = y.pedido
                        WHERE x.codigo_traslado = {codigoTraslado}
                        AND x.estado = {EstadoTraslado.GENERADO}
                        AND (
                            (x.monto - cast(y.monto as decimal(18,2))) <> 0 
                                OR y.estado = 9 
                                    OR y.qsys_vendedor <> {codigoRutaEspeciales}
                                        OR y.credito <> 0
                            )
		                ) b 
                    ON a.empresa = b.empresa AND a.serie = b.serie AND a.pedido = b.pedido
                    SET a.monto = b.monto_actualizado, a.estado = b.codigo_estado
                "}
            };

            return stmtModificarTraslado;
        }

        public string NumeroRegistros(string fechaOperacion, int codigoRutaEspeciales = 322)
        {
            throw new NotImplementedException();
        }

        public string TrasladosGenerados()
        {
            throw new NotImplementedException();
        }

        public string TrasladosGeneradosConta()
        {
            throw new NotImplementedException();
        }

        public string TrasladosPorDia(DateTime fechaOperacion, int estadoTraslado = EstadoTraslado.ANULADO)
        {
            var formatDate = "s";

            //throw new NotImplementedException();
            return $@"
                SELECT
                    x.codigo_traslado,
                    DATE_FORMAT(x.fecha_operacion, '%d/%m/%Y') AS fecha_operacion,
                    x.fecha_traslado,
                    IFNULL((
                            SELECT
                                count(*)
                            FROM
                                inf_traslado_detalle_especiales2
                            WHERE
                                estado = 1
                                AND codigo_traslado = x.codigo_traslado
                            GROUP BY
                                codigo_traslado
                        ), 0 ) AS numero_pedidos,
                    IFNULL((
                            SELECT
                                sum(monto)
                            FROM
                                inf_traslado_detalle_especiales2
                            WHERE
                                estado = 1
                                AND codigo_traslado = x.codigo_traslado
                            GROUP BY
                                codigo_traslado
                        ), 0 ) AS monto_total,
                    x.observaciones_traslado,
                    x.codigo_estado,
                    y.nombre AS estado,
                    x.usuario_ing,
                    DATE_FORMAT(x.fecha_ing, '%d/%m/%Y %H:%i:%s') AS fecha_ing,
                    1 AS permiso_imprimir
                FROM
                    inf_traslado_especiales2 x
                    INNER JOIN inf_estado_traslado_especiales2 y 
                        ON x.codigo_estado = y.codigo_estado_traslado
                WHERE 
                    x.codigo_estado <> {estadoTraslado}
                    AND x.fecha_operacion = '{fechaOperacion.ToString(formatDate)}'
            ";
        }
    }
}
