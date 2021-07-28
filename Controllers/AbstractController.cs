using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.IO;


namespace Submit_System
{
    public abstract class AbstractController : Controller
    {
        public const string SUBMISSIONS = "Submissions";
        public const string RUN_FILES = "Runfiles";
        public static string BASE_FILES { get => RUN_FILES; }
        private static readonly Dictionary<DBCode, int> DBCodeToHttp;
        protected readonly DatabaseAccess _access;
        static AbstractController()
        {
            DBCodeToHttp =  new Dictionary<DBCode, int> {
                [DBCode.OK] = 200,
                [DBCode.NotFound] = 404,
                [DBCode.Invalid] =  400,
                [DBCode.Error] =  500,
                [DBCode.NotAllowed] =  403,
                [DBCode.AlreadyExists] = 409
            };
        }
        public AbstractController(DatabaseAccess access)
        {
            _access = access;
        }
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
            return HandleDatabaseOutput(dbResult.dbCode);
        }
         protected ActionResult HandleDatabaseOutput(DBCode dbCode)
        {
            if(_access.ErrorString != null)
            {
                return new ObjectResult(_access.ErrorString) { StatusCode = DBCodeToHttp[dbCode]};
            }
            return new StatusCodeResult(DBCodeToHttp[dbCode]);
        }
       
        protected ActionResult HandleFileSending(string dir, string fileName)
        {
            string path;
            try
            {
                path = FileUtils.PathSafeCombine(dir, fileName);
            }
            catch(System.Security.SecurityException e)
            {
                return BadRequest(e.Message);
            }
            if(!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            string contentType = FileUtils.GetMimeType(fileName);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, contentType);
        }
        protected ActionResult FileDownload(string dir, string fileName)
        {
            string path;
            try
            {
                path = FileUtils.PathSafeCombine(dir, fileName);
            }
            catch(System.Security.SecurityException e)
            {
                return BadRequest(e.Message);
            }
            if(!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            string contentType = FileUtils.GetMimeType(fileName);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, contentType, fileName);
        }
        
        protected ActionResult HandleArchiveSending(string dir, string zipName)
        {
            try
            {
                return File(FileUtils.ToArchive(dir), "application/zip", zipName);
            }   
            catch(DirectoryNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                return ServerError();
            }
        }

        protected bool IsAnyNull(params Object[] args)
        {
            foreach(var arg in args)
            {
                if(arg == null)
                {
                    return true;
                }
            }
            return false;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string id = context.HttpContext.User.Identity.Name;
            if(id != null)
            {
                bool isAdmin = context.HttpContext.User.IsInRole("Admin");
                 _access.SetUser(id, isAdmin);
            }
        }
         
    }
}