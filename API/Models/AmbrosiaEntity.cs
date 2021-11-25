using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ambrosia.Models
{
    public class AmbrosiaEntity: TableEntity
    {
        public AmbrosiaEntity(string IMEI)
        {
            
            this.RowKey = IMEI;
        }
        public AmbrosiaEntity() { }
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        

    }
}
