using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/users"),Authorize]
public class UsersController(RaceDayDbContext db,IImageStorage images):ControllerBase {
 /// <summary>Get the signed-in user's own profile, including role-specific fields.</summary>
 [HttpGet("me")] public async Task<IActionResult> Me(){var id=User.CurrentId();var u=await db.Users.Include(x=>x.Organiser).Include(x=>x.Participant).FirstOrDefaultAsync(x=>x.UserId==id);if(u==null)return NotFound();return Ok(new{u.UserId,u.Email,u.Role,FullName=u.Organiser?.FullName??u.Participant?.FullName,ContactNumber=u.Organiser?.ContactNumber??u.Participant?.ContactNumber,u.Organiser?.OrganisationName,u.Participant?.DateOfBirth,u.Participant?.ProfilePictureUrl});}
 /// <summary>Update the signed-in user's profile.</summary>
 [HttpPut("me")] public async Task<IActionResult> Update(ProfileRequest req){if(string.IsNullOrWhiteSpace(req.FullName))return BadRequest("Full name required.");var id=User.CurrentId();var u=await db.Users.Include(x=>x.Organiser).Include(x=>x.Participant).FirstOrDefaultAsync(x=>x.UserId==id);if(u==null)return NotFound();if(u.Role=="Organiser"&&u.Organiser!=null){u.Organiser.FullName=req.FullName;u.Organiser.ContactNumber=req.ContactNumber;u.Organiser.OrganisationName=req.OrganisationName;}else if(u.Participant!=null){u.Participant.FullName=req.FullName;u.Participant.ContactNumber=req.ContactNumber;u.Participant.DateOfBirth=req.DateOfBirth;}await db.SaveChangesAsync();return await Me();}
 /// <summary>Upload a Participant profile picture (JPEG, PNG or WebP, max 5 MB) to Azure Blob Storage.</summary>
 [HttpPost("me/profile-picture"),Authorize(Roles="Participant"),Consumes("multipart/form-data")][RequestSizeLimit(5*1024*1024+1024)] public async Task<IActionResult> Picture(IFormFile file,CancellationToken ct){if(!ImageRules.Valid(file))return BadRequest("Use JPEG, PNG or WebP under 5 MB.");var person=await db.Participants.FindAsync(User.CurrentId());if(person==null)return NotFound();try{person.ProfilePictureUrl=await images.UploadAsync(file,"profiles",ct);}catch(InvalidOperationException){return Problem("Azure Blob Storage is not configured.",statusCode:503);}await db.SaveChangesAsync(ct);return Ok(new{person.ProfilePictureUrl});}
}
