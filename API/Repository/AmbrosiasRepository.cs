using Ambrosia.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Azure.Storage.Queues;
namespace Ambrosia.Repository
{
    public class AmbrosiasRepository:IAmbrosiasRepository
    {
        private string _connectionString;
        private static CloudTableClient _tableClient;
        private static CloudTable _ambrosiaTable;

        public async Task CreateAmbrosia(AmbrosiaEntity ambrosia)
        {
            //var insertOperation = TableOperation.Insert(student);
            //await _studentsTable.ExecuteAsync(insertOperation);
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);
            var jObj = JObject.Parse(jsonAmbrosia);
            jObj["operation"] = 0;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jObj.ToString(Formatting.Indented));
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);
            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String); 
        }
        public AmbrosiasRepository(IConfiguration configuration)
        {
            Task.Run(async () => { await InitializeTable(configuration); }).GetAwaiter().GetResult();

        }
        
        public async Task InitializeTable(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue(typeof(string), "AzureStorageConnectionString").ToString();
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _ambrosiaTable = _tableClient.GetTableReference("Ambrosia");
            await _ambrosiaTable.CreateIfNotExistsAsync();
        }

        public async Task<List<AmbrosiaEntity>> GetAllAmbrosias()
        {
            var ambrosias = new List<AmbrosiaEntity>();
            TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<AmbrosiaEntity> resultSegment = await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                ambrosias.AddRange(resultSegment);
            } while (token != null);
            return ambrosias;
        }
        public async Task UpdateAmbrosia(JObject ambrosia)
        {
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);
            var jObj = JObject.Parse(jsonAmbrosia);
            jObj["operation"] = 1;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jObj.ToString(Formatting.Indented));
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);
            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String); 
            /*TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, ambrosia["RowKey"].ToString()), TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ambrosia["PartitionKey"].ToString())));
            TableContinuationToken token = null;
            TableQuerySegment<AmbrosiaEntity> resultSegment = await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);
            token = resultSegment.ContinuationToken;
            switch(ambrosia["Field"].ToString())
            {
                case "last_accesed":    resultSegment.Results[0].last_accessed = DateTime.Now;
                                        break;
                case "status":          resultSegment.Results[0].status = ambrosia["Value"].ToObject<int>();
                                        break;

            }
            var updateOperation = TableOperation.Replace(resultSegment.Results[0]);
            await _ambrosiaTable.ExecuteAsync(updateOperation);*/
        }
        public async Task DeleteAmbrosia(JObject ambrosia)
        {
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);
            var jObj = JObject.Parse(jsonAmbrosia);
            jObj["operation"] = 2;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jObj.ToString(Formatting.Indented));
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);
            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String);
            /*TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, ambrosia["RowKey"].ToString()), TableOperators.And, TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, ambrosia["PartitionKey"].ToString())));
            TableContinuationToken token = null;
            TableQuerySegment<AmbrosiaEntity> resultSegment = await _ambrosiaTable.ExecuteQuerySegmentedAsync(query, token);
            token = resultSegment.ContinuationToken;
            var deleteOperation = TableOperation.Delete(resultSegment.Results[0]);
            await _ambrosiaTable.ExecuteAsync(deleteOperation);*/
        }
    }
}
