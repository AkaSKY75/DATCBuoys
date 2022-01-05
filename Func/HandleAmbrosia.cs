using Microsoft.Azure.Cosmos.Table;
using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace Company.Function
{
    public static class HandleAmbrosia
    {
        private static CloudTableClient _tableClient;
        private static CloudTable _ambrosiaTable;
        private static CloudTable _logTable;
        private static CloudTable _administratorTable;

        private static string _connectionString;
        [Function("HandleAmbrosia")]
        public static void Run([QueueTrigger("ambrosia-queue", Connection = "laborator4danmircea_STORAGE")] string myQueueItem,
            FunctionContext context)
        {
            TableQuery<AmbrosiaEntity> query;
            TableContinuationToken token;
            TableQuerySegment<AmbrosiaEntity> resultSegment;
            TableQuery<LogEntity> query_logger;
            TableQuerySegment<LogEntity> resultSegment_logger;
            TableOperation insertOperation_logger;
            LogEntity log = null;

             _connectionString = "DefaultEndpointsProtocol=https;AccountName=laborator4danmircea;AccountKey=cW8vQ+vW9S3iELU5W8zFzJgLHXUh0Y7HvMWxGYQcTr1sPVKl7pBB9/88QZbqcnzsvOpE8RxBbh9abO8yK5KLDg==;EndpointSuffix=core.windows.net";
            Task.Run(async () =>{ await InitializeTable();}).GetAwaiter().GetResult();
            var logger = context.GetLogger("HandleAmbrosia");
            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var command = JsonConvert.DeserializeObject<JObject>(myQueueItem);


            /*logger.LogInformation($"C# XCoord : {command.XCoord}\n YCoord: {command.YCoord}");
            query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, command.RowKey));
            token = null;
            resultSegment = Task.Run(async () =>{ return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);}).GetAwaiter().GetResult();
            token = resultSegment.ContinuationToken;
            if(resultSegment.Results.Count > 0)
                command.PartitionKey = (resultSegment.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString();
            else
                command.PartitionKey = "0";
            var insertOperation = TableOperation.Insert(command);
            Task.Run(async () =>{  await _ambrosiaTable.ExecuteAsync(insertOperation);}).GetAwaiter().GetResult();*/


            switch( command["operation"].ToObject<int>())
            {
                case 0:  AmbrosiaEntity ambrosia = new AmbrosiaEntity(){
                                    name = command["name"].ToObject<string>(),
                                    RowKey = command["RowKey"].ToObject<string>(),
                                    XCoord = command["XCoord"].ToObject<double>(),
                                    YCoord = command["YCoord"].ToObject<double>(),
                                    status = command["status"].ToObject<int>(),
                                    meters = command["meters"].ToObject<double>(),
                                    last_accessed = command["last_accessed"].ToObject<DateTime>()
                                    //last_accessed = DateTime.Now
                                };
                                logger.LogInformation($"C# XCoord : {ambrosia.XCoord}\n YCoord: {ambrosia.YCoord}");
                                query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, ambrosia.RowKey));
                                token = null;
                                resultSegment = Task.Run(async () =>{ return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);}).GetAwaiter().GetResult();
                                token = resultSegment.ContinuationToken;
                                if(resultSegment.Results.Count > 0)
                                    ambrosia.PartitionKey = (resultSegment.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString();
                                else
                                    ambrosia.PartitionKey = "0";
                                var insertOperation = TableOperation.Insert(ambrosia);
                                Task.Run(async () =>{  await _ambrosiaTable.ExecuteAsync(insertOperation);}).GetAwaiter().GetResult();
                                
                                //Logger
                                query_logger = new TableQuery<LogEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "0"));
                                token = null;
                                resultSegment_logger = Task.Run(async () =>{ return await _logTable.ExecuteQuerySegmentedAsync(query_logger, token);}).GetAwaiter().GetResult();
                                token = resultSegment_logger.ContinuationToken;
                                if(resultSegment_logger.Results.Count == 0)
                                    log = new LogEntity(){ RowKey = "0", PartitionKey = "0", message = "User with phone name " + ambrosia.name + " and IMEI " + ambrosia.RowKey + " added zone with id #" + ambrosia.PartitionKey + " and coordinates " + ambrosia.XCoord +  " latitude and " + ambrosia.YCoord + " longitude."};
                                else
                                    log = new LogEntity(){ RowKey = "0", PartitionKey = (resultSegment_logger.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString(), message = "User with phone name " + ambrosia.name + " and IMEI " + ambrosia.RowKey + " added zone with id #" + ambrosia.PartitionKey + " and coordinates " + ambrosia.XCoord +  " latitude and " + ambrosia.YCoord + " longitude."};

                                insertOperation_logger = TableOperation.Insert(log);
                                Task.Run(async () =>{  await _logTable.ExecuteAsync(insertOperation_logger);}).GetAwaiter().GetResult();

                                break;
                case 1:         query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, command["RowKey"].ToString()), TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, command["PartitionKey"].ToString())));
                                token = null;
                                resultSegment = Task.Run(async () => { return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token); }).GetAwaiter().GetResult();
                                token = resultSegment.ContinuationToken;
                                switch(command["field"].ToString())
                                {
                                    case "last_accessed":    resultSegment.Results[0].last_accessed = command["value"].ToObject<DateTime>();
                                                            break;
                                    case "status":          resultSegment.Results[0].status = command["value"].ToObject<int>();
                                                            break;

                                }
                                if(resultSegment.Results.Count == 1)
                                {
                                    var updateOperation = TableOperation.Replace(resultSegment.Results[0]);
                                    Task.Run(async () =>{  await _ambrosiaTable.ExecuteAsync(updateOperation);}).GetAwaiter().GetResult();
                                }
                                
                                //Logger
                                query_logger = new TableQuery<LogEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "1"));
                                token = null;
                                resultSegment_logger = Task.Run(async () =>{ return await _logTable.ExecuteQuerySegmentedAsync(query_logger, token);}).GetAwaiter().GetResult();
                                token = resultSegment_logger.ContinuationToken;
                                if(command["administrator"].ToObject<int>() == -1)
                                {
                                    if(command["type"].ToObject<int>() == 0)
                                    {
                                        if(resultSegment_logger.Results.Count == 0)
                                            log = new LogEntity(){ RowKey = "1", PartitionKey = "0", message = "User with phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " entered zone with id #" + command["PartitionKey"].ToObject<string>()};
                                        else
                                            log = new LogEntity(){ RowKey = "1", PartitionKey = (resultSegment_logger.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString(), message = "User with phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " entered zone with id #" + command["PartitionKey"].ToObject<string>()};
                                    }
                                    else if(command["type"].ToObject<int>() == 1)
                                    {
                                        if(resultSegment_logger.Results.Count == 0)
                                            log = new LogEntity(){ RowKey = "1", PartitionKey = "0", message = "User with phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " exited zone with id #" + command["PartitionKey"].ToObject<string>()};
                                        else
                                            log = new LogEntity(){ RowKey = "1", PartitionKey = (resultSegment_logger.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString(), message = "User with phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " exited zone with id #" + command["PartitionKey"].ToObject<string>()};
                                    }
                                    insertOperation_logger = TableOperation.Insert(log);
                                    Task.Run(async () =>{  await _logTable.ExecuteAsync(insertOperation_logger);}).GetAwaiter().GetResult();
                                }
                                
                                break;
                case 2:         query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, command["RowKey"].ToString()), TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, command["PartitionKey"].ToString())));
                                token = null;
                                resultSegment = Task.Run(async() => { return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token); }).GetAwaiter().GetResult();
                                token = resultSegment.ContinuationToken;
                                var deleteOperation = TableOperation.Delete(resultSegment.Results[0]);
                                Task.Run(async () => {await _ambrosiaTable.ExecuteAsync(deleteOperation);}).GetAwaiter().GetResult();
                                
                                //Logger
                                query_logger = new TableQuery<LogEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "2"));
                                token = null;
                                resultSegment_logger = Task.Run(async () =>{ return await _logTable.ExecuteQuerySegmentedAsync(query_logger, token);}).GetAwaiter().GetResult();
                                token = resultSegment_logger.ContinuationToken;
                                if(command["administrator"].ToObject<int>() == -1)
                                {
                                    if(resultSegment_logger.Results.Count == 0)
                                        log = new LogEntity(){ RowKey = "2", PartitionKey = "0", message = "User with phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " removed zone with id #" + command["PartitionKey"].ToObject<string>()};
                                    else
                                        log = new LogEntity(){ RowKey = "2", PartitionKey = (resultSegment_logger.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString(), message = "User with phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " removed zone with id #" + command["PartitionKey"].ToObject<string>()};
                                }
                                else if(command["administrator"].ToObject<int>() == 0)
                                {
                                    if(resultSegment_logger.Results.Count == 0)
                                        log = new LogEntity(){ RowKey = "2", PartitionKey = "0", message = "Zone with id #" + command["PartitionKey"].ToObject<string>() + " added by user having phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " was automatically removed due to inactivity."};
                                    else
                                        log = new LogEntity(){ RowKey = "2", PartitionKey = (resultSegment_logger.Results.Max(x => Convert.ToInt32(x.PartitionKey))+1).ToString(), message = "Zone with id #" + command["PartitionKey"].ToObject<string>() + " added by user having phone name " + command["name"].ToObject<string>() + " and IMEI " + command["RowKey"].ToObject<string>() + " was automatically removed due to inactivity."};
                                }
                                insertOperation_logger = TableOperation.Insert(log);
                                Task.Run(async () =>{  await _logTable.ExecuteAsync(insertOperation_logger);}).GetAwaiter().GetResult();
                                
                                break;
                
            }
            
        }
        private static async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _ambrosiaTable = _tableClient.GetTableReference("Ambrosia");
            _logTable = _tableClient.GetTableReference("Logs");
            _administratorTable = _tableClient.GetTableReference("Administrators");
            await _ambrosiaTable.CreateIfNotExistsAsync();
            await _logTable.CreateIfNotExistsAsync();
            await _administratorTable.CreateIfNotExistsAsync();
        }
    }
    
}
/*using Microsoft.Azure.Cosmos.Table;
using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Company.Function
{
    public static class HandleAmbrosia
    {
        private static CloudTableClient _tableClient;
        private static CloudTable _ambrosiaTable;
        private static CloudTable _logTable;
        private static CloudTable _administratorTable;

        private static string _connectionString;
        [Function("HandleAmbrosia")]
        public static void Run([QueueTrigger("ambrosia-queue", Connection = "laborator4danmircea_STORAGE")] string myQueueItem,
            FunctionContext context)
        {
             _connectionString = "DefaultEndpointsProtocol=https;AccountName=laborator4danmircea;AccountKey=cW8vQ+vW9S3iELU5W8zFzJgLHXUh0Y7HvMWxGYQcTr1sPVKl7pBB9/88QZbqcnzsvOpE8RxBbh9abO8yK5KLDg==;EndpointSuffix=core.windows.net";
            Task.Run(async () =>{ await InitializeTable();}).GetAwaiter().GetResult();
            var logger = context.GetLogger("HandleAmbrosia");
            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var command = JsonConvert.DeserializeObject<AmbrosiaEntity>(myQueueItem);
            logger.LogInformation($"C# XCoord : {command.XCoord}\n YCoord: {command.YCoord}");
            TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, command.RowKey));
            TableContinuationToken token = null;
            TableQuerySegment<AmbrosiaEntity> resultSegment = Task.Run(async () =>{ return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);}).GetAwaiter().GetResult();
            token = resultSegment.ContinuationToken;
    
            command.PartitionKey = resultSegment.Results.Count.ToString();
            var insertOperation = TableOperation.Insert(command);
            Task.Run(async () =>{  await _ambrosiaTable.ExecuteAsync(insertOperation);}).GetAwaiter().GetResult();
            
        }
        private static async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _ambrosiaTable = _tableClient.GetTableReference("Ambrosia");
            _logTable = _tableClient.GetTableReference("Logs");
            _administratorTable = _tableClient.GetTableReference("Administrators");
            await _ambrosiaTable.CreateIfNotExistsAsync();
            await _logTable.CreateIfNotExistsAsync();
            await _administratorTable.CreateIfNotExistsAsync();
        }
    }
    
}*/
