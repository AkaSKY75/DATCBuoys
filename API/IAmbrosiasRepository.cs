using Ambrosia.Models;
using Ambrosia.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ambrosia
{
    public interface IAmbrosiasRepository
    {
        //Task<List<AmbrosiaEntity>> GetAllAmbrosias();
        Task CreateAmbrosia(AmbrosiaEntity ambrosia);
       
        
    }
}
