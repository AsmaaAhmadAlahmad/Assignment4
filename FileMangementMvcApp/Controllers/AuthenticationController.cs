using FileManagement.Data;
using FileManagement.Domain.Entities;
using FileMangementMvcApp.Models;
using FileMangementMvcApp.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Assignement3_Domain.Helper;

namespace FileMangementMvcApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IGenericRepository<User, RegisterModel> genericRepository;
        private readonly HelperMapper<User, RegisterModel, LoginModel> mapper;

        public AuthenticationController(IGenericRepository<User, RegisterModel> genericRepository,
                                        HelperMapper<User,RegisterModel ,LoginModel> mapper )
        {
            this.genericRepository = genericRepository;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            ViewBag.NotificationWrongPasswordOfUsername = TempData["NotificationWrongPasswordOfUsername"];
            ViewBag.SuccessCreatedAccountMessage = TempData["SuccessMessage"];

            return View();
        }

        public IActionResult Register()
        {
            ViewBag.NotificationAlreayExists = TempData["NotificationAlreayExists"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            // التأكد من أن اس المستخدم ليس موجود سابقا في الداتا بيز
            var existUser = await genericRepository.GetItemAsync(filter: u => u.UserName == registerModel.UserName);

            if (existUser != null)
            {
                TempData["NotificationAlreayExists"] = "This account already exists";
                return RedirectToAction("Register");
            }
            else
            {
                // توليد الملح تلقائيًا
                string salt = BCrypt.Net.BCrypt.GenerateSalt();

                // هاش كلمة المرور باستخدام BCrypt.Net والملح الذي تم توليده
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerModel.Password, salt);

                // لم استخدم الاوتو مابر هنا لان المابر سيكون من اجل خاصية واحدة فقط وهي خاصية اسم المستخدم
                User user = new User
                {
                    UserName = registerModel.UserName,
                    Password = hashedPassword,
                };

                // اضافة المستخدم للداتا بيز
                await genericRepository.AddAsync(user);

                TempData["SuccessMessage"] = "Your account has been successfully created. Please log in now.";

                return RedirectToAction("Index", "File");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            // التأكد من تواجد اسم المستخدم في القاعدة
            var checkLogin = await genericRepository.GetItemAsync(filter: u => u.UserName == loginModel.UserName);

            if (checkLogin != null)
            {
                // التأكد من صحة كلمة المرور
                if (BCrypt.Net.BCrypt.Verify(loginModel.Password, checkLogin.Password))
                {
                    // Claims إذا كانت كلمة المرور صحيحة، قم بإعداد ال
                    List<Claim> claims = new List<Claim>
                    {
                       new Claim(ClaimTypes.NameIdentifier, checkLogin.Id.ToString()),
                       new Claim(ClaimTypes.Name, loginModel.UserName)
                    };

                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

                    return RedirectToAction("Index", "File");
                }
            }

            TempData["NotificationWrongPasswordOfUsername"] = "Wrong username or password";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }
    }
}
