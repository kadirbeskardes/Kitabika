
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookStore.Core.Entities;

namespace BookStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            try
            {
                var user = await _userService.LoginAsync(loginDto);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ModelState.AddModelError("", "Geçersiz kullanýcý adý veya þifre.");
                return View(loginDto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return View(createUserDto);

            try
            {
                await _userService.RegisterAsync(createUserDto);
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError("", "Kayýt baþarýsýz. Kullanýcý adý veya e-posta zaten kullanýlýyor olabilir.");
                return View(createUserDto);
            }
        }

        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            UserDto user=await _userService.GetUserByIdAsync(userId);
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
