using System;
using System.Configuration;
using System.Globalization;

namespace satelite.Controllers
{
    public abstract class AbstManagerConfig
    {
        /// <summary>
        /// Recupera la fecha actual.
        /// </summary>
        /// <returns>Fecha actual en formato "Dic. 11, 2019 12:40 PM"</returns>
        public static string GetActualDate()
        {
            string day = DateTime.Today.ToString("dd");
            string mont = DateTime.Today.ToString("MMM", CultureInfo.CreateSpecificCulture("es"));
            string year = DateTime.Today.ToString("yyyy");
            string hour = DateTime.Now.ToString("h:mm tt");

            return mont + " " + day + ", " + year + " " + hour;
        }


        /// <summary>
        /// Agrega los elementos de basicos de confguracion de la pagina
        /// al elemento ViewBag, los caloes se obtienen del Web.config
        /// </summary>
        /// <param name="ViewBag">Elemento view bag.</param>
        protected static void GetGeneralPageConfig(dynamic ViewBag)
        {
            ViewBag.date = GetActualDate();
            ViewBag.doimio = ConfigurationManager.AppSettings["ida:Wtrealm"];
            ViewBag.ambiente = ConfigurationManager.AppSettings["ida:RootURL"];
            ViewBag.infoEstu = ConfigurationManager.AppSettings["ida:StudentInformation"];
            ViewBag.alumnos = ConfigurationManager.AppSettings["ida:Student"];
            ViewBag.becasMatr = ConfigurationManager.AppSettings["ida:Scholarship"];
            ViewBag.servEstu = ConfigurationManager.AppSettings["ida:EstudentService"];
            ViewBag.prosEstu = ConfigurationManager.AppSettings["ida:Leaflets"];
            ViewBag.salir = "Account/SignOut";            
        }


        /// <summary>
        /// Recupera el identificador de la aplicacion 
        /// para ser usado en la autorizacion de recursos
        /// el valor esta configurado en el archivo Web.config
        /// </summary>
        /// <returns>El identificador en formato string</returns>
        public static string GetAppId()
        {
            string APPId = ConfigurationManager.AppSettings["ida:APPId"];
            return APPId;
        }

        /// <summary>
        /// Gets the codification mode64.
        /// </summary>
        /// <returns></returns>
        public static string GetCodificationMode64()
        {
            string codi_mode = ConfigurationManager.AppSettings["ida:Codification64"];
            return codi_mode;
        }


        /// <summary>
        /// Recupera la llave del API (API key) 
        /// para ser usado en la autorizacion de recursos
        /// el valor esta configurado en el archivo Web.config
        /// </summary>
        /// <returns>La llave de la aplicacion en formato cadena </returns>
        public static string GetApiKey()
        {
            string APIKey = ConfigurationManager.AppSettings["ida:APIKey"];
            return APIKey;           
        }


    }
}