using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Headers;
using System.Net.Http.Headers;
namespace Submit_System
{
    public class UserController : AbstractController 
    {  
        private const string PASSWORD_LINK = "https://localhost:5000/SetPassword?token={0}";
        private readonly TokenStorage _storage;
        public UserController(TokenStorage storage, DatabaseAccess access) : base(access)
        {
            _storage = storage;
        }
        [HttpPost]  
        [Route("User/Login")]
        public ActionResult<List<string>> Login([FromBody]LoginData login)  
        {  
            if (login == null || login?.Username == null || login?.Password == null)  
            {  
                return BadRequest();
            }
            
            var result = _access.AuthenticateUser(login.Username, login.Password);
            if(result.Item1 == null) {
                return NotFound();
            }
            string id = _storage.CreateToken(login.Username, false);
            var options = new CookieOptions {
                HttpOnly = true,
                SameSite = SameSiteMode.None, // Only to make testing easier
                Secure = true
            };
            HttpContext.Response.Cookies.Append("token", id, options);
            return new List<string> { "obsolete", result.Item1};
        }
        [Route("User/SetPassword")]
        [HttpPost]
        public IActionResult SetPassword(string token, [FromBody] string password)
        {
            
            (string userid, DBCode code) = _access.CheckPasswordToken(CryptoUtils.Sha256Hash(token));
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            _storage.RemoveByID(userid);
            code = _access.DeletePasswordToken(userid);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            code = _access.SetPassword(userid, CryptoUtils.KDFHash(password));
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            return Ok();
        }
        [HttpDelete]
        [Route("User/Logout")]
        public IActionResult Logout(string token) {
            token = token ?? Request.Cookies["token"];
            _storage.RemoveToken(token);
            return Ok();
        }
        [HttpHead]
        [Route("User/CheckToken")]
        public IActionResult CheckToken(string token)
        {
            token = Request.Cookies["token"];
            if(_storage.TryGetToken(token, out _))
            {
                return Ok();
            }
            return NotFound();
        }
        [HttpPut]
        [Route("User/Unlock")]
        public IActionResult TurnOff()
        {
            _storage._isTestMode = true;
            return Ok("Authentication disabled.");
        }
        [HttpPut]
        [Route("User/Lock")]
        public IActionResult TurnOn()
        {
            _storage._isTestMode = false;
            return Ok("Authentication enabled.");
        }
        [HttpPost]
        [Route("Admin/AddUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            user.PasswordHash = null;
            DataBaseManager.AddUser(user);
            string token = CryptoUtils.GetRandomBase64String(24);
            string tokenHash = CryptoUtils.Sha256Hash(token);
            string link =  String.Format(PASSWORD_LINK, HttpUtility.UrlEncode(token));
            _access.AddPasswordToken(user.ID, tokenHash, DateTime.Now.AddDays(2));
            string text = $"Password form link:<br>" + link;
            MaleUtils.SendRegistration(user.Email, text);
            return Ok();
        }
    }  
}