using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;

namespace Submit_System
{
    public abstract class AbstractController : ControllerBase  
    {
        
        private static readonly int[] results = new int[] { 200 , 404, 400, 500 };
        protected ActionResult ServerError() 
            => new StatusCodeResult(500);
         protected ActionResult ServerError([ActionResultObjectValue] Object obj)
            => new ObjectResult(obj) { StatusCode = 500 };
        protected ActionResult HandleDatabaseOutput((Object output, int dbCode, string msg) dbResult)
        {
            if(dbResult.dbCode == 0)
            {
                return Ok(dbResult.output);
            }
            Trace.WriteLine(dbResult.msg);
            return new StatusCodeResult(results[dbResult.dbCode]);
        }
        protected bool IsDatabaseError((Object output, int dbCode, string msg) dbResult)
        {
             if(dbResult.dbCode == 0)
            {
                return true;
            }
            return false;
        }
    }
}