using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using BusinessObject;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Repository.MemberRepo;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;


namespace eStore.Controllers
{

    public class ForgotPassController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([FromForm] string email)
        {
            try
            {
                var context = new SalesManagementContext();
                Member mem = context.Members.Where(x => x.Email.Equals(email)).FirstOrDefault();
                if (mem != null)
                {
                    var password = randomPass();
                    // Set up email message
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("lmpdungbd@gmail.com");
                    message.To.Add(new MailAddress(mem.Email));
                    message.Subject = "Password Reset";
                    message.Body = "Dear User,You requested to reset your password. Here is your new password: " + password;
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("lmpdungbd@gmail.com", "gdmwsmgazgujhafv");
                    smtpClient.EnableSsl = true;
                    // Send email
                    smtpClient.Send(message);

                    mem.Password = password;
                    context.SaveChanges();
                }
                else
                {
                    ViewBag.Message = "Email is not valid";
                    return View();
                }
                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        private string randomPass()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int passwordLength = random.Next(5, 10);
            string password = new string(Enumerable.Repeat(chars, passwordLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }


    }
}
