using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Aiursoft.Handler.Models;
using Echeers.Mq.Data;
using Echeers.Mq.Models.MessageAddressModels;
using Aiursoft.WebTools;
using Echeers.Mq.Models;

namespace Echeers.Mq.Controllers
{
    public class MessageController : Controller
    {
        private MqDbContext _dbContext;
        public MessageController(MqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> PushMessage(PushMessageAddressModel model)
        {
            var token = await _dbContext.AccessTokens.Include(t => t.ApplyApp).SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (token == null || token.ApplyApp == null)
            {
                return this.Protocol(ErrorType.Unauthorized, "Invalid accesstoken!");
            }
            //Ensure channel
            var channel = await _dbContext.Channels.SingleOrDefaultAsync(t => t.Id == model.ChannelId);
            if (channel == null)
            {
                return this.Protocol(ErrorType.NotFound, "We can not find your channel!");
            }
            if (channel.AppId != token.ApplyAppId)
            {
                return this.Protocol(ErrorType.Unauthorized, "The channel you wanna create message is not your app's channel!");
            }
            //Create Message
            var message = new Message
            {
                Id = Startup.MessageIdCounter.GetUniqueNo(),
                ChannelId = channel.Id,
                Content = model.MessageContent
            };
            MqMemoryContext.Messages.Add(message);

            return this.Protocol(ErrorType.Success, $"You have successfully created a message at channel:{channel.Id}!");
        }
    }
}