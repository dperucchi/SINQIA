using Microsoft.AspNetCore.Mvc;
using SINQIA.Models;
using SINQIA.Models.Calcular;
using SINQIA.Services;

namespace SINQIA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvestimentoController : Controller
    {
        private CalcularService _service;

        public InvestimentoController(CalcularService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<CalcularResponse> Calcular([FromBody] CalcularRequest request)
        {
            var resultado = _service.Calcular(request);
            return Ok(resultado);
        }
    }
}
