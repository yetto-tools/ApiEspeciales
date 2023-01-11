using System.Data;
using System.Data.Odbc;

namespace ApiEspeciales.Services.AppDbConext
{
    public class OdbcContext
    {
        private readonly IConfiguration _config;
        private readonly string _odbString = string.Empty;
        private readonly IHostEnvironment _env;
        private readonly ILogger<OdbcContext> _logger;

        public OdbcContext(IConfiguration config, IHostEnvironment env, ILogger<OdbcContext> logger)
        {
            _config = config;
            _env = env;
            _logger = logger;

            var Connection = string.Empty;

            try
            {
                if (_env.IsDevelopment()) Connection = "Development CROM";
                else if (_env.IsProduction()) Connection = "Produccion CROM";

                _odbString = _config.GetConnectionString(Connection);

                _logger.LogInformation("[{}] Conection Enviroment: {}", DateTime.UtcNow, Connection);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("[{}] {}", DateTime.UtcNow, ex.Message);
            }
        }
        public IDbConnection CreateConnection() => new OdbcConnection(_odbString);

        public Dictionary<string, string> SettingConnection()
        {
            var settings = _odbString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0].Trim(), split => split[1].Trim());
            return settings;
        }

    }
}
