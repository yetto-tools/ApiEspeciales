using ApiEspeciales.Services.AppDbConext;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ApiEspeciales.Modules.About.Controllers
{
    [EnableCors("CORS")]
    [Route("/")]
    [ApiController]
    public class AboutController : ControllerBase
    {
        private readonly ILogger<AboutController> _logger;
        private readonly OdbcContext _odbc;
        private readonly IHostEnvironment _hostingEnvironment;
        public ILogger<AboutController> Logger => _logger;

        public AboutController(ILogger<AboutController> logger, OdbcContext odbc, IHostEnvironment hostEnv)
        {
            _logger = logger;
            _odbc = odbc;
            _hostingEnvironment = hostEnv;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var info = _odbc.SettingConnection();
            var result = new
            {
                message = "Welcome To API's Service",
                version = "1.2",
                Environment = _hostingEnvironment.EnvironmentName,
                odbcSettings = new
                {
                    driver = info["driver"],
                    server = info["server"],
                    dataBase = info["database"],
                    charset = info["charset"]
                }
            };

            Logger.LogInformation("[{}] Information Server Connection: \"{}\" ", DateTime.UtcNow, result);

            return Ok(result);
        }

    }
}
