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
        public const string PASSWORD_LINK = "http://localhost:5000?token={0}";
        private readonly TokenStorage _storage;
        public UserController(TokenStorage storage, DatabaseAccess access) : base(access)
        {
            _storage = storage;
        }
        [HttpPost]  
        [Route("User/Login")]
        public ActionResult<List<string>> Login([FromBody]LoginData login)  
        {  
            if (IsAnyNull(login?.Username, login?.Password))  
            {  
                return BadRequest();
            }
            bool isAdmin;
            var result = _access.AuthenticateUser(login.Username, login.Password, out isAdmin);
            if(result.Item1 == null) {
                return NotFound();
            }
            string id = _storage.CreateToken(login.Username, isAdmin);
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
        public ActionResult ResetPasswordRequest(string userID)
        {
            (User u, DBCode c) = _access.GetUser(userID);
            if(c != DBCode.OK)
            {
                return HandleDatabaseOutput(c);
            }
            bool success = PasswordRequest(userID, u.Email);
            return success ? Ok() : ServerError();
        }
        [HttpPost]
        [Route("User/ResetPassword")]
        public ActionResult ResetPassword(string id)
        {
            (User user, DBCode code) = _access.GetUser(id);
            if(code != DBCode.OK)
            {
                return(HandleDatabaseOutput(code));
            }
            return PasswordRequest(id, user.Email) ? Ok() : ServerError();
        }
        [HttpPost]
        [Route("User/CheckPasswordToken")]
        public ActionResult CheckPasswordToken(string token)
        {
            DBCode code = _access.CheckPasswordToken(token).Item2;
            return HandleDatabaseOutput(code);
        }
        [NonAction]
        private bool PasswordRequest(string userID, string email)
        {
            string token = CryptoUtils.GetRandomBase64String(24);
            string tokenHash = CryptoUtils.Sha256Hash(token);
            string link =  String.Format(PASSWORD_LINK, HttpUtility.UrlEncode(token));
            DBCode code = _access.AddPasswordToken(userID, tokenHash, DateTime.Now.AddDays(2));
            if(code != DBCode.OK)
            {
                return false;
            }
            string text = $"Password form link:<br>" + link;
            MaleUtils.SendMail(email, "Submit Bar Ilan Reset Password" ,text);
            return true;
        }
        [HttpPost]
        [Route("Admin/AddUser")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult AddUser([FromBody] User user)
        {
            if(!_access.IsAdmin) { return Forbid(); }
            user.PasswordHash = null;
            DBCode code = _access.AddUser(user);
            if(code != DBCode.OK)
            {
                return HandleDatabaseOutput(code);
            }
            bool success = PasswordRequest(user.ID, user.Email);
            return success ? Ok() : ServerError();
        }
        [HttpPost]
        [Route("Admin/AddUsers")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult AddUsers([FromBody] string content)
        {
            string path = FileUtils.GetTempFileName(".csv");
            System.IO.File.WriteAllText(path, content);
            content = null;
            var a = ExcelLoader.ReadUsersFromFile(path, _access);
            System.IO.File.Delete(path);
            return Ok();
        }
    }  
}