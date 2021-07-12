using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Collections.Generic;
using System.Web;
namespace Submit_System
{
    public class UserController : AbstractController 
    {  
        private const string PASSWORD_LINK = "http://localhost:5000/SetPassword?token={0}";
        private readonly TokenStorage _storage;
        private readonly  DatabaseAccess _access;
        public UserController(TokenStorage storage, DatabaseAccess access)
        {
            _storage = storage;
            _access = access;

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
            string id = _storage.CreateToken(login.Username);
            return new List<string> { HttpUtility.UrlEncode(id), result.Item1};
        }
        [ServiceFilter(typeof(AuthFilter))]
        [Route("User/Password")]
        [HttpPut]
        public IActionResult SetPassword(string token, string userid, [FromBody]string password)
        {
            var hash = CryptoUtils.KDFHash(password);
            //Logout(token);
            return Ok();
        }
        [HttpDelete]
        [Route("User/Logout")]
        public IActionResult Logout(string token) {
            _storage.RemoveToken(token);
            return Ok();
        }
        [HttpHead]
        [Route("User/CheckToken")]
        public IActionResult CheckToken(string token)
        {
            if(_storage.TryGetUserID(token, out _))
            {
                return Ok();
            }
            return NotFound();
        }
        [HttpPut]
        [Route("User/Unlock")]
        public IActionResult TurnOff()
        {
            _storage.IsTestMode = true;
            return Ok("Authentication disabled.");
        }
        [HttpPut]
        [Route("User/Lock")]
        public IActionResult TurnOn()
        {
            _storage.IsTestMode = false;
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