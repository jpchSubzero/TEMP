using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using log4net;
using satelite.Models;
using System.Linq;
using System.Configuration;

namespace satelite.Controllers
{
    //[Authorize]
    public class ScholarshipController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ScholarshipController));

        /// <summary>
        /// Metodo para la renderizacion de la vista principal de la vista de becas
        /// </summary>
        /// <param name="token">Datos de la beca encriptados</param>
        /// <returns></returns>
        // GET: Scholarship
        public ActionResult Index(string token)
        {
            ManagerConfigScs.GetPageConfig(ViewBag);
            return View();
        }


        /// <summary>
        /// Renderizacion paracial de contentido extra en la vista.
        /// </summary>
        /// <returns>PartialView</returns>
        [HttpGet]
        public PartialViewResult ExtraContent()
        {
            ManagerConfigScs.GetPartialPageConfig(ViewBag);
            return PartialView("_ExtraContent");
        }


        /// <summary>
        /// Recupera los requermientos de la beca desde aun llamado al API.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRequirements()
        {
            bool status = false;
            string mesgError = "",
                origin = "Scolarship/GetRequirements",
                urlBaseRequest = "",
                urlRequest = "";
            dynamic requestJsonData;
            //var requestCodedData = HttpContext.Request.Params.Get("requestData");
            var requestCodedData = "ACXZPTPGOY7YOMF3SPDZE3UXMFMDMP6R55CJ7XOH2EOCKQ3GXIP3TTDUPWGFHK3J2BSCU7KFSBRFYX2Z7TBZMPJPWNEKQKKOJ43C7HTEJB4KXH7JWHCVB4SKGGW6FRYUNBAHT3WIEQKHESCAWJPV7I7VZI5SF3RSL56UTTKSKTNUUHQOSNCUT4ND6LLTBHUQB2YEQA7V77GNGLOUTL4P2PJF27YCNH5BJ47EHDBGMA4PWVSYCCH52GAVSTMLDDU7GWGF3HBI3YQKDGCAUBLDWEXASN7BJUFSAWWXJN7OVEGJWK573EPPR7ZWDPW7AOVZ7JDKKBSDGSW5G4755RRVZ2N5N2OOCTNPD5AE42UCGY73WX5OIQZLEEKKKYBM5SNO%24MAKS5EO75ZSYNXSWXKVUAWZZAY%243F7MDFZLAK5Y4L6WCUE6Y5FWKI";
            IDictionary<bool, string> decodedInternalRequestData = new Dictionary<bool, string>();
            IDictionary<bool, dynamic> executeRequestData = new Dictionary<bool, dynamic>();

            decodedInternalRequestData = ManagerDataScs.GetValidatedInternalData(requestCodedData, Log, origin);

            if (decodedInternalRequestData.ContainsKey(false))
            {
                status = false;
                return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + decodedInternalRequestData[false] + "\"}");
            }

            requestJsonData = JsonConvert.DeserializeObject(decodedInternalRequestData[true]);
            urlBaseRequest = ManagerConfigScs.GetURLServiceRequest(Convert.ToString(requestJsonData["ENV"]));
            urlRequest = ManagerRequestScs.GetUrlRequestRequiments(requestJsonData);
            var client = new RestClient(urlRequest);
            var request = new RestRequest();
            request.Method = Method.GET;

            AbsManagerRequest.GetCookieCode(ControllerContext, request, urlBaseRequest, "GET", "Scs");

            executeRequestData = ManagerRequestScs.ExecuteRequestGetRequirements(client, request, decodedInternalRequestData[true], Log);

            if (executeRequestData.ContainsKey(false))
            {
                status = false;
                return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + executeRequestData[false] + "\"}");
            }

            status = true;
            var requestScholarship = executeRequestData[true]["Payload"]["DATA"]["REQUIREMENTS"];

            if (!executeRequestData[true]["Payload"]["DATA"]["REQUIREMENTS"].HasValues)
            {
                requestScholarship = "[]";
            }
            else
            {
                string status_lock = executeRequestData[true]["Payload"]["DATA"]["STATUS_LOCK"];
                CheckUploadPermiss(requestScholarship, status_lock);
            }

            return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + mesgError + "\" ,\"dataRequest\": {" +
                "\"BECA_DESC\":\"" + executeRequestData[true]["Payload"]["PK_REQUEST"]["BECA_DESC"] + "\" ," +
                "\"BECA_CODE\":\"" + executeRequestData[true]["Payload"]["PK_REQUEST"]["BECA_CODE"] + "\" ," +
                "\"STATUS_LOCK\":\"" + executeRequestData[true]["Payload"]["DATA"]["STATUS_LOCK"] + "\"," +
                "\"REQUIREMENTS\":" + requestScholarship + "," +
                "\"ALLOW_CODE_LOCK\":\"" + ManagerConfigScs.GetAllowCodeUploadLock() + "\"" +
                "} }");
        }

        /// <summary>
        /// Checks the upload permiss.
        /// </summary>
        /// <param name="requestScholarship">The request scholarship.</param>
        /// <param name="statusLock">The status lock.</param>
        private void CheckUploadPermiss(dynamic requestScholarship, string statusLock)
        {
            var uploadCode = (statusLock == "Y") ? ManagerConfigScs.GetAllowCodeUploadLock().Split(',') : ManagerConfigScs.GetAllowCodeUploadOpen().Split(',');

            foreach (var item in requestScholarship)
            {
                item["UPLOAD_STATUS"] = Array.Exists(uploadCode, element => element == Convert.ToString(item["KVRAREQ_TRST_CODE"]));
            }
        }


        /// <summary>
        /// Carga un archivo para un requerimiento en becas
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            bool status = true;
            string requestData = HttpContext.Request.Params.Get("requestData"),

                mesgError = "",
                typeFile = "",
                RESTServiceUrl = "",
                origin = "Scholarship/UploadFile/",
                urlDocu = "",
                requestCode = HttpContext.Request.Params.Get("requestCode");
            dynamic requestJsonData;
            int sizeFile = 0;

            requestData = "ACXZPTPGOY7YOMF3SPDZE3UXMFMDMP6R55CJ7XOH2EOCKQ3GXIP3TTDUPWGFHK3J2BSCU7KFSBRFYX2Z7TBZMPJPWNEKQKKOJ43C7HTEJB4KXH7JWHCVB4SKGGW6FRYUNBAHT3WIEQKHESCAWJPV7I7VZI5SF3RSL56UTTKSKTNUUHQOSNCUT4ND6LLTBHUQB2YEQA7V77GNGLOUTL4P2PJF27YCNH5BJ47EHDBGMA4PWVSYCCH52GAVSTMLDDU7GWGF3HBI3YQKDGCAUBLDWEXASN7BJUFSAWWXJN7OVEGJWK573EPPR7ZWDPW7AOVZ7JDKKBSDGSW5G4755RRVZ2N5N2OOCTNPD5AE42UCGY73WX5OIQZLEEKKKYBM5SNO%24MAKS5EO75ZSYNXSWXKVUAWZZAY%243F7MDFZLAK5Y4L6WCUE6Y5FWKI";

            IDictionary<bool, string> validFile = new Dictionary<bool, string>();
            IDictionary<bool, string> decodedInternalRequestData = new Dictionary<bool, string>();

            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = (files.Count > 0) ? files[0] : null;

            if (file == null)
            {
                status = false;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(
                    ErrorMessageDAO.FILE_NOT_FOUNT,
                    ManagerDataScs.GetContextInfoUploadFileLog(HttpContext),
                    origin + "file"));
                return Json("{\"status\":\"" + status +
                    "\" ,\"message\":\"" + ErrorMessageDAO.FILE_NOT_FOUNT +
                    "\" ,\"url\":\"" + urlDocu +
                    "\", \"requestCode\":\"" + requestCode + "\"}");
            }

            sizeFile = file.ContentLength;
            typeFile = Path.GetExtension(file.FileName);
            validFile = ManagerDataScs.ValidateFile(sizeFile, typeFile);

            if (validFile.ContainsKey(false))
            {
                status = false;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(
                    validFile[false],
                    ManagerDataScs.GetContextInfoUploadFileLog(HttpContext),
                    origin + "validateFile"));
                return Json("{\"status\":\"" + status +
                    "\" ,\"message\":\"" + validFile[false] +
                    "\" ,\"url\":\"" + urlDocu +
                    "\", \"requestCode\":\"" + requestCode + "\"}");
            }

            decodedInternalRequestData = ManagerDataScs.GetValidatedInternalData(requestData, Log, origin);

            if (decodedInternalRequestData.ContainsKey(false))
            {
                status = false;
                return Json("{\"status\":\"" + status +
                    "\" ,\"message\":\"" + decodedInternalRequestData[false] +
                    "\" ,\"url\":\"" + urlDocu +
                    "\", \"requestCode\":\"" + requestCode + "\"}");
            }

            requestJsonData = JsonConvert.DeserializeObject(decodedInternalRequestData[true]);
            //RESTServiceUrl = ManagerConfigScs.GetUrlUploadFile(Convert.ToString(requestJsonData["ENV"]));
            RESTServiceUrl = ConfigurationManager.AppSettings["ida:ScsRESTServiceUr"];

            var client = new RestClient(RESTServiceUrl);
            var request = new RestRequest();
            request.Method = Method.POST;

            try
            {
                //Obtencion del codigo de autorizacion para consumo del servicio
                AbsManagerRequest.GetCookieCode(ControllerContext, request, RESTServiceUrl, "POST", "Scs");
            }
            catch (Exception error)
            {
                status = false;
                mesgError = ErrorMessageDAO.ERROR_GENERATE_COOKI;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message, decodedInternalRequestData[true], origin + "AbsManagerRequest/GetCookieCode"));
                return Json("{\"status\":\"" + status +
                    "\" ,\"message\":\"" + mesgError +
                    "\" ,\"url\":\"" + urlDocu +
                    "\", \"requestCode\":\"" + requestCode + "\"}");
            }

            try
            {
                ManagerRequestScs.SetParametersUploadFileRequest(request, requestJsonData, file, HttpContext);
            }
            catch (Exception error)
            {
                status = false;
                mesgError = ErrorMessageDAO.NO_REQUEST_DATA;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message, decodedInternalRequestData[true], origin + "SetParametersUploadFileRequest"));
                return Json("{\"status\":\"" + status +
                    "\" ,\"message\":\"" + mesgError +
                    "\" ,\"url\":\"" + urlDocu +
                    "\", \"requestCode\":\"" + requestCode + "\"}");
            }

            IDictionary<bool, string> executeResult = ManagerRequestScs.ExecuteRequestUploadFile(client, request, decodedInternalRequestData[true], Log);

            if (executeResult.ContainsKey(false))
            {
                status = false;
                mesgError = executeResult[false];
            }
            else
            {
                status = true;
                urlDocu = executeResult[true];
            }

            return Json("{\"status\":\"" + status +
                "\" ,\"message\":\"" + mesgError +
                "\" ,\"url\":\"" + urlDocu +
                "\", \"requestCode\":\"" + requestCode + "\"}");
        }




    }
}