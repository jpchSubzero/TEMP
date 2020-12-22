using RestSharp;
using System;
using System.Web.Mvc;
using System.Security.Cryptography;
using HttpCookie = System.Web.HttpCookie;
using System.Text;
using System.Threading.Tasks;

namespace satelite.Controllers
{
    public class AbsManagerRequest
    {


        /// <summary>
        /// Obtiene el cookie de autorización para consumo de servicios.
        /// </summary>
        /// <param name="controllerContext">El controllador de context.</param>
        /// <param name="request">La solicitud request.</param>
        /// <param name="RESTServiceUrl">La URL del servicio que se autorizara.</param>
        /// <param name="method">El metodo a autorizar (GET/POST).</param>
        /// <param name="origin">El origin de las solicitud (Oss ->servicios / Scs -> Becas).</param>
        public static void GetCookieCode(ControllerContext controllerContext, RestRequest request, string RESTServiceUrl, string method, string origin)
        {
            string autorizacionCookie = "";
            string codigoCookie = "";
            string APPId = AbstManagerConfig.GetAppId(); 
            string APIKey = AbstManagerConfig.GetApiKey();
            var cookie = controllerContext.HttpContext.Request.Cookies[codigoCookie];

            //cooke de autorizacion para token de S/O (scholarship/becas) servicio accion P post/G get
            if (origin == "Oss")
            {
                codigoCookie = (method == "POST") ? "_catsOap" : "_catsOag";
            }
            else
            {
                codigoCookie = (method == "POST") ? "_catsSap" : "_catsSag";
            }

            if (cookie != null)
            {
                autorizacionCookie = cookie.Value;
            }
            else
            {
                autorizacionCookie = GetAuthorizationCode(APPId, APIKey, method, RESTServiceUrl);
                HttpCookie cookieSolicitud = new HttpCookie(codigoCookie, autorizacionCookie);
                cookieSolicitud.Expires = DateTime.Now.AddMinutes(9);
                controllerContext.HttpContext.Response.SetCookie(cookieSolicitud);
            }

            request.AddHeader("Authorization", autorizacionCookie);
        }


        /// <summary>
        /// Obtiene el codigo de autorizacion para el llamado de servicios WEB API.
        /// </summary>
        /// <param name="appId">Identificador de la aplicacion.</param>
        /// <param name="apiKey">API key de la aplicacion (Llave).</param>
        /// <param name="requestHttpMethod">Metodo HTTP invocado (POST - GET).</param>
        /// <param name="requestURL">URL llamado para invocar el WEB API.</param>
        /// <returns>Codigo de autorizacion</returns>
        private static string GetAuthorizationCode(string appId, string apiKey, string requestHttpMethod, string requestURL)
        {
            //El formato del código de autorizacion que se envía en el "Header Authorization" es:
            //"amx appId:firma:timestamp"
            //En donde: amx es el nombre del esquema personalizado de autenticación
            //          appId --> identificador del aplicativo, este valor es entregado por el proveedor del servicio (UTPL)
            //          firma --> corresponde a un valor hash de una cadena definida
            //          timestamp --> marca de tiempo. Es el total de segundos transcurridos desde la fecha del 01/01/1970 hasta la fecha actual en UTC

            //Calcular FIRMA
            //La firma se calcula mediante el algoritmo hash SHA256 utilizando la clave apiKey entregada por el proveedor del servicio (UTPL)
            //La cadena a firmar tiene el siguiente formato: "appIdrequestHttpMethodrequestUrirequestTimeStamp" (todos los parámetros son seguidos)
            //Ejemplo de la cadena a firmar: 
            //   4d53bce03ec34c0a911182d4c228ee6cPOSThttps%3a%2f%2fsrv-si-001.utpl.edu.ec%2frest_pro%2fapi%2fseguridad%2fautenticacion1443536591
            //   *------------------------------*---*--------------------------------------------------------------------------------*----------
            //                 appId            method                              uri en minúsculas y codificado                      timestamp
            //Finalmente de la firma se obtiene su representación en string en base64
            string firma = "";
            string requestUri = System.Web.HttpUtility.UrlEncode(requestURL.ToLower());

            //Calcular TIMESTAMP
            //Para calcular el timestamp se utiliza la siguiente expresión: timestamp = (fechaActualUTC - fecha_01_01_1970).TotalSegundos            
            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //Se crea la cadena a firmar
            string cadenaAFirmar = String.Format("{0}{1}{2}{3}", appId, requestHttpMethod, requestUri, requestTimeStamp);

            //Se obtiene el arreglo de byte desde la clave apiKey que es un string en base64
            byte[] apiKeyByteArray = Convert.FromBase64String(apiKey);
            //Se obtiene el arreglo d bytes de la cadenaAFirmar
            byte[] signature = Encoding.UTF8.GetBytes(cadenaAFirmar);
            //Se instancia del algoritmo HMACSHA256 con la clave apiKey
            using (HMACSHA256 hmac = new HMACSHA256(apiKeyByteArray))
            {
                //Se obtiene la firma (valor hash)
                byte[] signatureBytes = hmac.ComputeHash(signature);
                //Se obtiene la representación a string en base64
                firma = Convert.ToBase64String(signatureBytes);
            }

            //Se arma el código de autorización en el formato indicado. Nota que primero va el esquema
            //que siempre va a ser "amx" seguido de un espacio y luego la trama "appId:firma:timestamp"
            string codigoAutorizacion = "amx " + appId + ":" + firma + ":" + requestTimeStamp;
            return codigoAutorizacion;
            //return "amx 4d53bce03ec34c0a911182d4c228ee6c:zpwIaBdEqH6PfmDpeq88TETX7b0+n620QjD0oIkm2Lg=:1443328943";
        }


        /// <summary>
        /// Ejecusion asincrona de la solicitud.
        /// </summary>
        /// <param name="client">Cliente de ejecucion.</param>
        /// <param name="request">Solicitud a ser ejecutada.</param>
        /// <returns>Tarea completada (Respuesta)</returns>
        public static Task<IRestResponse> ExecuteAsyncRequest(RestClient client, IRestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, restResponse =>
            {
                taskCompletionSource.SetResult(restResponse);
            });

            return taskCompletionSource.Task;
        }

                     
    }
}