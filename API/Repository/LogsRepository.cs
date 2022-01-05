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
    public class LogsRepository:ILogsRepository
    {
        private string _connectionString;
        private static CloudTableClient _tableClient;
        private static CloudTable _logTable;

        public LogsRepository(IConfiguration configuration)
        {
            Task.Run(async () => { await InitializeTable(configuration); }).GetAwaiter().GetResult();

        }
        
        public async Task InitializeTable(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue(typeof(string), "AzureStorageConnectionString").ToString();
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _logTable = _tableClient.GetTableReference("Logs");
            await _logTable.CreateIfNotExistsAsync();
        }

        public async Task<List<LogEntity>> GetAllLogs()
        {
            var logs = new List<LogEntity>();
            TableQuery<LogEntity> query = new TableQuery<LogEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<LogEntity> resultSegment = await _logTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                logs.AddRange(resultSegment);
            } while (token != null);
            return logs;
        }
    }
}
