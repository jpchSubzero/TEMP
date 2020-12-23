using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LeerMensajesActiveMQ.Services
{
    public class UploadFile : IuploadFile
    {
        public async Task<HttpResponseMessage> Upload(Dictionary<string, string> parameters, byte[] file, string fileName)
        {
            using (var client = new HttpClient())
            {

                using (var dataForm = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    dataForm.Add(new StreamContent(new MemoryStream(file)), "bilddatei", fileName);

                    foreach (var item in parameters)
                    {
                        dataForm.Add(new StringContent(item.Value), item.Key);
                    }

                    string requestUri = ConfigurationSettings.AppSettings.Get("URL_API_PASARELA");

                    using (var message = await client.PostAsync(requestUri, dataForm))
                    {
                        return message;
                    }
                }
            }
        }
    }
}
