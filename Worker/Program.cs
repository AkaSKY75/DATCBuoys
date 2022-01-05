using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Worker
{
    class Program
    {


        static void Main(string[] args)
        {

            RestClient client;
            RestRequest request;
            IRestResponse response;
            request = new RestRequest(Method.GET);
            List<AmbrosiaEntity> ambrosias;
            client = new RestClient("https://datcbuoys-webapi.azurewebsites.net/Ambrosia");
            client.Timeout = -1;

            response = client.Execute(request);
            ambrosias = JsonConvert.DeserializeObject<List<AmbrosiaEntity>>(response.Content);
            foreach (var zone in ambrosias)
            {
                if((DateTime.Now - zone.last_accessed).TotalMinutes >= 10)
                {
                    request = new RestRequest(Method.DELETE);
                    request.AddJsonBody(new { name = zone.name, RowKey = zone.RowKey, PartitionKey = zone.PartitionKey, administrator = 0 });
                    response = client.Execute(request);
                }
            }
        }
    }
}
