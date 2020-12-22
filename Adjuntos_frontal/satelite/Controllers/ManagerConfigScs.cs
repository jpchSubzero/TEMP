using System;
using System.Configuration;



namespace satelite.Controllers
{
    public class ManagerConfigScs : AbstManagerConfig
    {
        /// <summary>
        /// Obtiene la configuración general de la página para 
        /// sistemas del Scs.
        /// </summary>
        /// <param name="ViewBag">Objeto ViewBag.</param>
        public static void GetPageConfig(dynamic ViewBag)
        {
            GetGeneralPageConfig(ViewBag);
            ViewBag.inicio = ConfigurationManager.AppSettings["ida:StartScs"];
            ViewBag.reviSoliReal = ConfigurationManager.AppSettings["ida:CheckServiceRequest"];
            ViewBag.limitSizeFileName = ConfigurationManager.AppSettings["ida:LimitSizeFileNameScs"]; //Peso de archivos permitidos (vista)
            ViewBag.limitSizeFileNumber = ConfigurationManager.AppSettings["ida:LimitSizeFileScs"]; //Peso de archivos permitidos (numero de bytes)
            ViewBag.subTitulo = ConfigurationManager.AppSettings["ida:SubTitleNameScs"];
            ViewBag.docuBeca = ConfigurationManager.AppSettings["ida:NameDocuScs"];
        }


        /// <summary>
        /// Obtiene la configuración parcial de la página para 
        /// sistemas del Scs (permite incluir configuraciones especificas)
        /// </summary>
        /// <param name="ViewBag">Objeto ViewBag.</param>
        public static void GetPartialPageConfig(dynamic ViewBag)
        {
            ViewBag.limitSizeFileName = ConfigurationManager.AppSettings["ida:LimitSizeFileNameScs"]; //Peso de archivos permitidos (vista)
            ViewBag.docuBeca = ConfigurationManager.AppSettings["ida:NameDocuScs"]; //Documentos permitos (vista)
        }


        /// <summary>
        /// Gets the URL service request.
        /// </summary>
        /// <param name="env">The enviroment</param>
        /// <returns>The URL to retrieve requirements for Scs</returns>
        public static string GetURLServiceRequest(string env)
        {
            string urlService = "";
            switch (env)
            {
                case "PROD":
                    urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlGetRequestPROD"];
                    break;
                case "PPRD":
                    urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlGetRequestPPRD"];
                    break;
                case "DEVL":
                    urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlGetRequestPPRD"];
                    break;
            }

            string debug = ConfigurationManager.AppSettings["ida:Debug"];
            if (debug == "Y")
            {
                urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlGetRequestLocal"];
            }

            return urlService;
        }


        /// <summary>
        /// Obtiene la ruta URL para permitir la carga de archicos en Scs.
        /// </summary>
        /// <returns>El URL para carga de archivos </returns>
        public static string GetUrlUploadFile(string env)
        {
            string urlService = "";
            switch (env)
            {
                case "PROD":
                    urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlUploadPROD"];
                    break;
                case "PPRD":
                    urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlUploadPPRD"];
                    break;
                case "DEVL":
                    urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlUploadPPRD"];
                    break;
            }

            string debug = ConfigurationManager.AppSettings["ida:Debug"];
            if (debug == "Y")
            {
                urlService = ConfigurationManager.AppSettings["ida:ScsRESTServiceUrlGetRequestLocal"];
            }

            return urlService;
        }


        /// <summary>
        /// Obtiene el liminte de archivos permitidos para la carga.
        /// </summary>
        /// <returns>Limite de peso de archivos permitidos</returns>
        public static int GetLimiteSizeFile()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["ida:LimitSizeFileScs"]);
        }



        /// <summary>
        /// Obtiene el tipo de archivo permitido para la carga en Scs.
        /// </summary>
        /// <returns>El nombre de tipos de archivos permitidos separado por una coma</returns>
        public static string GetAllowedTypeFile()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:TypeDocuScs"]);
        }


        /// <summary>
        /// Codigo de estatus de las solitudes de becas en los que no se permite
        /// cargar archivos cando la solicuts esta abierta.
        /// </summary>
        /// <returns>Codigo de estatus no permitido para carga de requisitos</returns>
        public static string GetAllowCodeUploadOpen()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:StateUploadFileScsOpen"]);
        }


        /// <summary>
        /// Codigo de estatus de las solitudes de becas en los que no se permite
        /// cargar archivos cuando la solcutd esta bloqueada.
        /// </summary>
        /// <returns>Codigo de estatus no permitido para carga de requisitos</returns>
        public static string GetAllowCodeUploadLock()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:StateUploadFileScsLock"]);
        }


        /// <summary>
        /// Obtiene el tipo de contenido para realizar la carga 
        /// de archivos en Scs
        /// </summary>
        /// <returns>Retorna el tipo de contendo en formato de cadena</returns>
        public static string GetContentTypeUploadService()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:ContentTypeScs"]);
        }


        /// <summary>
        /// Recupera el nombre de la libreria usado para la carga de archivos Scs.
        /// </summary>
        /// <returns>Cadena con el nombre de la libreria</returns>
        public static string GetLibraryNameUploadService()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:LibraryNameScs"]);
        }


    }
}