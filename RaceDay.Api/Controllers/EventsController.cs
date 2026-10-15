using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/events")]
public class EventsController(RaceDayDbContext db,IImageStorage images):ControllerBase {
 /// <summary>List upcoming events, optionally filtered by eventType and location.</summary>
 [HttpGet,AllowAnonymous] public async Task<IActionResult> List([FromQuery]string? eventType,[FromQuery]string? location){var q=db.Events.AsNoTracking().Where(x=>x.EventDate>=DateTime.UtcNow);if(!string.IsNullOrWhiteSpace(eventType))q=q.Where(x=>x.EventType==eventType);if(!string.IsNullOrWhiteSpace(location))q=q.Where(x=>x.Location.Contains(location));return Ok(await q.OrderBy(x=>x.EventDate).Select(x=>new{x.EventId,x.Name,x.Description,x.EventDate,x.Location,x.DistanceKm,x.EventType,x.BannerImageUrl}).ToListAsync());}
 /// <summary>List the current Organiser's events with enrolment counts.</summary>
 [HttpGet("mine"),Authorize(Roles="Organiser")] public async Task<IActionResult> Mine(){var id=User.CurrentId();return Ok(await db.Events.Where(x=>x.OrganiserId==id).Select(x=>new{x.EventId,x.Name,x.EventDate,x.EventType,EnrolmentCount=x.Enrolments.Count}).ToListAsync());}
 /// <summary>Get an event and its available categories.</summary>
 [HttpGet("{id:int}"),AllowAnonymous] public async Task<IActionResult> Get(int id){var item=await db.Events.AsNoTracking().Where(x=>x.EventId==id).Select(x=>new{x.EventId,x.OrganiserId,x.Name,x.Description,x.EventDate,x.Location,x.DistanceKm,x.EventType,x.BannerImageUrl,Categories=x.Categories.Select(c=>new{c.CategoryId,c.Name,c.MinAge,c.MaxAge,c.DistanceKm})}).FirstOrDefaultAsync();return item==null?NotFound():Ok(item);}
 /// <summary>Create an event owned by the signed-in Organisers.</summary>
 [HttpPost,Authorize(Roles="Organiser")] public async Task<IActionResult> Create(EventRequest req){var error=Helpers.EventError(req.Name,req.Location,req.DistanceKm,req.EventType,req.EventDate);if(error!=null)return BadRequest(error);var item=new RaceEvent{OrganiserId=User.CurrentId(),Name=req.Name,Description=req.Description,EventDate=req.EventDate,Location=req.Location,DistanceKm=req.DistanceKm,EventType=req.EventType};db.Events.Add(item);await db.SaveChangesAsync();return CreatedAtAction(nameof(Get),new{id=item.EventId},item);}
 /// <summary>Update an event; only its owning Organiser can edit it.</summary>
 [HttpPut("{id:int}"),Authorize(Roles="Organiser")] public async Task<IActionResult> Update(int id,EventRequest req){var item=await db.Events.FindAsync(id);if(item==null)return NotFound();if(item.OrganiserId!=User.CurrentId())return Forbid();var error=Helpers.EventError(req.Name,req.Location,req.DistanceKm,req.EventType,req.EventDate);if(error!=null)return BadRequest(error);item.Name=req.Name;item.Description=req.Description;item.EventDate=req.EventDate;item.Location=req.Location;item.DistanceKm=req.DistanceKm;item.EventType=req.EventType;await db.SaveChangesAsync();return Ok(item);}
 /// <summary>Delete your event. Enrolments must first be removed to protect race records.</summary>
 [HttpDelete("{id:int}"),Authorize(Roles="Organiser")] public async Task<IActionResult> Delete(int id){var item=await db.Events.FindAsync(id);if(item==null)return NotFound();if(item.OrganiserId!=User.CurrentId())return Forbid();if(await db.Enrolments.AnyAsync(x=>x.EventId==id))return Conflict("Cannot delete an event with enrolments.");db.Events.Remove(item);await db.SaveChangesAsync();return NoContent();}
 /// <summary>Upload a banner for your event (JPEG, PNG or WebP, max 5 MB).</summary>
 [HttpPost("{id:int}/banner"),Authorize(Roles="Organiser"),Consumes("multipart/form-data")][RequestSizeLimit(5*1024*1024+1024)] public async Task<IActionResult> Banner(int id,IFormFile file,CancellationToken ct){var item=await db.Events.FindAsync(id);if(item==null)return NotFound();if(item.OrganiserId!=User.CurrentId())return Forbid();if(!ImageRules.Valid(file))return BadRequest("Use JPEG, PNG or WebP under 5 MB.");try{item.BannerImageUrl=await images.UploadAsync(file,"banners",ct);}catch(InvalidOperationException){return Problem("Azure Blob Storage is not configured.",statusCode:503);}await db.SaveChangesAsync(ct);return Ok(new{item.BannerImageUrl});}
}
