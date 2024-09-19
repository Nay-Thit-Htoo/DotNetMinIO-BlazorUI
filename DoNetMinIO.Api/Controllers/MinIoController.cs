using DoNetMinIO.Api.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoNetMinIO.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinIoController : ControllerBase
    {
        private readonly IMinIoService _minIoService;
        public MinIoController(IMinIoService minIoService)
        {
            _minIoService = minIoService;
        }


        [HttpGet("TestMinioConnectionAsync")]
        public async Task<IActionResult> TestMinioConnectionAsync()
        {
            return Ok(await _minIoService.TestMinioConnectionAsync());
        }       

    }
}
