using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/auth")]
public class AuthController(RaceDayDbContext db):ControllerBase {
 /// <summary>Register an Organiser or Participant with a securely hashed password.</summary><response code="201">User ID and role</response><response code="409">Email already registered</response>
 [HttpPost("register"),AllowAnonymous] public async Task<IActionResult> Register(RegisterRequest req){if(req.Role is not ("Organiser" or "Participant"))return BadRequest("Role must be Organiser or Participant.");if(string.IsNullOrWhiteSpace(req.FullName)||req.Password.Length<8)return BadRequest("Full name and password of at least 8 characters required.");var email=req.Email.Trim().ToLowerInvariant();if(await db.Users.AnyAsync(x=>x.Email==email))return Conflict("Email already registered.");var user=new User{Email=email,Role=req.Role};user.PasswordHash=new PasswordHasher<User>().HashPassword(user,req.Password);if(req.Role=="Organiser")user.Organiser=new Organiser{FullName=req.FullName,ContactNumber=req.ContactNumber,OrganisationName=req.OrganisationName};else user.Participant=new Participant{FullName=req.FullName,ContactNumber=req.ContactNumber,DateOfBirth=req.DateOfBirth};db.Users.Add(user);try{await db.SaveChangesAsync();}catch(DbUpdateException){return Conflict("Unable to register; email may already exist.");}return StatusCode(201,new{user.UserId,user.Role});}
 /// <summary>Validate credentials and issue a browser session cookie.</summary><response code="200">Authenticated role</response><response code="401">Invalid credentials</response>
 [HttpPost("login"),AllowAnonymous] public async Task<IActionResult> Login(LoginRequest req){var user=await db.Users.FirstOrDefaultAsync(x=>x.Email==req.Email.Trim().ToLowerInvariant());if(user==null||new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,req.Password)==PasswordVerificationResult.Failed)return Unauthorized("Invalid credentials.");var claims=new[]{new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),new Claim(ClaimTypes.Email,user.Email),new Claim(ClaimTypes.Role,user.Role)};await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));return Ok(new{message="Logged in",user.UserId,user.Role});}
 /// <summary>End the current authenticated sessions.</summary>
 [HttpPost("logout"),Authorize] public async Task<IActionResult> Logout(){await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);return Ok(new{message="Logged out"});}
}
