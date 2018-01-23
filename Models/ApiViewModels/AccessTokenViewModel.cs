using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Models;

namespace Echeers.Mq.Models.ApiViewModels
{
    public class AccessTokenViewModel : AiurProtocal
    {
        public virtual string AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}
