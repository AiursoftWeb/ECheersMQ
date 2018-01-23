using System;
using System.Collections.Generic;
using System.Text;
using Aiursoft.Pylon.Models;
using Echeers.Mq.Models;

namespace ECheers.Mq.Models.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocal
    {
        public string AppId { get; set; }
        public IEnumerable<Channel> Channel { get; set; }
    }
}
