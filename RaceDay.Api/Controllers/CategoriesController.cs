using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController] public class CategoriesController(RaceDayDbContext db):ControllerBase {
 /// <summary>List categories for an event.</summary>
 [HttpGet("api/events/{eventId:int}/categories"),AllowAnonymous] public async Task<IActionResult> List(int eventId){if(!await db.Events.AnyAsync(x=>x.EventId==eventId))return NotFound();return Ok(await db.Categories.Where(x=>x.EventId==eventId).Select(x=>new{x.CategoryId,x.EventId,x.Name,x.MinAge,x.MaxAge,x.DistanceKm}).ToListAsync());}
 /// <summary>Add an age or distance category to your events.</summary>
 [HttpPost("api/events/{eventId:int}/categories"),Authorize(Roles="Organiser")] public async Task<IActionResult> Create(int eventId,CategoryRequest req){var ev=await db.Events.FindAsync(eventId);if(ev==null)return NotFound();if(ev.OrganiserId!=User.CurrentId())return Forbid();var error=Helpers.CategoryError(req.Name,req.MinAge,req.MaxAge,req.DistanceKm);if(error!=null)return BadRequest(error);var cat=new Category{EventId=eventId,Name=req.Name,MinAge=req.MinAge,MaxAge=req.MaxAge,DistanceKm=req.DistanceKm};db.Categories.Add(cat);await db.SaveChangesAsync();return Created($"/api/events/{eventId}/categories",cat);}
 /// <summary>Update a category belonging to your event.</summary>
 [HttpPut("api/categories/{id:int}"),Authorize(Roles="Organiser")] public async Task<IActionResult> Update(int id,CategoryRequest req){var cat=await db.Categories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.CategoryId==id);if(cat==null)return NotFound();if(cat.Event.OrganiserId!=User.CurrentId())return Forbid();var error=Helpers.CategoryError(req.Name,req.MinAge,req.MaxAge,req.DistanceKm);if(error!=null)return BadRequest(error);cat.Name=req.Name;cat.MinAge=req.MinAge;cat.MaxAge=req.MaxAge;cat.DistanceKm=req.DistanceKm;await db.SaveChangesAsync();return Ok(new{cat.CategoryId,cat.EventId,cat.Name,cat.MinAge,cat.MaxAge,cat.DistanceKm});}
 /// <summary>Delete a category if nobody is enrolled in it.</summary>
 [HttpDelete("api/categories/{id:int}"),Authorize(Roles="Organiser")] public async Task<IActionResult> Delete(int id){var cat=await db.Categories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.CategoryId==id);if(cat==null)return NotFound();if(cat.Event.OrganiserId!=User.CurrentId())return Forbid();if(await db.Enrolments.AnyAsync(x=>x.CategoryId==id))return Conflict("Cannot delete a category with enrolments.");db.Categories.Remove(cat);await db.SaveChangesAsync();return NoContent();}
}
