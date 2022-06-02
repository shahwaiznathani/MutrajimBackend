using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MutrajimAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MutrajimAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }
        //POST : api/ApplicationUser/UserRegister //To Post User Register Info in DB
        [HttpPost]
        [Route("UserRegister")]
        public async Task<Object> PostApplicationUser(ApplicationUserDTO model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex });
            }
        }
        //POST: api/ApplicationUser/Login   //Authenticate Login and provide Token
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> LoginFunction(LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString())
                    }),
                    //Minutes for token expiration
                    Expires = DateTime.UtcNow.AddMinutes(25),
                    SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                var Id = user.Id;
                HttpContext.Session.SetString("UserId", Id);
                //return token and userid upon login
                return Ok(new { token , Id});
            }
            else
            {
                return BadRequest(new { message = "Username or Password is incorrect" });
            }
        }
        //Store Id to session sotrage and fetch from user
        [HttpGet]
        [Route("UserFromSession")]
        public async Task<ActionResult<ApplicationUser>> GetUserFromSession(string id)
        {
            //change id here to work
            //var sessionuserId = HttpContext.Session.GetString("UserId");
            var currentUser = await _userManager.FindByIdAsync(id);

            if (currentUser == null)
            {
                return NotFound();
            }

            return currentUser;
        }
        //Get User Details Using UserName
        [HttpGet]
        [Route("GetByName")]
        public async Task<ActionResult<ApplicationUser>> GetUserByName(string uname)
        {
            var currentUser = await _userManager.FindByNameAsync(uname);

            if (currentUser == null)
            {
                return NotFound();
            }

            return currentUser;
        }

        //Patch api used as post
        [HttpPost]
        [Route("UpdateFileId")]
        public async Task<ActionResult<ApplicationUser>> UpdateFileId(UpdateFileIdDTO dto)
        {
            var currentUser = await _userManager.FindByIdAsync(dto.UserId);

            if (currentUser == null)
            {
                return NotFound();
            }
            currentUser.fileID = dto.FileId;
            await _userManager.UpdateAsync(currentUser);

            var isValid = TryValidateModel(currentUser);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            return currentUser;
        }

        //Patch api used as post 
        [HttpPost]
        [Route("UpdateLocaleId")]
        public async Task<ActionResult<ApplicationUser>> UpdateSettingsId(UpdateLocaleIdDTO dto)
        { 
            var currentUser = await _userManager.FindByIdAsync(dto.UserId);

            if (currentUser == null)
            {
                return NotFound();
            }
            currentUser.settingId = dto.LocaleId;
            await _userManager.UpdateAsync(currentUser);

            var isValid = TryValidateModel(currentUser);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            return currentUser;
        }
    }
}
