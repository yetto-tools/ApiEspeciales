using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ApiEspeciales.Modules.Especiales2.Interfaces
{
    internal interface IEspeciales2ActionService
    {
        Task<IActionResult> CambiarEstadoAsync([FromForm, Required] int codigo, [FromForm, Required] int estado);
        Task<IActionResult> ConsultaTrasladosPorDiaAsync([FromQuery, Required] DateTime fecha);
        Task<IActionResult> DetalleTrasladoAsync([FromQuery, Required] int codigo);
        Task<IActionResult> DetalleTrasladoEdicionAsync([FromQuery, Required] int codigo);
        Task<IActionResult> EliminarDetalleTrasladoAsync([FromRoute, Required] string codigo);
        Task<IActionResult> EliminarTrasladoAsync([FromRoute, Required] int codigo);
        Task<IActionResult> GenerarAsync([FromForm, Required] string fecha, [FromForm, Required] string usuario);
        Task<IActionResult> ImportacionesAsync();
        Task<IActionResult> ImportacionesPorFechaAsync([FromHeader, Required] DateTime fecha);
        Task<IActionResult> ModificacionDetalleTrasladadoAsync([FromForm, Required] int codigo, [FromForm, Required] string usuario);
        Task<IActionResult> TrasladosGeneradosAsync();
        Task<IActionResult> TrasladosGeneradosContaAsync();
    }
}