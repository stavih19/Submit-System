using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
namespace Submit_System
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //Trace.WriteLine(CryptoUtils.Hash("password"));
            //CryptoUtils.Test();
            // var a = File.ReadAllBytes("ex2.zip");
            // var b = Convert.ToBase64String(a);
            // Trace.WriteLine(b);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
