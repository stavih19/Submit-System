using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Principal;
using System.Diagnostics;
namespace Submit_System
{
    public class AuthFilter: IResourceFilter
    {  
        private readonly TokenStorage _storage;
        public AuthFilter(TokenStorage storage) {
            _storage = storage;
        }
        public void OnResourceExecuted(ResourceExecutedContext context) {
            string user = context.HttpContext.User.Identity.Name;
            Debug.WriteLine($"Request {context.HttpContext.Request.Path} By {user}. Status Code: {context.HttpContext.Response.StatusCode}");
        }
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string tokenId = context.HttpContext.Request.Cookies["token"]?.ToString();
            Token token;
            if(_storage.TryGetToken(tokenId, out token))
            {
                string[] roles = token.IsAdmin ? new string[] { "Admin" } : new string[0];
                context.HttpContext.User = new GenericPrincipal(new GenericIdentity(token.UserID), roles);
                string query = context.HttpContext.Request.QueryString.ToString();
                if(query.Contains("Admin/") && !token.IsAdmin)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }  
}