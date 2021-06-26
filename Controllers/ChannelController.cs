using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Handler.Models;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Tools;
using Echeers.Mq.Data;
using Echeers.Mq.Models;
using Echeers.Mq.Models.ChannelAddressModels;
using Echeers.Mq.Models.ChannelViewModels;
using Echeers.Mq.Models.ListenAddressModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Echeers.Mq.Controllers
{
    public class ChannelController : ControllerBase
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
                return this.Protocol(ErrorType.Unauthorized, "Invalid accesstoken!");
            }
            var channels = await _dbContext
                .Channels
                .Where(t => t.AppId == token.ApplyAppId)
                .ToListAsync();

            return this.Protocol(new ViewMyChannelsViewModel
            {
                AppId = token.ApplyAppId,
                Channel = channels,
                Code = ErrorType.Success,
                Message = "Successfully get your channels!"
            });
        }

        public async Task<IActionResult> ValidateChannel(ChannelAddressModel model)
        {
            var channel = await _dbContext.Channels.FindAsync(model.Id);
            if (channel == null)
            {
                return this.Protocol(ErrorType.NotFound, "Can not find your channel!");
            }
            if (channel.ConnectKey != model.Key)
            {
                return this.Protocol(ErrorType.Unauthorized, "Wrong connection key!");
            }
            else
            {
                return this.Protocol(ErrorType.Success, "Corrent Info.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromForm]CreateChannelAddressModel model)
        {
            var token = await _dbContext.AccessTokens.Include(t => t.ApplyApp).SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (token == null || token.ApplyApp == null)
            {
                return this.Protocol(ErrorType.Unauthorized, "Invalid accesstoken!");
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

            return this.Protocol(new CreateChannelViewModel
            {
                ChannelId = newChannel.Id,
                ConnectKey = newChannel.ConnectKey,
                Code = ErrorType.Success,
                Message = "Successfully created your channel!"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChannel([FromForm]DeleteChannelAddressModel model)
        {
            var token = await _dbContext.AccessTokens.Include(t => t.ApplyApp).SingleOrDefaultAsync(t => t.Value == model.AccessToken);
            if (token == null || token.ApplyApp == null)
            {
                return this.Protocol(ErrorType.Unauthorized, "Invalid accesstoken!");
            }
            var channel = await _dbContext.Channels.FindAsync(model);
            if (channel.AppId != token.ApplyAppId)
            {
                this.Protocol(ErrorType.Unauthorized, "The channel you try to delete is not your app's channel!");
            }
            _dbContext.Channels.Remove(channel);
            await _dbContext.SaveChangesAsync();
            
            return this.Protocol(ErrorType.Success, "Successfully deleted your channel!");
        }
    }
}