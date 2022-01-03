using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.Command
{
    public class Command : TableEntity
    {
       
        public int type {get; set; }
    }
}