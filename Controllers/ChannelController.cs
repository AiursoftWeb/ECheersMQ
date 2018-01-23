// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Echeers.Mq.Data;
// using ECheers.Mq.Models.ChannelAddressModels;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;


// namespace Echeers.Mq.Controllers
// {
//     public class ChannelController : Controller
//     {
//         private MqDbContext _dbContext;
//         public ChannelController(MqDbContext dbContext)
//         {
//             _dbContext = dbContext;
//         }

//         public async Task<IActionResult> ViewMyChannels(ViewMyChannelsAddressModel model)
//         {
//             var channels = await _dbContext
//                 .Channels
//                 .Where(t => t.AppId == app.AppId)
//                 .ToListAsync();
//             var viewModel = new ViewMyChannelsViewModel
//             {
//                 AppId = appLocal.Id,
//                 Channel = channels,
//                 code = ErrorType.Success,
//                 message = "Successfully get your channels!"
//             };
//             return Json(viewModel);
//         }

//         public async Task<IActionResult> ValidateChannel(ChannelAddressModel model)
//         {
//             var channel = await _dbContext.Channels.FindAsync(model.Id);
//             if (channel == null)
//             {
//                 return Json(new AiurProtocal
//                 {
//                     code = ErrorType.NotFound,
//                     message = "Can not find your channel!"
//                 });
//             }
//             if (channel.ConnectKey != model.Key)
//             {
//                 return Json(new AiurProtocal
//                 {
//                     code = ErrorType.Unauthorized,
//                     message = "Wrong connection key!"
//                 });
//             }
//             else
//             {
//                 return Protocal(ErrorType.Success, "Current Info.");
//             }
//         }

//         [HttpPost]
//         public async Task<IActionResult> CreateChannel([FromForm]CreateChannelAddressModel model)
//         {
//             //Update app info
//             var app = await ApiService.ValidateAccessTokenAsync(model.AccessToken);
//             var appLocal = await _dbContext.Apps.Include(t => t.Channels).SingleOrDefaultAsync(t => t.Id == app.AppId);
//             if (appLocal == null)
//             {
//                 appLocal = new StargateApp
//                 {
//                     Id = app.AppId,
//                     Channels = new List<Channel>()
//                 };
//                 _dbContext.Apps.Add(appLocal);
//             }
//             //Create and save to database
//             var newChannel = new Channel
//             {
//                 Description = model.Description,
//                 ConnectKey = StringOperation.RandomString(20)
//             };
//             appLocal.Channels.Add(newChannel);
//             await _dbContext.SaveChangesAsync();
//             //return model
//             var viewModel = new CreateChannelViewModel
//             {
//                 ChannelId = newChannel.Id,
//                 ConnectKey = newChannel.ConnectKey,
//                 code = ErrorType.Success,
//                 message = "Successfully created your channel!"
//             };
//             return Json(viewModel);
//         }

//         [HttpPost]
//         public async Task<IActionResult> DeleteChannel([FromForm]DeleteChannelAddressModel model)
//         {
//             var app = await ApiService.ValidateAccessTokenAsync(model.AccessToken);
//             var channel = await _dbContext.Channels.FindAsync(model);
//             if (channel.AppId != app.AppId)
//             {
//                 return Json(new AiurProtocal { code = ErrorType.Unauthorized, message = "The channel you try to delete is not your app's channel!" });
//             }
//             _dbContext.Channels.Remove(channel);
//             await _dbContext.SaveChangesAsync();
//             return Json(new AiurProtocal { code = ErrorType.Success, message = "Successfully deleted your channel!" });
//         }

//         /// <summary>
//         /// This action will delete all channels he created!
//         /// </summary>
//         /// <param name="model"></param>
//         /// <returns></returns>
//         [HttpPost]
//         public async Task<IActionResult> DeleteApp([FromForm]DeleteAppAddressModel model)
//         {
//             var app = await ApiService.ValidateAccessTokenAsync(model.AccessToken);
//             if (app.AppId != model.AppId)
//             {
//                 return Json(new AiurProtocal { code = ErrorType.Unauthorized, message = "The app you try to delete is not the accesstoken you granted!" });
//             }
//             var target = await _dbContext.Apps.FindAsync(app.AppId);
//             if (target != null)
//             {
//                 _dbContext.Channels.Delete(t => t.AppId == target.Id);
//                 _dbContext.Apps.Remove(target);
//                 await _dbContext.SaveChangesAsync();
//                 return Json(new AiurProtocal { code = ErrorType.Success, message = "Successfully deleted that app and all channels." });
//             }
//             return Json(new AiurProtocal { code = ErrorType.HasDoneAlready, message = "That app do not exists in our database." });
//         }
//     }
// }