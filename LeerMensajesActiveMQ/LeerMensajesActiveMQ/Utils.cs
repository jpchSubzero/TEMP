using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeerMensajesActiveMQ
{
    public class Utils
    {
        /// <summary>
        /// Obtiene los datos de los encabezados del mensaje sin los prefijos
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDataForm(Dictionary<string, string> headers)
        {
            Dictionary<string, string> dataForm = new Dictionary<string, string>();

            Dictionary<string, string> metadata = new Dictionary<string, string>();

            

            foreach (var item in headers)
            {
                if (item.Key.StartsWith(Constants.PrefixMetatada))
                {
                    string newKey = item.Key.Substring(Constants.LengthPrefix);
                    metadata[newKey] = item.Value;
                }
                if (item.Key.StartsWith(Constants.PrefixDataForm))
                {
                    string newKey = item.Key.Substring(Constants.LengthPrefix);
                    dataForm[newKey] = item.Value;
                }
            }

            var jsonMetadata = JsonConvert.SerializeObject(metadata);
            dataForm[Constants.ParamMetadata] = jsonMetadata;

            return dataForm;
        }
    }
}
