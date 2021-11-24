using Microsoft.Azure.Cosmos.Table;
using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace Company.Function
{
    public static class HandleAmbrosia
    {
        private static CloudTableClient _tableClient;
        private static CloudTable _ambrosiaTable;
        private static string _connectionString;
        [Function("HandleAmbrosia")]
        public static void Run([QueueTrigger("ambrosia-add-queue", Connection = "datcbuoys_STORAGE")] string myQueueItem,
            FunctionContext context)
        {
             _connectionString = "DefaultEndpointsProtocol=https;AccountName=datcbuoys;AccountKey=42sbB51rcIja1ogyqKlT5Yw8d0uWsZH7W5/Vo8niUm83zYg1/J0Ggf64GfPs02BDInyBI845RWZwohYEIZwiLQ==;EndpointSuffix=core.windows.net";
            Task.Run(async () =>{ await InitializeTable();}).GetAwaiter().GetResult();
            var logger = context.GetLogger("HandleAmbrosia");
            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var command = JsonConvert.DeserializeObject<AmbrosiaEntity>(myQueueItem);
            logger.LogInformation($"C# Deserialized obj : {command.XCoord}");
            TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, command.RowKey));
            TableContinuationToken token = null;
            TableQuerySegment<AmbrosiaEntity> resultSegment = Task.Run(async () =>{ return await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);}).GetAwaiter().GetResult();
            token = resultSegment.ContinuationToken;
        
            command.PartitionKey=resultSegment.Results.Count.ToString();
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
