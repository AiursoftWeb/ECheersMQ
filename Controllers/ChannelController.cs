using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Echeers.Mq.Data;
using Echeers.Mq.Models;
using Echeers.Mq.Models.ChannelAddressModels;
using Echeers.Mq.Models.ChannelViewModels;
using Echeers.Mq.Models.ListenAddressModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Echeers.Mq.Controllers
{
    public class ChannelController : AiurController
    {
        private MqDbContext _dbContext;
        public ChannelController(MqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> ViewMyChannels(ViewMyChannelsAddressModel model)
        {
            var token = await _dbContext.AccessTokens.Include(t => t.ApplyApp).SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (token == null || token.ApplyApp == null)
            {
                return Protocal(ErrorType.Unauthorized, "Invalid accesstoken!");
            }
            var channels = await _dbContext
                .Channels
                .Where(t => t.AppId == token.ApplyAppId)
                .ToListAsync();
            var viewModel = new ViewMyChannelsViewModel
            {
                AppId = token.ApplyAppId,
                Channel = channels,
                code = ErrorType.Success,
                message = "Successfully get your channels!"
            };
            return Json(viewModel);
        }

        public async Task<IActionResult> ValidateChannel(ChannelAddressModel model)
        {
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel == null)
            {
                return Json(new AiurProtocal
                {
                    code = ErrorType.NotFound,
                    message = "Can not find your channel!"
                });
            }
            if (channel.ConnectKey != model.Key)
            {
                return Json(new AiurProtocal
                {
                    code = ErrorType.Unauthorized,
                    message = "Wrong connection key!"
                });
            }
            else
            {
                return Protocal(ErrorType.Success, "Current Info.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromForm]CreateChannelAddressModel model)
        {
            var token = await _dbContext.AccessTokens.Include(t => t.ApplyApp).SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (token == null || token.ApplyApp == null)
            {
                return Protocal(ErrorType.Unauthorized, "Invalid accesstoken!");
            }
            //Create and save to database
            var newChannel = new Channel
            {
                Description = model.Description,
                ConnectKey = StringOperation.RandomString(20),
                AppId = token.ApplyAppId
            };
            _dbContext.Channels.Add(newChannel);
            await _dbContext.SaveChangesAsync();
            //return model
            var viewModel = new CreateChannelViewModel
            {
                ChannelId = newChannel.Id,
                ConnectKey = newChannel.ConnectKey,
                code = ErrorType.Success,
                message = "Successfully created your channel!"
            };
            return Json(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChannel([FromForm]DeleteChannelAddressModel model)
        {
            var token = await _dbContext.AccessTokens.Include(t => t.ApplyApp).SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (token == null || token.ApplyApp == null)
            {
                return Protocal(ErrorType.Unauthorized, "Invalid accesstoken!");
            }
            var channel = await _dbContext.Channels.FindAsync(model);
            if (channel.AppId != token.ApplyAppId)
            {
                return Json(new AiurProtocal { code = ErrorType.Unauthorized, message = "The channel you try to delete is not your app's channel!" });
            }
            _dbContext.Channels.Remove(channel);
            await _dbContext.SaveChangesAsync();
            return Json(new AiurProtocal { code = ErrorType.Success, message = "Successfully deleted your channel!" });
        }
    }
}