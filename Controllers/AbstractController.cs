using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web;

namespace Submit_System
{
    public abstract class AbstractController : ControllerBase  
    {
        public const string SUBMISSIONS = "Submissions";
        public const string RUN_FILES = "Runfiles";
        public static string BASE_FILES { get => RUN_FILES; }
        private static readonly Dictionary<DBCode, int> result =  new Dictionary<DBCode, int> {
            [DBCode.OK] = 200,
            [DBCode.NotFound] = 404,
            [DBCode.Invalid] =  400,
            [DBCode.Error] =  500,
            [DBCode.NotAllowed] =  403,
            [DBCode.AlreadyExists] = 409
        };
        protected ActionResult ServerError() 
            => new StatusCodeResult(500);
         protected ActionResult ServerError([ActionResultObjectValue] Object obj)
            => new ObjectResult(obj) { StatusCode = 500 };
        protected ActionResult<T> HandleDatabaseOutput<T>((T output, DBCode dbCode) dbResult)
        {
            if(dbResult.dbCode == DBCode.OK)
            {
                return Ok(dbResult.output);
            }
            return new StatusCodeResult(result[dbResult.dbCode]);
        }
         protected ActionResult HandleDatabaseOutput(DBCode dbCode)
        {
            return new StatusCodeResult(result[dbCode]);
        }
        protected bool IsDatabaseError((Object output, DBCode code, string msg) dbResult)
        {
             if(dbResult.code == DBCode.OK)
            {
                return true;
            }
            return false;
        }

        protected ActionResult<SubmitFile> HandleFileSending(string dir, string file)
        {
            file = HttpUtility.UrlDecode(file);
            string fullPath = FileUtils.GetFullPath(file, dir);
            if(!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found");
            }
            byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
            return SubmitFile.Create(file, bytes);
        }
    }
}