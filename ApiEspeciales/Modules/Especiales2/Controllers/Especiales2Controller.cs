using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ApiEspeciales.Modules.Especiales2.Interfaces;
using ApiEspeciales.Modules.Especiales2.Models;
using ApiEspeciales.Modules.Especiales2.SQL;
using ApiEspeciales.Services.AppDbConext;


namespace ApiEspeciales.Modules.Especiales2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class Especiales2Controller : ControllerBase, IEspeciales2ActionService
    {

        private readonly ILogger<Especiales2Controller> _logger;
        private readonly OdbcContext _odbc;
        public readonly Especiales2Statement stmt = new();

        public Especiales2Controller(ILogger<Especiales2Controller> logger, OdbcContext odbc)
        {
            _logger = logger;
            _odbc = odbc;
        }

        [HttpPut("cambiarestado")]
        public async Task<IActionResult> CambiarEstadoAsync([FromForm, Required] int codigo, [FromForm, Required] int estado)
        {
            using var db = _odbc.CreateConnection();
            try
            {
                var rows = await db.ExecuteAsync(
                    sql: stmt.CambiarEstado(codigo, estado)
                );

                if (rows == 0) return StatusCode(304, new { affectedRows = rows });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(" [{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [HttpGet("consultatrasladospordia")]
        public async Task<IActionResult> ConsultaTrasladosPorDiaAsync([FromQuery, Required] DateTime fecha)
        {
            using var db = _odbc.CreateConnection();
            try
            {
                var DataAccessObject = await db.QueryAsync(
                    sql: stmt.TrasladosPorDia(
                        fechaOperacion: fecha,
                        estadoTraslado: EstadoTraslado.ANULADO
                    )
                );

                if (DataAccessObject == null) return NotFound();
                if (!DataAccessObject.Any()) return NoContent();

                return Ok(DataAccessObject);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [HttpGet("detalletraslado")]
        public async Task<IActionResult> DetalleTrasladoAsync([FromQuery, Required] int codigo)
        {
            using var db = _odbc.CreateConnection();
            try
            {
                var DataAccessObject = await db.QueryAsync(
                    sql: stmt.DetalleTraslado(
                       codigoTraslado: codigo
                    )
                );

                if (DataAccessObject == null) return NotFound();
                if (!DataAccessObject.Any()) return NoContent();

                return Ok(DataAccessObject);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("detalletrasladoedicion")]
        public async Task<IActionResult> DetalleTrasladoEdicionAsync([FromQuery, Required] int codigo)
        {
            using var db = _odbc.CreateConnection();
            try
            {
                var DataAccessObject = await db.QueryAsync(
                    sql: stmt.DetalleTrasladoEdicion(
                        codigoTraslado: codigo
                    )
                );

                if (DataAccessObject == null) return NotFound();
                if (!DataAccessObject.Any()) return NoContent();

                return Ok(DataAccessObject);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("eliminardetalletraslado/{codigo}")]
        public async Task<IActionResult> EliminarDetalleTrasladoAsync([FromRoute, Required] string codigo)
        {
            using var db = _odbc.CreateConnection();
            using var trans = db.BeginTransaction();

            try
            {
                var param = codigo.Split(',');

                var eliminarDetalleTraslado =
                    await db.ExecuteAsync(
                        sql: stmt.EliminarDetalleTraslado(codigoEmpresa: param[0], codigoSerie: param[1], codigoPedido: param[2])
                    );

                if (eliminarDetalleTraslado == 0)
                {
                    return StatusCode(304, new { affectedRows = new { eliminarDetalleTraslado } });
                }

                return Ok(eliminarDetalleTraslado);
            }
            catch (Exception ex)
            {
                trans.Rollback();

                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);

                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("eliminartraslado/{codigo}")]
        public async Task<IActionResult> EliminarTrasladoAsync([FromRoute, Required] int codigo)
        {

            using var db = _odbc.CreateConnection();
            using var Trans = db.BeginTransaction();

            try
            {
                string? eliminarDetalle = string.Empty;
                string? eliminarEncabezado = string.Empty;

                stmt.EliminarTraslado(codigo).TryGetValue("eliminarDetalle", out eliminarDetalle);
                stmt.EliminarTraslado(codigo).TryGetValue("eliminarEncabezado", out eliminarEncabezado);

                var eliminarDetalleTraslado = await db.ExecuteAsync(
                    sql: eliminarDetalle,
                    transaction: Trans
                );

                Trans.Commit();

                var eliminarTraslado = await db.ExecuteAsync(
                    sql: eliminarEncabezado,
                    transaction: Trans
                );
                Trans.Commit();

                if (eliminarTraslado == 0 || eliminarDetalleTraslado == 0)
                {
                    return StatusCode(304,
                        new
                        {
                            affectedRows = new
                            {
                                eliminarTraslado,
                                eliminarDetalleTraslado
                            }
                        });
                }

                return Ok(eliminarTraslado);
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [HttpPost("generar")]
        public Task<IActionResult> GenerarAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet("importaciones")]
        public async Task<IActionResult> ImportacionesAsync()
        {
            using var db = _odbc.CreateConnection();
            var DataAccessObject = await db.QueryAsync(sql: stmt.Importaciones());

            return Ok(DataAccessObject);
        }

        [HttpGet("importacionesporfecha")]
        public async Task<IActionResult> ImportacionesPorFechaAsync([FromHeader, Required] DateTime fecha)
        {
            using var db = _odbc.CreateConnection();
            var DataAccessObject = await db.QueryAsync(
                sql: stmt.ImportacionPorfecha(
                    fechaOperacion: fecha.ToString("yyyy-MM-dd"))
                );
            return Ok(DataAccessObject);

        }

        [HttpPost("modificaciondetalletrasladado")]
        public Task<IActionResult> ModificacionDetalleTrasladadoAsync([FromForm, Required] int codigo, [FromForm, Required] string usuario)
        {
            throw new NotImplementedException();
        }

        [HttpGet("trasladosgenerados")]
        public async Task<IActionResult> TrasladosGeneradosAsync()
        {
            using var db = _odbc.CreateConnection();
            try
            {
                var DataAccessObject =
                    await db.QueryAsync(
                        stmt.TrasladosGenerados()
                    );
                if (DataAccessObject == null) return NotFound();
                if (!DataAccessObject.Any()) return NoContent();

                return Ok(DataAccessObject);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("trasladosgeneradosconta")]
        public async Task<IActionResult> TrasladosGeneradosContaAsync()
        {
            using var db = _odbc.CreateConnection();

            var DataAccessObject = await db.QueryAsync(
                stmt.TrasladosGeneradosConta()
               );

            return Ok(DataAccessObject);
        }

    }
}
