using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using RestSharp;
using Newtonsoft.Json;
using satelite.Models;
using log4net;
using System.Threading.Tasks;


namespace satelite.Controllers
{
    public class ManagerRequestScs: AbsManagerRequest
    {
        /// <summary>
        /// Recupera la URL para consultar requerimientos
        /// </summary>
        /// <param name="requestData">Los datos de la solicitud</param>
        /// <returns>URL en formato string</returns>
        public static string GetUrlRequestRequiments(dynamic requestData)
        {
            string urlBaseRequest = ManagerConfigScs.GetURLServiceRequest(Convert.ToString(requestData["ENV"]));
            string urlRequest = urlBaseRequest +
                "?Pidm=" + requestData["PK_REQUEST"]["PIDM"] +
                "&Apfr_Code=" + requestData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_APFR_CODE"] +
                "&Aidy_Code=" + requestData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_AIDY_CODE"] +
                "&Aidp_Code=" + requestData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_AIDP_CODE"] +
                "&Origin=" + requestData["ORIGIN"] +
                "&Env=" + requestData["ENV"];

            return urlRequest;
        }


        /// <summary>
        /// Ejecuta la solicitud para recuperar requerimientos 
        /// </summary>
        /// <param name="client">El cliente de consulta.</param>
        /// <param name="request">La solicitud.</param>
        /// <param name="requestData">Los datos de consulta</param>
        /// <param name="Log">Objeto log para .</param>
        /// <returns>Devuelve un estado booleano y un mensaje que indica el mensaje de error en caso de ser necesario</returns>
        public static IDictionary<bool, dynamic> ExecuteRequestGetRequirements(RestClient client, RestRequest request, string requestData, ILog Log)
        {
            IDictionary<bool, dynamic> executeResult = new Dictionary<bool, dynamic>();
            IDictionary<bool, dynamic> validateResult = new Dictionary<bool, dynamic>();
            try
            {
                var response = new RestResponse();
                Task.Run(async () =>
                {
                    response = await ExecuteAsyncRequest(client, request) as RestResponse;
                }).Wait(TimeSpan.FromMilliseconds(50000));

                var content = response.Content;
                dynamic jsonResult = JsonConvert.DeserializeObject(content);

                if (Convert.ToBoolean(jsonResult["Success"]))
                {
                    validateResult = ValidateRequirements(jsonResult, content, Log);
                    if (validateResult.ContainsKey(false))
                    {
                        return validateResult;
                    }

                    executeResult.Add(true, jsonResult);
                    return executeResult;
                }
                else
                {
                    string mesgErrorAPI = jsonResult["Error"];
                    string mesgError = ErrorMessageDAO.ERROR_GET_REQUEST_API;
                    Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(content+" ERROR_JSON:"+mesgErrorAPI, requestData, "Scholarship/GetRequirements/ExecuteRequestGetRequirements/CallApiError"));
                    executeResult.Add(false, mesgError);
                    return executeResult;
                }
            }
            catch (Exception error)
            {
                string mesgError = ErrorMessageDAO.ERROR_GET_REQUEST_API;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message+" NO SE PUDO LLAMAR AL SERVICIO", requestData, 
                    "Scholarship/GetRequirements/ExecuteRequestGetRequirements/CallApi"
                    )
                );
                executeResult.Add(false, mesgError);
                return executeResult;
            }
        }


        /// <summary>
        /// Ejecuta la solicitud para subir el archivo
        /// </summary>
        /// <param name="client">El cliente de consulta.</param>
        /// <param name="request">La solicitud.</param>
        /// <param name="requestData">Los datos de consulta</param>
        /// <param name="Log">Objeto log para .</param>
        /// <returns>Devuelve un estado booleano y un mensaje que indica el mensaje de error en caso de ser necesario</returns>
        public static IDictionary<bool, string> ExecuteRequestUploadFile(RestClient client, RestRequest request, string requestData, ILog Log)
        {
            IDictionary<bool, string> executeResult = new Dictionary<bool, string>();

            try
            {
                var response = new RestResponse();
                Task.Run(async () =>
                {
                    response = await ExecuteAsyncRequest(client, request) as RestResponse;
                }).Wait(TimeSpan.FromMilliseconds(50000));

                var content = response.Content;
                dynamic jsonResult = JsonConvert.DeserializeObject(content);

                var success = jsonResult["Result"]["Success"];

                if (success != null && Convert.ToBoolean(success))
                {
                    var url = jsonResult["Result"]["Data"]["UrlFile"];
                    executeResult.Add(true, Convert.ToString(url));
                }
                else
                {
                    string mesgErrorAPI = jsonResult["Error"];
                    string mesgError = ErrorMessageDAO.ERROR_UPLOAD_API;
                    Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(content + " ERROR_JSON:" + mesgErrorAPI, requestData,
                        "Scholarship/UploadFile/ScholarshipController/ExecuteRequest/CallApiError"));
                    executeResult.Add(false, mesgError);
                    return executeResult;
                }
            }
            catch (Exception error)
            {
                string mesgError = ErrorMessageDAO.ERROR_UPLOAD_API;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message, requestData, "Scholarship/UploadFile/ScholarshipController/ExecuteRequest/CallApi"));
                executeResult.Add(false, mesgError);
                return executeResult;
            }

            return executeResult;
        }


        /// <summary>
        /// Valida informacion recuperda de los requerimientos.
        /// </summary>
        /// <param name="responseData">Los datos de respuesta a validar.</param>
        /// <param name="content">El contenido.</param>
        /// <param name="Log">Objeto log para .</param>
        /// <returns>Devuelve un estado booleano y un mensaje que indica el mensaje de error en caso de ser necesario</returns>
        public static IDictionary<bool, dynamic> ValidateRequirements(dynamic responseData, string content, ILog Log)
        {
            IDictionary<bool, dynamic> validateResult = new Dictionary<bool, dynamic>();

            if (responseData["Payload"]["DATA"]["STATUS_LOCK"] ==null )
            {
                string mesgError = ErrorMessageDAO.ERROR_INTERMINTATE_STATE;
                validateResult.Add(false, ErrorMessageDAO.ERROR_INTERMINTATE_STATE);
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(
                    "ESTATUS DE SOLICITUD NULO", 
                    content, 
                    "Scholarship/GetRequirements/ExecuteRequestGetRequirements/ValidateRequirements"
                    )
                );
                return validateResult;
            }

            if (responseData["Payload"]["PK_REQUEST"]["BECA_DESC"] == null)
            {
                string mesgError = ErrorMessageDAO.ERROR_INTERMINTATE_DESCRIPTION;
                validateResult.Add(false, ErrorMessageDAO.ERROR_INTERMINTATE_DESCRIPTION);
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(
                    "DESCRIPCION DE BECA NULA", 
                    content, 
                    "Scholarship/GetRequirements/ExecuteRequestGetRequirements/ValidateRequirements"
                    )
                );
                return validateResult;
            }


            validateResult.Add(true, "");
            return validateResult; 
        }


        /// <summary>
        ///  Agrega los parametros necesarios a la solicitud para cargar el archivo
        /// </summary>
        /// <param name="request">La solicitud.</param>
        /// <param name="dynamicData">Lo datos en formato dynamico.</param>
        /// <param name="file">El archivo.</param>
        /// <param name="httpContext">El contexto HTTP.</param>
        public static void SetParametersUploadFileRequest(RestRequest request, dynamic dynamicData, HttpPostedFileBase file, HttpContextBase httpContext)
        {
            int year = dynamicData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_AIDY_CODE"],
                pidm = dynamicData["PK_REQUEST"]["PIDM"];
            string origin = dynamicData["ORIGIN"],
                dni = dynamicData["PK_REQUEST"]["CI"],            
                scholarSchip = dynamicData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_APFR_CODE"],
                period = dynamicData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_AIDP_CODE"],
                env = dynamicData["ENV"],
                studentName = dynamicData["PK_REQUEST"]["NAME"],
                contentType = ManagerConfigScs.GetContentTypeUploadService(),
                libraryName = ManagerConfigScs.GetLibraryNameUploadService(),
                scholarShipName = httpContext.Request.Params.Get("scholarShipName"),
                scholarshipFundValue = httpContext.Request.Params.Get("scholarshipFundValue"),
                requestCode = httpContext.Request.Params.Get("requestCode"),
                requestDesc = httpContext.Request.Params.Get("requestDesc");

            //request.AddParameter("Pidm", pidm);
            //request.AddParameter("ScholarshipCode", scholarSchip.Trim());
            //request.AddParameter("ScholarshipDesc", scholarShipName.Trim());
            //request.AddParameter("LibraryName", libraryName);
            //request.AddParameter("ContentType", contentType);
            //request.AddParameter("HelpPeriod", period.Trim());
            //request.AddParameter("Year", year);
            //request.AddParameter("RequirementCode", requestCode.Trim());
            //request.AddParameter("RequirementDesc", requestDesc.Trim());
            //request.AddParameter("Cedula", dni.Trim());
            //request.AddParameter("Origin", origin.Trim());
            //request.AddParameter("Env", env.Trim());
            //request.AddParameter("FoundCode", scholarshipFundValue.Trim());
            //request.AddParameter("StudentName", studentName.Trim());

            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("Pidm", pidm.ToString());
            metadata.Add("Identificacion", dni.Trim());
            metadata.Add("StudentName", studentName.Trim().Replace(" ", "_"));
            metadata.Add("Apfr_Code", scholarSchip.Trim());
            metadata.Add("Apfr_Desc", scholarShipName.Trim());
            metadata.Add("Found_Code", scholarshipFundValue.Trim());
            metadata.Add("Aidy_Code", year.ToString().Trim());
            metadata.Add("Aidp_Code", period.Trim());
            metadata.Add("Treq_Code", requestCode.Trim());
            metadata.Add("Treq_Desc", requestDesc.Trim());

            var json = JsonConvert.SerializeObject(metadata);

            request.AddParameter("Origin", origin.Trim());
            request.AddParameter("Metadata", json);
            request.AddParameter("Env", env.Trim());

            byte[] Bytes = new byte[file.InputStream.Length + 1];
            file.InputStream.Read(Bytes, 0, Bytes.Length);
            var fileContent = new ByteArrayContent(Bytes);
            string filename = file.FileName;

            request.AlwaysMultipartFormData = true;
            request.AddFile("File", Bytes, filename, file.ContentType);
            request.Timeout = 50000;
        }


    }
}