using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Abstractions.Services;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        IEmailService _emailService { get; }
        UserManager<AppUser> _userManager { get; }
        SignInManager<AppUser> _signInManager { get; }
        RoleManager<IdentityRole> _roleManager { get; }

        public AccountController(UserManager<AppUser> userManager,
               SignInManager<AppUser> signInManager,
               RoleManager<IdentityRole> roleManager,
               IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(registerVM.Username);
            if (user !=null)
            {
                ModelState.AddModelError("Username", "Bu istifadeci artiq movcuddur.");
                return View();
            }
            user = new AppUser
            {
                FirsName = registerVM.Name,
                LastName = registerVM.Surname,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, "Member");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail),"Account", new {token, Email=user.Email}, Request.Scheme);
            _emailService.Send(user.Email, "Confirmation link", confirmationLink);
            //await _signInManager.SignInAsync(user, true);
            return RedirectToAction(nameof(SuccessfullyRegistered));
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            AppUser user= await _userManager.FindByEmailAsync(email);
            if (user==null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) NotFound();
            await _signInManager.SignInAsync(user, true);
            return View();
        }
        public IActionResult SuccessfullyRegistered()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Login or Password is wrong");
                    return View();
                }
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("","Please confirm your email");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistance, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Login or Password is wrong");
                return View();
            }
            if (ReturnUrl==null)
            {
            return RedirectToAction("Index","Home");
            }
            else
            {
                return Redirect(ReturnUrl);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
            //return RedirectToAction("Index", "Home");
        }
        //public async Task<IActionResult> Test()
        //{
        //    AppUser user = new AppUser
        //    {
        //        FirsName = "admin",
        //        LastName = "admin",
        //        UserName = "admin",
        //        Email = "tu7fmvb99@code.edu.az"
        //    };
        //    await _userManager.CreateAsync(user, "Admin123");
        //    await _userManager.AddToRoleAsync(user, "Admin");
        //    return View();

        //}

        public async Task<IActionResult> AddRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Moderator" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            return View();
        }
    }
}
