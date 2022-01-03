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
    public class AmbrosiaController : ControllerBase
    {
        private IAmbrosiasRepository _ambrosiasRepository;
        private readonly ILogger<AmbrosiaController> _logger;

        public AmbrosiaController(IAmbrosiasRepository ambrosiasRepository)
        {
            _ambrosiasRepository = ambrosiasRepository;
        }


        [HttpGet]
        public async Task<List<AmbrosiaEntity>> Get()
        {
            return await _ambrosiasRepository.GetAllAmbrosias();
        }

        [HttpPost]
        public async Task Post([FromBody] AmbrosiaEntity ambrosia)
        {
            await _ambrosiasRepository.CreateAmbrosia(ambrosia);
        }
        
        
    }
}
