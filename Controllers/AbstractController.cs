using System.Net;
using Microsoft.AspNetCore.Mvc;
namespace Submit_System
{
    public abstract class AbstractController : ControllerBase  
    {
          private static readonly HttpStatusCode[] results = new HttpStatusCode[4]
        {
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.BadRequest,
            HttpStatusCode.InternalServerError
        };
        protected IActionResult ReturnResult(HttpStatusCode code, string msg="")
        {
            return new ContentResult
            {
                Content = msg,
                StatusCode = (int) code,
                ContentType = "text/plain" 
            };
        }
    }
}