using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeerMensajesActiveMQ.Services
{
    public interface IuploadFile
    {
        Task<HttpResponseMessage> Upload(Dictionary<string, string> parameters, byte[] file, string fileName);
    }
}