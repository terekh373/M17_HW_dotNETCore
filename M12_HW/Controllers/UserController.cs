﻿using M12_HW.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace M12_HW.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        //http://localhost:port/user/register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return BadRequest("Email and Password are required.");

            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded) return Ok($"User {email} registred");//RedirectToAction("Home", "Index");
            else
            {
                foreach (var error in result.Errors) Console.WriteLine(error.Description);
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Email and password are required.");

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            return BadRequest("Invalid email or password.");
        }

        public async Task<ViewResult> CreateRole() => View();
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("Error RoleName ...");
            }
            var existRoleName = await _roleManager.RoleExistsAsync(role.RoleName);
            if (existRoleName)
            {
                return BadRequest("This RoleName alredy exist ...");
            }
            var result = await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            if (result.Succeeded)
            {
                return Ok($"Role: {role.RoleName} created ...");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<ViewResult> AssignRole() => View();
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(Role role)
        {
            if (
                string.IsNullOrEmpty(role.UserId) &&
                string.IsNullOrEmpty(role.RoleId) &&
                string.IsNullOrEmpty(role.RoleName)
                )
            {
                return BadRequest("Error assign role ...");
            }
            var existRole = await _roleManager.FindByIdAsync(role.RoleId);
            if (existRole == null)
            {
                return BadRequest("Not found Role ...");
            }
            var existUser = await _userManager.FindByIdAsync(role.UserId);
            if (existUser == null)
            {
                return BadRequest("Not found User ...");
            }
            var result = await _userManager.AddToRoleAsync(existUser, role.RoleName);
            if (result.Succeeded)
            {
                return Ok("Roles assigned ...");
            }
            return BadRequest(result.Errors);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home"); 
        }
    }
}
