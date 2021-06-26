using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Handler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Echeers.Mq.Data;
using Echeers.Mq.Models;
using Echeers.Mq.Models.ApiAddressModels;
using Aiursoft.WebTools;
using Aiursoft.XelNaga.Tools;
using Echeers.Mq.Models.ApiViewModels;

namespace Echeers.Mq.Controllers
{
    public class ApiController : Controller
    {
        private readonly UserManager<MqUser> _userManager;
        private readonly SignInManager<MqUser> _signInManager;
        private readonly ILogger _logger;
        private readonly MqDbContext _dbContext;

        public ApiController(
            UserManager<MqUser> userManager,
            SignInManager<MqUser> signInManager,
            ILoggerFactory loggerFactory,
            MqDbContext _context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ApiController>();
            _dbContext = _context;
        }

        public async Task<IActionResult> AccessToken(AccessTokenAddressModel model)
        {
            var app = await _dbContext.Apps.SingleOrDefaultAsync(t => t.Id == model.AppId && t.Secret == model.AppSecret);
            if (app == null)
            {
                return this.Protocol(ErrorType.Unauthorized, "Wrong app id or app secret!");
            }
            var newAC = new AccessToken
            {
                ApplyAppId = model.AppId,
                Value = (DateTime.Now.ToString() + HttpContext.GetHashCode().ToString() + model.AppId).GetMD5()
            };
            _dbContext.AccessTokens.Add(newAC);
            await _dbContext.SaveChangesAsync();

            return this.Protocol(new AccessTokenViewModel
            {
                Code = ErrorType.Success,
                Message = "Successfully get access token.",
                AccessToken = newAC.Value,
                DeadTime = newAC.CreateTime + newAC.AliveTime
            });
        }
    }
}