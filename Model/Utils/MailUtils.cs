using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
namespace Submit_System
{
    public static class MaleUtils
    {
        private static readonly string Address;
        private static readonly string Password; //"fjoejf23o3r1de11";
        private static readonly string REGISTER_SUBJECT =  "הרשמה למערכת סבמיט";
        public static readonly string PASSWORD_RESET_SUBJECT = "מערכת סבמיט - שחזור סיסמא";
        public static readonly string SmtpServer;
        public static readonly int port;
        static MaleUtils()
        {
            var emailDetails = MyConfig.Configuration.GetSection("Email");
            Address = emailDetails .GetValue<string>("Address");
            Password = emailDetails.GetValue<string>("Password");
            SmtpServer = emailDetails.GetValue<string>("Host");
            port = emailDetails.GetValue<int>("Port");
        }
        public static bool SendMail(string sendTo, string subject, string text)
        {
            using(var smtp = new SmtpClient())
            {
                smtp.Host = SmtpServer;
                smtp.Port = port;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(Address,Password);
                using (var msg = new MailMessage(Address, sendTo, subject, text))
                {
                    try
                    {
                        smtp.Send(msg);
                        return true;
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        return false;
                    }
                }
                
            }
        }
        public static bool SendRegistration(string sendTo, string link)
        {
            return SendMail(sendTo, REGISTER_SUBJECT, link);
        }
        public static bool PasswordReset(string sendTo, string link)
        {
            return SendMail(sendTo, PASSWORD_RESET_SUBJECT, link);
        }
        public static bool SendCheckResult(string sendTo, string title, string headerText, List<CheckResult> results)
        {
            return false; 
        }
    }
}