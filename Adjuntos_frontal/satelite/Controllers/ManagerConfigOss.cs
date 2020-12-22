using System;
using System.Configuration;


namespace satelite.Controllers
{
    public class ManagerConfigOss : AbstManagerConfig
    {
        /// <summary>
        /// Obtiene la configuración general de la página para 
        /// sistemas del Oss.
        /// </summary>
        /// <param name="ViewBag">Objeto ViewBag.</param>
        public static void GetPageConfig(dynamic ViewBag)
        {
            GetGeneralPageConfig(ViewBag);
            ViewBag.inicio = ConfigurationManager.AppSettings["ida:StartOss"];
            ViewBag.reviSoliReal = ConfigurationManager.AppSettings["ida:CheckServiceRequest"];
            ViewBag.limitSizeFileName = ConfigurationManager.AppSettings["ida:LimitSizeFileNameOss"]; //Peso de archivos permitidos (vista)
            ViewBag.limitSizeFileNumber = ConfigurationManager.AppSettings["ida:LimitSizeFileOss"]; //Peso de archivos permitidos (numero de bytes)
            ViewBag.docuServ = ConfigurationManager.AppSettings["ida:NameDocuOss"]; //Documentos permitos (vista)
            ViewBag.subTitulo = ConfigurationManager.AppSettings["ida:SubTitleNameOss"];
            ViewBag.docuBeca = ConfigurationManager.AppSettings["ida:NameDocuOss"];
        }


        /// <summary>
        /// Obtiene la configuración parcial de la página para 
        /// sistemas del Oss (permite incluir configuraciones especificas)
        /// </summary>
        /// <param name="ViewBag">Objeto ViewBag.</param>
        public static void GetPartialPageConfig(dynamic ViewBag)
        {
            ViewBag.ambiente = ConfigurationManager.AppSettings["ida:RootURL"];
            ViewBag.reviSoliReal = ConfigurationManager.AppSettings["ida:CheckServiceRequest"];
        }


        /// <summary>
        /// Obtiene el límite de tamaño permitido para archivos Oss.
        /// </summary>
        /// <returns>El límite en bytes</returns>
        public static int GetLimiteSizeFile()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["ida:LimitSizeFileOss"]);
        }


        /// <summary>
        /// Obtiene el tipo de archivo permitido para la carga en Oss.
        /// </summary>
        /// <returns>El nombre de tipos de archivos permitidos separado por una coma</returns>
        public static string GetAllowedTypeFile()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:TypeDocuOss"]);
        }


        /// <summary>
        /// Obtiene la ruta URL para permitir la carga de archicos en Oss.
        /// </summary>
        /// <returns>El URL para carga de archivos </returns>
        public static string GetUrlUploadFile()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:OssRESTServiceUrlUpload"]);
        }


        /// <summary>
        /// Obtiene el tipo de contenido usado para el servicio de carga para Oss.
        /// </summary>
        /// <returns>Cadena de tipo de contenido para carga de archivos en OSS</returns>
        public static string GetContentTypeUploadService()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:ContentTypeOss"]);
        }


        /// <summary>
        /// Recupera el nombre de la libreria usado para la carga de archivos OSS.
        /// </summary>
        /// <returns>Cadena con el nombre de la libreria</returns>
        public static string GetLibraryNameUploadService()
        {
            return Convert.ToString(ConfigurationManager.AppSettings["ida:LibraryNameOss"]);
        }


    }
}