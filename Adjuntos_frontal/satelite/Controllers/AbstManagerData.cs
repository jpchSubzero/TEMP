using System;
using System.Collections.Generic;
using log4net;
using System.Web;
using satelite.Models;

namespace satelite.Controllers
{
    public abstract class AbstManagerData
    {
        /// <summary>
        /// Valida el tamaño del archivo.
        /// </summary>
        /// <param name="sizeFile">El tamaño del archivo.</param>
        /// <param name="limitSize">El limite del peso permitido.</param>
        /// <returns>Retorna verdadero o falso</returns>
        public static bool ValidateSizeFile(int sizeFile, int limitSize)
        {
            return (sizeFile <= limitSize);
        }


        /// <summary>
        /// Falida el tipo de archivo.
        /// </summary>
        /// <param name="typeFile">El tipo del archivo</param>
        /// <param name="validTypes">Tipo de archivo permitido.</param>
        /// <returns>Retorna verdadero o falso</returns>
        public static bool ValidateFileType(string typeFile, string validTypes)
        {
            string lowerType = typeFile.ToLower();

            validTypes = validTypes.Replace(" ", "");
            string[] allowedList = validTypes.Split(',');
            return Array.Exists(allowedList, element => element == lowerType);
        }


        /// <summary>
        /// Recupera el codigo interno decodificando la informacin desde la URL usando 
        /// el algorito AES en formato de 32 bytes
        /// </summary>
        /// <param name="codedData">La informacion codificada</param>
        /// <param name="Log">El elemento log para registrar sucesos</param>
        /// <param name="origin">El origen de invocacion del metodo para registrar en el log</param>
        /// <returns>Dicionario con valor de llave Verdadero y un string con los datos decodificdos o 
        /// llave con valor Falso y con datos indicando el error ocurrido</returns>
        public static IDictionary<bool, string> GetInternalDecodedData(string codedData, ILog Log, string origin)
        {
            IDictionary<bool, string> decodedData = new Dictionary<bool, string>();
            var originalData = codedData;
            codedData = (codedData != null || codedData.Length >= 0) ? HttpUtility.UrlDecode(codedData) : codedData;
            codedData = codedData.Replace(' ', '+');

            if (codedData == null || codedData.Length <= 0)
            {
                decodedData.Add(false, ErrorMessageDAO.NO_REQUEST_DATA);
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError("NO SE RECUPERO DATOS DE LA URL DE SOLICITUD", "SIN DATOS", origin + "/AbstManagerData/GetInernalDecoded/"));
                return decodedData;
            }

            string[] listCodedData = codedData.Split('$');
            string encodeServiceData = listCodedData[0];
            string vector = listCodedData[1];
            string key = listCodedData[2];

            try
            {
                ManagedAes managedAes = new ManagedAes(vector, key);
                string decodeInternalData = managedAes.DecryptText(encodeServiceData);
                decodedData.Add(true, decodeInternalData);
            }
            catch (Exception error)
            {
                decodedData.Add(false, ErrorMessageDAO.ERROR_DECODE_DATA_AES);
                Log.Info(ErrorMessageDAO.GetGenerirErrorLogError(error.Message, originalData + " | Decodificado: " + codedData, origin + "/AbstManagerData/GetInernalDecoded/AES"));
                return decodedData;
            }

            return decodedData;
        }


    }
}