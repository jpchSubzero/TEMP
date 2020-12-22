using System;
using System.Collections.Generic;
using satelite.Models;
using log4net;
using Newtonsoft.Json;
using System.Web;

namespace satelite.Controllers
{
    public class ManagerDataScs : AbstManagerData
    {
        /// <summary>
        /// Valida integralmente el archivo a adjuntar (peso y tipo)
        /// </summary>
        /// <param name="sizeFile">Tamaño maximo permitido.</param>
        /// <param name="typeFile">Tipo de archivo peritido</param>
        /// <returns>Retorna un valor boolean con el estado de la validacion, 
        /// y un mensaje indicado el error en caso q suceda</returns>
        public static IDictionary<bool, string> ValidateFile(int sizeFile, string typeFile)
        {
            IDictionary<bool, string> fileAllowed = new Dictionary<bool, string>();

            int limitSize = ManagerConfigScs.GetLimiteSizeFile();
            string allowedTypeFile = ManagerConfigScs.GetAllowedTypeFile();


            if ( !ValidateSizeFile(sizeFile, limitSize) )
            {
                fileAllowed.Add(false, ErrorMessageDAO.SIZE_FILE_NO_ALLOWED);
                return fileAllowed;
            }

            if ( !ValidateFileType( typeFile, allowedTypeFile))
            {
                fileAllowed.Add(false, ErrorMessageDAO.TYPE_FILE_NO_ALLOWED);
                return fileAllowed;
            }

            fileAllowed.Add(true, "");
            return fileAllowed;
        }


        /// <summary>
        /// Retorna y valida la integridad de los datos pasados por URL, 
        /// revisando su estructura.
        /// </summary>
        /// <param name="requestCodedData">Los datos de la solicitud codificados</param>
        /// <param name="Log">El elemento log usado para registrar eventos</param>
        /// <param name="origin">Origen donde fue llamado el metodo</param>
        /// <returns>Devuelve un estado booleano y un mensaje que indica el mensaje de error en caso q suceda </returns>
        public static IDictionary<bool, string> GetValidatedInternalData(string requestCodedData, ILog Log, string origin)
        {
            IDictionary<bool, string> decodedInternalValidData = new Dictionary<bool, string>();
            IDictionary<bool, string> decodedInternalRequestData = new Dictionary<bool, string>();
            decodedInternalRequestData = GetInternalDecodedData(requestCodedData, Log, origin);

            if (decodedInternalRequestData.ContainsKey(false))
            {                
                return decodedInternalRequestData;
            }

            try
            {
                dynamic requestJsonData = JsonConvert.DeserializeObject(decodedInternalRequestData[true]);
                if (requestJsonData["PK_REQUEST"]["PIDM"] != null &&
                    requestJsonData["ORIGIN"] != null &&
                    requestJsonData["PK_REQUEST"]["NAME"] != null &&
                    requestJsonData["ENV"] != null &&
                    requestJsonData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_APFR_CODE"] != null &&
                    requestJsonData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_AIDY_CODE"] != null &&
                    requestJsonData["PK_REQUEST"]["PK_REQUEST"]["KVRFAPP_AIDP_CODE"] != null)
                {
                    return decodedInternalRequestData;
                }
                else
                {
                    decodedInternalValidData.Add(false, ErrorMessageDAO.NO_REQUEST_DATA);
                    Log.Info(ErrorMessageDAO.GetGenerirErrorLogError("S/A - LOS DATOS NO TIENEN LA INFORMACION NECESARIA PARA PROCESAR LA SOLICITUD",
                        decodedInternalRequestData[true],
                        origin + "/ManagerDataScs/GetValidatedInternalData/"));
                    return decodedInternalValidData;
                }
            }
            catch (Exception error)
            {
                decodedInternalValidData.Add(false, ErrorMessageDAO.NO_REQUEST_DATA);
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message+" LOS DATOS NO TIENE LA INFORMACION NECESARIA PARA PROCESAR LA SOLICITUD", 
                    decodedInternalRequestData[true], 
                    origin + "/ManagerDataScs/GetValidatedInternalData/"));
                return decodedInternalValidData;
            }
            
        }


        /// <summary>
        /// Obtiene la información de contexto HTTP para cargar un archivo.
        /// </summary>
        /// <param name="httpContext">Contexto HTTP.</param>
        /// <returns>Informacion de contexto en formato de cadena de texto</returns>
        public static string GetContextInfoUploadFileLog(HttpContextBase httpContext)
        {
            string scholarShipName = httpContext.Request.Params.Get("scholarShipName"),
                scholarshipFundValue = httpContext.Request.Params.Get("scholarshipFundValue"),
                requestCode = httpContext.Request.Params.Get("requestCode"),
                requestDesc = httpContext.Request.Params.Get("requestDesc"),
                requestData = httpContext.Request.Params.Get("requestData");

            return requestData + " | " + requestCode + " | " + requestDesc + " | " + scholarshipFundValue + " | " + scholarShipName;
        }

    
    }
}