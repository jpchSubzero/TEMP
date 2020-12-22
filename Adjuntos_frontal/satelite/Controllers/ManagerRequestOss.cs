using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using RestSharp;
using Newtonsoft.Json;
using satelite.Models;
using log4net;

namespace satelite.Controllers
{
    public class ManagerRequestOss : AbsManagerRequest
    {
        /// <summary>
        /// Agrega los parametros necesarios a la solicitud para cargar el archivo
        /// </summary>
        /// <param name="request">Solicitud.</param>
        /// <param name="requestData">Datos a agregarse</param>
        /// <param name="file">Archivo a cargarse</param>
        public static void SetParametersRequest(RestRequest request, string requestData, HttpPostedFileBase file)
        {
            string[] serviceData = requestData.Split('_');
            string pidm = serviceData[0];
            string numbServ = serviceData[1];
            string libraryName = ManagerConfigOss.GetLibraryNameUploadService(),
                contentType = ManagerConfigOss.GetContentTypeUploadService();

            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("Pidm", pidm);
            metadata.Add("NumeroServicio", numbServ);

            var json = JsonConvert.SerializeObject(metadata);

            request.AddParameter("Origin", "SER");
            request.AddParameter("Metadata", json);
            //request.AddParameter("LibraryName", libraryName);
            //request.AddParameter("DocumentType", contentType);

            //request.AddParameter("Origin", "SER");
            //request.AddParameter("Metadata", "{\"pidm\": \"2815\",\"service_number\": \"21758\"}");
            //request.AddParameter("Webhook", "http://localhost:23224/api/webhooks/incoming/custom");

            byte[] Bytes = new byte[file.InputStream.Length + 1];
            file.InputStream.Read(Bytes, 0, Bytes.Length);
            var fileContent = new ByteArrayContent(Bytes);
            string filename = file.FileName;

            request.AlwaysMultipartFormData = true;
            request.AddFile("File", Bytes, filename, file.ContentType);
        }


        /// <summary>
        /// Ejecuta la solicitud de servicio web del cliente.
        /// </summary>
        /// <param name="client">El objeto cliente.</param>
        /// <param name="request">La solicitud.</param>
        /// <param name="requestData">Los datos de la solcitud.</param>
        /// <param name="Log">El objeto log para registrar eventos.</param>
        /// <returns>Devuelve un estado booleano y un mensaje que indica el mensaje de error en caso q suceda</returns>
        public static IDictionary<bool, string> ExecuteRequest(RestClient client, RestRequest request, string requestData, ILog Log)
        {
            IDictionary<bool, string> executeResult = new Dictionary<bool, string>();

            try
            {
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                dynamic jsonResult = JsonConvert.DeserializeObject(content);

                if (content != "" && Convert.ToBoolean(jsonResult["Success"]))
                {
                    executeResult.Add(true, "");
                }
                else
                {
                    var contentCode = response.ErrorMessage + "STATUS CODE: " + response.StatusCode;
                    string mesgError = ErrorMessageDAO.ERROR_POST_FILE_API;
                    Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(content+" ERROR_CODE:"+ contentCode, requestData, "Services/UploadFile/ServicesController/ExecuteRequest/CallApiError"));
                    executeResult.Add(false, mesgError);
                    return executeResult;
                }
            }
            catch (Exception error)
            {
                string mesgError = ErrorMessageDAO.ERROR_UPLOAD_API;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message, requestData, "Services/UploadFile/ServicesController/ExecuteRequest/CallApi"));
                executeResult.Add(false, mesgError);
                return executeResult;
            }

            return executeResult;
        }


    }

}