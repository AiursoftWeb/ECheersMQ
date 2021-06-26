using System.Collections.Generic;
using Aiursoft.Handler.Models;

namespace Echeers.Mq.Models.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<Channel> Channel { get; set; }
    }
}
