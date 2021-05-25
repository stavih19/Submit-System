using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Collections.Generic;
namespace Submit_System
{
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
            if (login == null || login?.Username == null || login?.Password == null)  
            {  
                return BadRequest();
            }
            string passwordHash = CryptoUtils.Hash("password"); // Hash result for "password"
            // var readResult = DataBaseManager.ReadUser(login.Username);
            // if(IsDatabaseError(readResult))
            // {
            //     return HandleDatabaseOutput(readResult);
            // }
            //User user = readResult.Item1;
            if(login.Username == "Yosi" && CryptoUtils.Verify(login.Password, passwordHash)) {
                string ID = _storage.CreateToken(login.Username);
                return new List<string> {ID, "Yosi"};
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
    }  
}