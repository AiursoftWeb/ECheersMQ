using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Echeers.Mq.Data;
using Echeers.Mq.Services;
using Aiursoft.Pylon.Models;
using Echeers.Mq.Models.ListenAddressModels;

namespace Echeers.Mq.Controllers
{
    public class ListenController : AiurController
    {
        private MqDbContext _dbContext;
        private IPusher<WebSocket> _pusher;
        private DataCleaner _cleaner;

        public ListenController(MqDbContext dbContext,
            WebSocketPusher pusher,
            DataCleaner cleaner)
        {
            _dbContext = dbContext;
            _pusher = pusher;
            _cleaner = cleaner;
        }

        [AiurForceWebSocket]
        public async Task<IActionResult> Channel(ChannelAddressModel model)
        {
            await _cleaner.StartCleanerService();
            var lastReadTime = DateTime.Now;
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel.ConnectKey != model.Key)
            {
                return Json(new AiurProtocal
                {
                    code = ErrorType.Unauthorized,
                    message = "Wrong connection key!"
                });
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
                    Console.WriteLine(DateTime.Now.Millisecond + "Checked!");
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