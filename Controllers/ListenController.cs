using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Handler.Models;
using Echeers.Mq.Data;
using Echeers.Mq.Services;
using Aiursoft.WebTools;
using Echeers.Mq.Models.ListenAddressModels;

namespace Echeers.Mq.Controllers
{
    public class ListenController : Controller
    {
        private MqDbContext _dbContext;
        private WebSocketPusher _pusher;

        public ListenController(MqDbContext dbContext,
            WebSocketPusher pusher)
        {
            _dbContext = dbContext;
            _pusher = pusher;
        }

        public async Task<IActionResult> Channel(ChannelAddressModel model)
        {
            var lastReadTime = DateTime.Now;
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel.ConnectKey != model.Key)
            {
                return this.Protocol(ErrorType.Unauthorized, "Wrong connection key!");
            }
            await _pusher.Accept(HttpContext);
            int sleepTime = 0;
            while (_pusher.Connected)
            {
                try
                {
                    var nextMessages = MqMemoryContext
                        .Messages
                        .Where(t => t.ChannelId == model.Id)
                        .Where(t => t.CreateTime > lastReadTime)
                        .ToList();
                    if (!nextMessages.Any())
                    {
                        if (sleepTime < 300)
                            sleepTime += 5;
                        await Task.Delay(sleepTime);
                    }
                    else
                    {
                        var nextMessage = nextMessages.OrderBy(t => t.CreateTime).First();
                        await _pusher.SendMessage(nextMessage.Content);
                        lastReadTime = nextMessage.CreateTime;
                        sleepTime = 0;
                    }
                }
                catch (InvalidOperationException)
                {

                }
            }
            return null;
        }
    }
}