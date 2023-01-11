using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ApiEspeciales.Modules.Pedidos.Interfaces
{
    public interface IPedidosActionService
    {
        Task<IActionResult> VentasCreditoAsync([FromQuery, Required] DateTime fecha);
    }
}
