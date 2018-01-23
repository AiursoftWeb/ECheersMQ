using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECheers.Mq.Models.ListenAddressModels
{
    public class ChannelAddressModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
    }
}
