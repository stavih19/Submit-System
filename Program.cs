using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
namespace Submit_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MaleUtils.SendMail("testotesto11@outlook.com", "hello", "<u><h3>yes</h3></u><hr><u><h3>yes</h3></u>");
            //CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
