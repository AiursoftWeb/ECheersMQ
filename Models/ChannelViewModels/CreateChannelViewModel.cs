using Aiursoft.Handler.Models;

namespace Echeers.Mq.Models.ChannelViewModels
{
    public class CreateChannelViewModel : AiurProtocol
    {
        public int ChannelId { get; set; }
        public string ConnectKey { get; set; }
    }
}
