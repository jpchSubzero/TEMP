using System.Collections.Generic;
using satelite.Models;

namespace satelite.Controllers
{
    public class ManagerDataOss : AbstManagerData
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

            int limitSize = ManagerConfigOss.GetLimiteSizeFile();
            string allowedTypeFile = ManagerConfigOss.GetAllowedTypeFile();


            if ( !ValidateSizeFile(sizeFile, limitSize) )
            {
                fileAllowed.Add(false, ErrorMessageDAO.SIZE_FILE_NO_ALLOWED);
                return fileAllowed;
            }

            if ( !ValidateFileType( typeFile,  allowedTypeFile))
            {
                fileAllowed.Add(false, ErrorMessageDAO.TYPE_FILE_NO_ALLOWED);
                return fileAllowed;
            }

            fileAllowed.Add(true, "");
            return fileAllowed;
        }
                

    }
}