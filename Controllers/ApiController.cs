using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Aiursoft.Pylon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using Echeers.Mq.Data;
using Echeers.Mq.Models;
using Echeers.Mq.Models.ApiAddressModels;
using Aiursoft.Pylon.Services;
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
                return Json(new AiurProtocal
                {
                    code = ErrorType.Unauthorized,
                    message = "Wrong app id or app secret!"
                });
            }
            var newAC = new AccessToken
            {
                ApplyAppId = model.AppId,
                Value = (DateTime.Now.ToString() + HttpContext.GetHashCode().ToString() + model.AppId).GetMD5()
            };
            _dbContext.AccessTokens.Add(newAC);
            await _dbContext.SaveChangesAsync();
            return Json(new AccessTokenViewModel
            {
                code = ErrorType.Success,
                message = "Successfully get access token.",
                AccessToken = newAC.Value,
                DeadTime = newAC.CreateTime + newAC.AliveTime
            });
        }
    }
}