using System;
using Aiursoft.Handler.Models;

namespace Echeers.Mq.Models.ApiViewModels
{
    public class AccessTokenViewModel : AiurProtocol
    {
        public virtual string AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}
