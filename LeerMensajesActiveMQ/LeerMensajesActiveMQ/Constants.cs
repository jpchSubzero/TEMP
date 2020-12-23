using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeerMensajesActiveMQ
{
    public class Constants
    {
        /// <summary>
        /// Prefijo del encabezado del mensaje que tiene los metadatos
        /// </summary>
        public const string PrefixMetatada = "_met_";
        /// <summary>
        /// Prefijo del encabezado del mensaje que tiene los parametros de la request
        /// </summary>
        public const string PrefixDataForm = "_dfr_";
        /// <summary>
        /// Nombre del parametro para enviar la metadata
        /// </summary>
        public const string ParamMetadata = "Metadata";
        /// <summary>
        /// Tamaño del prefijo de las propiedades del mensaje
        /// </summary>
        public const int LengthPrefix = 5;
        /// <summary>
        /// Propiedad del mensaje que tiene el nombre del archivo
        /// </summary>
        public const string NameFile = "_dfi_Name";

    }
}
