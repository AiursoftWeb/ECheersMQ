using System;
using System.Collections.Generic;
using System.Text;

namespace Echeers.Mq.Models.MessageAddressModels
{
    public class PushMessageAddressModel
    {
        public string AccessToken { get; set; }
        public int ChannelId { get; set; }
        public string MessageContent { get; set; }
    }
}
