using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ambrosia.Models
{
    public class LogEntity: TableEntity
    {
        public LogEntity() { }
        public string message { get; set; }
    }
}
