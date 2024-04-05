using BusinessObject;
using DataAccess.Repository.MemberRepo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eStore.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([FromForm] string email, [FromForm] string password)
        {            
            try
            {
                IMemberRepository memberRepository = new MemberRepository();
                Member loginMember = memberRepository.Login(email, password);

                if (loginMember != null)
                {
                    var memberClaims = new List<Claim>()
                    {
                        new Claim("MemberId", loginMember.MemberId.ToString()),
                        new Claim(ClaimTypes.Email, loginMember.Email),
                        new Claim(ClaimTypes.Name, loginMember.Fullname)
                    };

                    var memberIdentity = new ClaimsIdentity(memberClaims, "Member Identity");

                    var memberPrincipal = new ClaimsPrincipal(new[] { memberIdentity });
                    HttpContext.SignInAsync(memberPrincipal);
                }
                return RedirectToAction("Index", "Home");
            } catch (Exception ex)
            {
                ViewBag.Email = email;
                ViewBag.Message = ex.Message;
                return View();
            }
        }
        
        public IActionResult UserAccessDenied()
        {
            return View();
        }
    }
}
