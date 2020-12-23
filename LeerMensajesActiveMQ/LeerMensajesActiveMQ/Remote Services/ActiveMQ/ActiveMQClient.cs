using Apache.NMS;
using Apache.NMS.Util;
using LeerMensajesActiveMQ.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeerMensajesActiveMQ.Remote_Services.ActiveMQ
{
    class ActiveMQClient : IActiveMQ
    {
        private readonly IuploadFile _uploadFile;

        public ActiveMQClient() {
             _uploadFile = new UploadFile();
        }

        public ActiveMQClient (IuploadFile uploadFile)
        {
            _uploadFile = uploadFile;
        }

        public async  Task ReadMessageQueue()
        {
            TimeSpan receiveTimeout = TimeSpan.FromSeconds(5);
            string uriActiveMQ = ConfigurationSettings.AppSettings.Get("URL_ACTIVEMQ");
            Uri connecturi = new Uri(uriActiveMQ);
            string destinationName = ConfigurationSettings.AppSettings.Get("QUEUE_NAME"); ;

            try
            {
                IConnectionFactory factory = new NMSConnectionFactory(connecturi);

                using (IConnection connection = factory.CreateConnection())
                using (ISession session = connection.CreateSession())
                {
                    IDestination destination = SessionUtil.GetDestination(session, destinationName);
                    // Create a consumer 
                    using (IMessageConsumer consumer = session.CreateConsumer(destination))
                    {
                        // Start the connection so that messages will be processed.
                        connection.Start();

                        // Consume a message
                        //Unit for time
                        var message = consumer.Receive(receiveTimeout);

                        if (message == null)
                        {
                            Console.WriteLine($"No hay mensajes para leer en la cola");
                            return;
                        }

                        var streamMessageRead = message as IBytesMessage;
                        Dictionary<string, string> headers = new Dictionary<string, string>();
                        var keys = streamMessageRead.Properties.Keys;
                        var content = streamMessageRead.Content;
                        //var length = streamMessageRead.BodyLength;
                        var properties = streamMessageRead.Properties;

                        foreach (var item in keys)
                        {
                            var key = item.ToString();
                            headers[key] = properties[key].ToString();
                        }

                        var dataForm = Utils.GetDataForm(headers);
                        byte[] buffer = new byte[16 * 1024];

                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = streamMessageRead.ReadBytes(buffer)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            //You have to rewind the MemoryStream before copying
                            ms.Seek(0, SeekOrigin.Begin);
                            //Se invoca a la Api que sube el archivo a Sharepoint
                            var result = await _uploadFile.Upload(dataForm, ms.GetBuffer(), headers["_dfi_Name"]);
                            Console.WriteLine($"Se ha ledido un archivo: {await result.Content.ReadAsStringAsync()}");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ha ocurrido una excepcion: {ex.Message}");
            }
        }
    }
}
