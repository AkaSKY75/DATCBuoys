using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Company.Function
{
    public class LogEntity: TableEntity
    {
        public LogEntity() { }
        public string message { get; set; }
    }
}
