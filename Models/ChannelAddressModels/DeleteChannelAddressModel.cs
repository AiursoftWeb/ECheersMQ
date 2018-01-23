using System;
using System.Collections.Generic;
using System.Text;

namespace ECheers.Mq.Models.ChannelAddressModels
{
    public class DeleteChannelAddressModel
    {
        public string AccessToken { get; set; }
        public int ChannelId { get; set; }
    }
}
