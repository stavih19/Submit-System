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
            // Test test = new Test
            // {
            //     TimeoutInSeconds = 60,
            //     Input = "",
            //     ExerciseID = "aaa",
            //     MainSourseFile = "aaa.py",
            //     ExpectedOutput = "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.",
            // };
            // var a = new Python3Tester();
            // a.AddTest(test);
            // a.SetFilesLocation("stuff");
            // a.SetTestLocation("aaaagswgvewf");
            // a.RunAllTests();
            // var res = a.GetCheckResults();
            // var check = new Python3CheckStyleTester();
            // check.SetFilesLocation("stuff");
            // check.SetCheckLocation("wgwgwgg");
            // check.RunCheck();
            // var style = check.GetCheckResult();
            // var r = new Result(res, style);
            // MaleUtils.SendMail("dr.kojar@gmail.com", "lol", r.ToString());
            Trace.WriteLine(Convert.ToBase64String(File.ReadAllBytes("stuff\\aaa.py")));
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
