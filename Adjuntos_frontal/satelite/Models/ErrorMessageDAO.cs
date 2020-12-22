

namespace satelite.Models
{
    public class ErrorMessageDAO
    {
        public static string FILE_NOT_FOUNT = "Error, necesita adjuntar un archivo para continuar.";
        public static string TYPE_FILE_NO_ALLOWED = "Error, el formato del archivo adjuntado no esta permitido.";
        public static string SIZE_FILE_NO_ALLOWED = "Error, el tamaño del archivo adjuntado supera el máximo permitido.";
        public static string NO_REQUEST_DATA = "Error en los datos enviados, no se pudo recuperar la información para procesar la solicitud.";
        public static string ERROR_DECODE_DATA_AES = "Error en los datos enviados, no se pudo recuperar adecuadamente la información para procesar la solicitud.";
        public static string ERROR_GENERATE_COOKI = "Error interno en la obtención de permisos.";
        public static string ERROR_UPLOAD_API = "Error, ha sucedido un inconveniente inesperado mientras se procesaba tu adjunto, favor intentar más tarde.";
        public static string ERROR_GET_REQUEST_API = "Error, ha sucedido un inconveniente inesperado mientras se intentaba recuperar tus requisitos, favor intentar más tarde.";
        public static string ERROR_POST_FILE_API = "Error, ha sucedido un inconveniente inesperado mientras se intentaba enviar tu documento, favor intentar más tarde.";
        public static string ERROR_INTERMINTATE_DESCRIPTION = "Error, no se pudo determinar al descripción de la beca solicitada, favor consulte con el administrador.";
        public static string ERROR_INTERMINTATE_STATE = "Error, no se pudo determinar el estado de su solicitud, favor consulte con el administrador.";

               
        /// <summary>
        /// Obtiene el mensaje de error a guardar en el log.
        /// </summary>
        /// <param name="error">El error generado.</param>
        /// <param name="data">Los datos.</param>
        /// <param name="origin">El origen.</param>
        /// <returns></returns>
        public static string GetGenerirErrorLogError(string error, string data, string origin)
        {
            string messaje = "ERROR: " + error + " | DATOS: " + data + " | ORIGEN: " + origin;
            return messaje;
        }


    }
}