using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using log4net;
using Saludsa.UtilidadesRest;

namespace EjemploMFile
{
    internal class Program
    {
        #region Variables

        internal static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Variables

        private static void Main()
        {
            var current = Process.GetCurrentProcess();

            using (new Mutex(true, current.ProcessName, out var createdNew))
            {
                if (!createdNew)
                    return;

                try
                {
                    var cabecera = ObtnerCabecera();
                    var token = ObtenerToken();
                    if (token.IsNull())
                        goto Fin;

                    var auth = new AutorizacionRest(cabecera, token.Autorizacion);

                    var uri = AppConfig.ObtenerValorParametro("UriGestionDocumentos");

                    const string ruc = "0123456789001";

                    using (var cliente = ConsumirRest.CrearClienteApi(uri, auth))
                    {
                        //Buscar el contenido no documental
                        var con = BuscarNoDocumental(cliente, ruc);

                        var noDoc = new DTO.ContenidoNoDocumental();
                        if (con.IsNotNullOrEmpty())
                        {
                            noDoc.Id = con[0].Id;
                            noDoc.Guid = con[0].Guid;
                            noDoc.Titulo = con[0].Titulo;
                            noDoc.IdTipoObjeto = con[0].IdTipoObjeto;
                            noDoc.IdClase = con[0].IdClase;
                            noDoc.Version = con[0].Version;
                        }
                        else
                        {
                            //Crear contenido no documental
                            noDoc = CargarNoDocumental(cliente, ruc);
                            if (noDoc.IsNull())
                                goto Fin;
                        }

                        //Actualizar contenido no documental
                        noDoc.Titulo = $"{DateTime.Now:yyyy-MM-dd:HHmm}";
                        noDoc = ActualizarNoDocumental(cliente, noDoc);
                        if (noDoc.IsNull())
                            goto Fin;

                        //Cargar contenido documental
                        var conDoc = CargarDocumental(cliente, noDoc);
                        if (conDoc.IsNull())
                            goto Fin;

                        //Actualizar contenido documental
                        conDoc.Titulo = $"{DateTime.Now:yyyy-MM-dd:HHmm}";
                        conDoc = ActualizarDocumental(cliente, conDoc);
                        if (conDoc.IsNull())
                            goto Fin;

                        //Eliminar contenido documental
                        var res = EliminarDocumental(cliente, conDoc);
                        if (res == false)
                            goto Fin;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    Console.WriteLine(ex.Message);
                }

                Fin:
                Console.WriteLine("Presione cualquier tecla para finalizar.");
                Console.ReadKey();
            }
        }

        #region M-Files

        private static List<DTO.Contenido> BuscarNoDocumental(HttpClient cliente, string ruc)
        {
            var api = AppConfig.ObtenerValorParametro("ApiBuscar");

            var contenido = new
            {
                //Tipo de objeto: Prestador
                IdTipoObjeto = 108,
                //Clase: Prestador
                IdClase = 70,
                Propiedades = new[] {
                    new {
                        //Prestador RUC
                        Id = 1103,
                        Valor = (object)ruc,
                        Condicion = "Igual"
                    }
                }
            };

            var respuesta = ConsumirRest.ConsultarApiPost<List<DTO.Contenido>>(cliente, api, cuerpo: contenido);

            if (respuesta.EsOk())
            {
                Console.WriteLine("Contenido no documental encontrado");
                foreach (var d in respuesta.Datos)
                {
                    Console.WriteLine($"\tContenido no documental [{ruc}], version [{d.Version}]");
                }
                return respuesta.Datos;
            }

            Console.WriteLine($"Contenido no documental no encontrado [{ruc}]");
            foreach (var mensaje in respuesta.Mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return null;
        }

        private static DTO.ContenidoNoDocumental CargarNoDocumental(HttpClient cliente, string ruc)
        {
            var api = AppConfig.ObtenerValorParametro("ApiCargar");

            var authors = new[] {
                "Mahesh Chand", "Jeff Prosise", "Dave McCarter", "Allen O'neill",
                "Monica Rathbun", "Henry He", "Raj Kumar", "Mark Prime",
                "Rose Tracey", "Mike Crown" };

            //Ver la estructura "ContenedorClaseNoDocumental.json"

            var contenido = new
            {
                //Tipo de objeto: Prestador
                IdTipoObjeto = 108,
                //Clase: Prestador
                IdClase = 70,
                //Título del contenido
                Titulo = $"Prestador {ruc}",
                Propiedades = new[] {
                    new {
                        //Prestador Nombres
                        Id = 1104,
                        Valor = (object)authors[new Random().Next(authors.Length)]
                    },
                    new {
                        //Prestador Apellidos
                        Id = 1105,
                        Valor = (object)authors[new Random().Next(authors.Length)]
                    },
                    new {
                        //Prestador Número Convenio
                        Id = 1106,
                        Valor = (object)new Random().Next(100, 900)
                    },
                    new {
                        //Prestador RUC
                        Id = 1103,
                        Valor = (object)ruc
                    },
                    new {
                        //Prestador Tipo
                        Id = 1108,
                        Valor = (object)new Random().Next(1, 7)   //Consultar los valores de la lista no: 109
                    }
                }
            };

            var respuesta = ConsumirRest.ConsultarApiPut<DTO.ContenidoNoDocumental>(cliente, api, cuerpo: contenido);

            if (respuesta.EsOk())
            {
                Console.WriteLine($"Contenido no documental cargado, version [{respuesta.Datos.Version}]");
                return respuesta.Datos;
            }

            Console.WriteLine("Contenido no documental no cargado");
            foreach (var mensaje in respuesta.Mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return null;
        }

        private static DTO.ContenidoNoDocumental ActualizarNoDocumental(HttpClient cliente, DTO.ContenidoNoDocumental noDocumental)
        {
            var api = AppConfig.ObtenerValorParametro("ApiActualizar");

            var contenido = new
            {
                IdTipoObjeto = noDocumental.IdTipoObjeto,
                IdContenido = Convert.ToInt32(noDocumental.Id),
                //Se puede omitir la versión, si es 0 o nulo toma la última versión
                Version = noDocumental.Version,
                //Se puede omitir el título en caso de no querer actualizarlo
                Titulo = noDocumental.Titulo,
                //Se debe enviar solo las propiedades que requieran actualizar
                Propiedades = new[] {
                    new {
                        Id = 1106,
                        Valor = (object)new Random().Next(100, 900)
                    }
                }
            };

            var respuesta = ConsumirRest.ConsultarApiPut<DTO.ContenidoNoDocumental>(cliente, api, cuerpo: contenido);

            if (respuesta.EsOk())
            {
                Console.WriteLine($"Contenido no documental actualizado, version [{respuesta.Datos.Version}]");
                return respuesta.Datos;
            }

            Console.WriteLine("Contenido no documental no actualizado");
            foreach (var mensaje in respuesta.Mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return null;
        }

        private static DTO.ContenidoDocumental CargarDocumental(HttpClient cliente, DTO.ContenidoNoDocumental noDocumental)
        {
            var api = AppConfig.ObtenerValorParametro("ApiCargar");

            //Ver la estructura "ContenedorClaseDocumental.json"

            var path = Path.Combine(AppContext.BaseDirectory, "Archivos");

            var contenido = new
            {
                //Tipo de objeto: Documento
                IdTipoObjeto = 0,
                //Clase: Prestador Documentos
                IdClase = 71,
                //Título del contenido
                Titulo = $"Documentos {DateTime.Now:yyyy-MM-dd:HHmm}",
                Propiedades = new[] {
                    new {
                        //Se asocia el contenido documental al no documental
                        Id = 1102,
                        Valor = (object)Convert.ToInt32(noDocumental.Id)
                    }
                },
                Documentos = new[] {
                    new {
                        Nombre = "Cédula.pdf",
                        Contenido = File.ReadAllBytes(Path.Combine(path, "Doc1.pdf"))
                    },
                    new {
                        Nombre = "Ruc.png",
                        Contenido = File.ReadAllBytes(Path.Combine(path, "Doc2.png"))
                    }
                }
            };

            var respuesta = ConsumirRest.ConsultarApiPut<DTO.ContenidoDocumental>(cliente, api, cuerpo: contenido);

            if (respuesta.EsOk())
            {
                Console.WriteLine($"Contenido documental cargado, version [{respuesta.Datos.Version}]");
                foreach (var archivo in respuesta.Datos.Archivos)
                {
                    Console.WriteLine($"\tArchivo ID [{archivo.Id}], version [{archivo.Version}]");
                }
                return respuesta.Datos;
            }

            Console.WriteLine("Contenido documental no cargado");
            foreach (var mensaje in respuesta.Mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return null;
        }

        private static DTO.ContenidoDocumental ActualizarDocumental(HttpClient cliente, DTO.ContenidoDocumental documental)
        {
            var api = AppConfig.ObtenerValorParametro("ApiActualizar");

            var path = Path.Combine(AppContext.BaseDirectory, "Archivos");

            var contenido = new
            {
                IdTipoObjeto = documental.IdTipoObjeto,
                IdContenido = Convert.ToInt32(documental.Id),
                //Se puede omitir la versión, si es 0 o nulo toma la última versión
                VersionContenido = documental.Version,
                //Se puede omitir el título en caso de no querer actualizarlo
                Titulo = documental.Titulo,
                //Se debe enviar solo las propiedades que requieran actualizar
                //Propiedades = null,
                Documento = new
                {
                    Id = documental.Archivos[0].Id,
                    Nombre = "Cédula actualizado.docx",
                    Contenido = File.ReadAllBytes(Path.Combine(path, "Doc3.docx"))
                }
            };

            var respuesta = ConsumirRest.ConsultarApiPut<DTO.ContenidoDocumental>(cliente, api, cuerpo: contenido);

            if (respuesta.EsOk())
            {
                Console.WriteLine($"Contenido documental actualizado, version [{respuesta.Datos.Version}]");
                foreach (var archivo in respuesta.Datos.Archivos)
                {
                    Console.WriteLine($"\tArchivo ID [{archivo.Id}], version [{archivo.Version}]");
                }
                return respuesta.Datos;
            }

            Console.WriteLine("Contenido documental no actualizado");
            foreach (var mensaje in respuesta.Mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return null;
        }

        private static bool? EliminarDocumental(HttpClient cliente, DTO.ContenidoDocumental documental)
        {
            var api = AppConfig.ObtenerValorParametro("ApiEliminar");

            var contenido = new
            {
                IdTipoObjeto = documental.IdTipoObjeto,
                IdContenido = Convert.ToInt32(documental.Id),
                VersionContenido = documental.Version,
                IdArchivo = documental.Archivos[0].Id
            };

            var respuesta = ConsumirRest.ConsultarApiDelete<bool?>(cliente, api, cuerpo: contenido);

            if (respuesta.EsOk())
            {
                Console.WriteLine("Contenido documental eliminado");
                return respuesta.Datos;
            }

            Console.WriteLine("Contenido documental no eliminado");
            foreach (var mensaje in respuesta.Mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return false;
        }

        #endregion M-Files

        #region Token

        private static CabeceraServicioRest ObtnerCabecera()
        {
            const string codigoAplicacion = "INTRANET";
            const string codigoPlataforma = "SRVCWIN";
            const string sistemaOperativo = "Microsoft Windows 10 Versión 1909";
            const string dispositivoNavegador = "Aplicación windows";
            const string direccionIp = "1.1.1.1";

            return Rest.CrearCabecera(codigoAplicacion, codigoPlataforma, sistemaOperativo, dispositivoNavegador, direccionIp);
        }

        private static TokenRest ObtenerToken()
        {
            var auth = Rest.ObtenerToken("usrbaytec", "T3mporal", AppConfig.ObtenerValorParametro("ClientId"), out var token, out var mensajes);
            if (auth.IsNotNull())
            {
                Console.WriteLine("Token obtenido");
                return token;
            }

            Console.WriteLine("Problemas al obtener token");
            foreach (var mensaje in mensajes)
            {
                Console.WriteLine("\t" + mensaje);
            }

            return null;
        }

        #endregion Token
    }
}

#region DTO

namespace EjemploMFile.DTO
{
    internal class Ver
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public int Version { get; set; }
    }

    internal class Contenido : Ver
    {
        public int IdTipoObjeto { get; set; }
        public int IdClase { get; set; }

        public string Titulo { get; set; }
    }

    internal class ContenidoNoDocumental : Contenido
    {
    }

    internal class ContenidoDocumental : Contenido
    {
        public List<Archivo> Archivos { get; set; }

        internal class Archivo : Ver
        {
            public string Nombre { get; set; }
            public string Extension { get; set; }
        }
    }
}

#endregion DTO