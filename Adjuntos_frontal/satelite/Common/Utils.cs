using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace satelite.Common
{
    public class Utils
    {
        /// <summary>
        /// Limpia el nombre de un archivo para subirlo a Sharepoint
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string CleanFileName(string fileName)
        {
            Regex reg = new Regex("[#%&*:,<>?/{|}\"]");
            string cleanFileName = reg.Replace(fileName, string.Empty);
            cleanFileName = Regex.Replace(cleanFileName, @"\.{2,}", ".");

            return cleanFileName;
        }

        /// <summary>
        /// Obtiene la fecha actual y remplaza los caracteres /: y espacios por vacios
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDateTime()
        {
            string currentDate = DateTime.Now.ToString();
            currentDate = Regex.Replace(currentDate, @"[/:\s]", string.Empty);

            return currentDate;
        }
    }
}