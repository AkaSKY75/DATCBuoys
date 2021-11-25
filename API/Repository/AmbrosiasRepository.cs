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
        
        public async Task CreateAmbrosia(AmbrosiaEntity ambrosia)
        {
            //var insertOperation = TableOperation.Insert(student);
            //await _studentsTable.ExecuteAsync(insertOperation);
            var jsonAmbrosia = JsonConvert.SerializeObject(ambrosia);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonAmbrosia);
            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "ambrosia-add-queue"
                );
            queueClient.CreateIfNotExists();

            await queueClient.SendMessageAsync(base64String); 
        }
        public AmbrosiasRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue(typeof(string), "AzureStorageConnectionString").ToString();
        }
        
        

        /*public  async Task<List<AmbrosiaEntity>> GetAllAmbrosias()
        {
            var ambrosias = new List<AmbrosiaEntity>();
            TableQuery<AmbrosiaEntity> query = new TableQuery<AmbrosiaEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<AmbrosiaEntity> resultSegment = await _ambrosiasTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                ambrosias.AddRange(resultSegment);
            } while (token != null);
            return ambrosias;
        }*/
    }
}
