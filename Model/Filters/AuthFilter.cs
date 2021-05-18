using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Submit_System
{
    public class AuthFilter: ActionFilterAttribute
    {  
        private readonly TokenStorage _storage;
        protected Token UserToken;

        public AuthFilter(TokenStorage storage) {
            _storage = storage;
        }
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            string tokenId = actionContext.HttpContext.Request.Query["token"].ToString();
            string userid;
            if(_storage.TryGetUserID(tokenId, out userid))
            {
                actionContext.ActionArguments["userid"] = userid;     
                return;
            }
            actionContext.Result = new UnauthorizedResult();
        }
    }  
}