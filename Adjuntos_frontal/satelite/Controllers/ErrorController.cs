using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace satelite.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// Metodo para la renderizacion de la vista principal de la vista de error
        /// </summary>
        /// <param name="token">Datos de error</param>
        /// <returns></returns>
        [HandleError]
        public ActionResult Index(string token)
        {
            return View();
        }

        /// <summary>
        /// Metodo para la renderizacion de la vista principal de la vista de error
        /// cuando la pagina no es encontrada 
        /// </summary>
        /// <param name="token">Datos de error</param>
        /// <returns></returns>
        [HandleError]
        public ActionResult NotFound(string token)
        {
            return View();
        }

    }
}