using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Worker
{
    public class AmbrosiaEntity : TableEntity
    {
        public AmbrosiaEntity(string IMEI)
        {

            this.RowKey = IMEI;
        }
        public AmbrosiaEntity() { }
        public string name { get; set; }
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public int status { get; set; }
        public double meters { get; set; }
        public DateTime last_accessed { get; set; }
    }
}
