using System.Threading.Tasks;

namespace LeerMensajesActiveMQ.Remote_Services.ActiveMQ
{
    public interface IActiveMQ
    {
        Task ReadMessageQueue();
    }
}