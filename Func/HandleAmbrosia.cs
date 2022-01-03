using Microsoft.Azure.Cosmos.Table;
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
        private static string _connectionString;
        [Function("HandleAmbrosia")]
        public static void Run([QueueTrigger("ambrosia-add-queue", Connection = "laborator4danmircea_STORAGE")] string myQueueItem,
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
            await _ambrosiaTable.CreateIfNotExistsAsync();
        }
    }
    
}
