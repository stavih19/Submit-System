using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
namespace Submit_System
{
    public static class MaleUtils
    {
        const string ADDRESS = "submitsystem.proj@gmail.com";
        const string PASSWORD = "fjoejf23o3r1de11";
        const string REGISTER_SUBJECT =  "הרשמה למערכת סבמיט";
        const string PASSWORD_RESET_SUBJECT = "מערכת סבמיט - שחזור סיסמא";
        const string SMTP_SERVER = "smtp.gmail.com";
        const int PORT = 587;

        public static void SendMail(string sendTo, string subject, string text)
        {
            try
            {
                using(var smtp = new SmtpClient())
                {
                    smtp.Host = SMTP_SERVER;
                    smtp.Port = PORT;
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(ADDRESS,PASSWORD);
                    smtp.Send(
                        new MailMessage(ADDRESS, sendTo, subject, text)
                    );
                }
            }
            catch {}
        }
        public static void SendRegistration(string sendTo, string link)
        {
            SendMail(sendTo, REGISTER_SUBJECT, link);
        }
        public static void PasswordReset(string sendTo, string link)
        {
            SendMail(sendTo, PASSWORD_RESET_SUBJECT, link);
        }
        public static void SendCheckResult(string sendTo, string title, string headerText, List<CheckResult> results)
        {

        }
    }
}