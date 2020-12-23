using Apache.NMS;
using Apache.NMS.Util;
using LeerMensajesActiveMQ.Remote_Services.ActiveMQ;
using LeerMensajesActiveMQ.Services;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeerMensajesActiveMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Esperando mensajes... ");

            IActiveMQ activeMqCliente = new ActiveMQClient();

            // Read all messages off the queue
            while (true)
            {
                activeMqCliente.ReadMessageQueue();

            }

            //var client = new RestClient("http://localhost:23224/api/webhooks/incoming/custom");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("ms-signature", "sha256 =4ebdde0e15a3490e4ed25225156a002caa3cb1519563425fd11d4df81240c215");
            //request.AddHeader("Content-Type", "application/json");
            ////request.AddHeader("Cookie", "__RequestVerificationToken=CHd9eSq5fx2FSdsRc3sCByUSHQW3-tiAqtZ-c94U9GKUoItjZAt1SLrn4GDVka9NrFb1KFbIwCMavyFtpjg_nDE8hqDGhVgTKCViJXycrlI1");
            //request.AddParameter("application/json", "{\r\n    \"data1\": \"Valor dato 1\",\r\n    \"data2\": \"Valor dato 2\"\r\n}", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);
            //Console.ReadLine();
        }
    }
}
