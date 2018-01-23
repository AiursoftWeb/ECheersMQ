using System;
using System.Collections.Generic;
using System.Text;
using Aiursoft.Pylon.Models;

namespace ECheers.Mq.Models.ChannelViewModels
{
    public class CreateChannelViewModel : AiurProtocal
    {
        public int ChannelId { get; set; }
        public string ConnectKey { get; set; }
    }
}
