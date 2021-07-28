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
            
            // // (string userid, DBCode code) = _access.CheckPasswordToken(form.Token);
            // // _access.DeletePasswordToken(userid);
            //_storage.RemoveByID(userid);
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
            token = token ?? Request.Cookies["token"];
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
        [Route("Database/Reset")]
        public IActionResult Reset()
        {
            bool isSuccess = false; // DataBaseManager.Reset();
            return isSuccess ? Ok() : ServerError();
        }
        [HttpPost]
        [Route("Admin/AddUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            user.PasswordHash = "N/A";
            DataBaseManager.AddUser(user);
            string token = CryptoUtils.GetRandomBase64String(24);
            string tokenHash = CryptoUtils.Sha256Hash(token);
            string link =  String.Format(PASSWORD_LINK, HttpUtility.UrlEncode(token));
            //_access.AddPasswordToken(user.ID, tokenHash);
            //MaleUtils.SendRegistration(user.Email, link);
            return Ok();
        }
    }  
}