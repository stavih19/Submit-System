using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
namespace Submit_System
{
    public class ExceptionFilter: IExceptionFilter
    { 
        public void OnException(ExceptionContext context)
        {
            Trace.WriteLine(context.Exception.ToString());
            context.Result = new StatusCodeResult(500);
            context.ExceptionHandled = true;
        }
    }  
}