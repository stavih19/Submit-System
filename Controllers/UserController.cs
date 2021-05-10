using System.Net;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Security;
namespace Submit_System
{
    [Route("[controller]/[action]")] 
    [ApiController]  
    public class UserController : ControllerBase  
    {  
        // GET api/values  
        [HttpPost]  
        public ActionResult<string> Login([FromBody]LoginData user)  
        {  
            if (user == null)  
            {  
                return BadRequest("Invalid request");  
            }  
            return "this is a token";
        }
        [HttpPost]
       public IActionResult Logout(string token) {
            return Ok();
        }
    }  
}