using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Company.Function
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
        public int status {get; set; }
    }
}
