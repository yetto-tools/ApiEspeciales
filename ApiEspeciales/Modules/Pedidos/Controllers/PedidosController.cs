using ApiEspeciales.Modules.Especiales2.Controllers;
using ApiEspeciales.Modules.Pedidos.Interfaces;
using ApiEspeciales.Modules.Pedidos.SQL;
using ApiEspeciales.Services.AppDbConext;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ApiEspeciales.Modules.Pedidos.Controllers
{
    [EnableCors("CORS")]
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase, IPedidosActionService
    {
        private readonly ILogger<Especiales2Controller> _logger;
        private readonly OdbcContext _odbc;
        public readonly PedidosStatement stmt = new();

        public PedidosController(ILogger<Especiales2Controller> logger, OdbcContext odbc)
        {
            _logger = logger;
            _odbc = odbc;
        }

        [HttpGet("ventascredito")]
        public async Task<IActionResult> VentasCreditoAsync([FromQuery, Required] DateTime fecha)
        {
            using var db = _odbc.CreateConnection();
            try
            {
                var DataSetTransfers = await db.QueryAsync(
                    sql: stmt.VentasAlCreditoPorFecha(fecha: fecha.ToString("yyyy/MM/dd"))
                );

                if (DataSetTransfers == null) return NotFound();
                if (!DataSetTransfers.Any()) return NoContent();

                return Ok(DataSetTransfers);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{}] {} ", DateTime.UtcNow, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }

        }
    }
}
