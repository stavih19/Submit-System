using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Collections.Generic;
namespace Submit_System
{
    [Route("")] 
    [ApiController]  
    public class UserController : AbstractController 
    {  
        private readonly TokenStorage _storage;
        public UserController(TokenStorage storage)
        {
            _storage = storage;

        }
        [HttpPost]  
        [Route("User/Login")]
        public ActionResult<List<string>> Login([FromBody]LoginData login)  
        {  
            if (login == null)  
            {  
                return BadRequest();  
            }
            var hash = CryptoUtils.Hash(login.Password);
            //var readResult = DataBaseManager.ReadUser(login.Username);
            if(login.Username == "Yosi" && login.Password == "password") {
                string ID = _storage.CreateToken(login.Username);
                string name = "Yosi Yosi";
                return new List<string> { ID, name };
            }
            return NotFound();
        }
        [ServiceFilter(typeof(AuthFilter))]
        [Route("User/Password")]
        [HttpPut]
        public IActionResult SetPassword(string token, string userid, [FromBody]string password)
        {
            var hash = CryptoUtils.Hash(password);
            //Logout(token);
            return Ok();
        }
        [HttpGet]
        public IActionResult Logout(string token) {
            _storage.RemoveToken(token);
            return Ok();
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
    }  
}