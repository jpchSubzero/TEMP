using System;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using RestSharp;
using System.IO;
using log4net;
using satelite.Models;

namespace satelite.Controllers
{
    //[Authorize]
    public class ServicesController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServicesController));


        /// <summary>
        /// Renderizacion de la vista principal
        /// </summary>
        /// <param name="token">Datos codificados de la solicitud de servicio</param>
        /// <returns></returns>
        public ActionResult Index(string token)
        {
            ManagerConfigOss.GetPageConfig(ViewBag);
            return View();
        }

        /// <summary>
        /// Renderizacion paracial de contentido extra en la vista.
        /// </summary>
        /// <returns>PartialView</returns>
        [HttpGet]
        public PartialViewResult ExtraContent()
        {
            ManagerConfigOss.GetPartialPageConfig(ViewBag);
            return PartialView("_ExtraContent");
        }

        /// <summary>
        /// Carga un archivo para un requerimiento en Oss.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateFile()
        {
            string token = HttpContext.Request.Params.Get("Token"), 
                mesgError = "", 
                typeFile = "",
                origin = "Services/UploadFile/"; 
            int sizeFile = 0; 
            var status = true;
            IDictionary<bool, string> validFile = new Dictionary<bool, string>();
            IDictionary<bool, string> decodedInternalRequestData = new Dictionary<bool, string>();

            ViewBag.date = AbstManagerConfig.GetActualDate();
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = (files.Count > 0)? files[0]:null;

            if (file == null)
            {
                status = false;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(ErrorMessageDAO.FILE_NOT_FOUNT, token, origin+"file") );
                return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + ErrorMessageDAO.FILE_NOT_FOUNT + "\"}");
            }

            sizeFile = file.ContentLength;  
            typeFile = Path.GetExtension(file.FileName);
            validFile = ManagerDataOss.ValidateFile(sizeFile, typeFile);

            if (validFile.ContainsKey(false))
            {
                status = false;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(validFile[false], token, origin+"validateFile"));
                return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + validFile[false] + "\"}");
            }
            
            decodedInternalRequestData = AbstManagerData.GetInternalDecodedData(token, Log, origin);

            if (decodedInternalRequestData.ContainsKey(false))
            {
                status = false;
                return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + decodedInternalRequestData[false] + "\"}");
            }

            //URL usado para el envio del archivo
            string RESTServiceUrl = ManagerConfigOss.GetUrlUploadFile();
            var client = new RestClient(RESTServiceUrl);
            var request = new RestRequest();
            request.Method = Method.POST;

            try
            {
                //Obtencion del codigo de autorizacion para consumo del servicio
                AbsManagerRequest.GetCookieCode(ControllerContext, request, RESTServiceUrl, "POST", "Oss");
            }
            catch (Exception error)
            {
                status = false;
                mesgError = ErrorMessageDAO.ERROR_GENERATE_COOKI;
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message, decodedInternalRequestData[true], origin+"AbsManagerRequest/GetCookieCode"));
                return Json("{\"status\":\"" + status + "\" ,\"message\":\"" + mesgError + "\"}");                
            }

            ManagerRequestOss.SetParametersRequest(request, decodedInternalRequestData[true], file);

            IDictionary<bool, string> executeResult = ManagerRequestOss.ExecuteRequest(client, request, decodedInternalRequestData[true], Log);

            if (executeResult.ContainsKey(false))
            {
                status = false;
                mesgError = executeResult[false];
            }
            else
            {
                status = true;
            }            

            return Json("{\"status\":\""+status+"\" ,\"message\":\""+mesgError+"\"}");
        }


    }
}
 