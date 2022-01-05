using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ambrosia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Ambrosia.Repository;
using Newtonsoft.Json.Linq;

namespace Ambrosia.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private ILogsRepository _logRepository;
        private readonly ILogger<AmbrosiaController> _logger;

        public LogsController(ILogsRepository logRepository)
        {
            _logRepository = logRepository;
        }


        [HttpGet]
        public async Task<List<LogEntity>> Get()
        {
            return await _logRepository.GetAllLogs();
        }
    }
}
