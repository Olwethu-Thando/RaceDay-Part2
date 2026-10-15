using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController] public class ResultsController(RaceDayDbContext db):ControllerBase {
 /// <summary>Record a finish time in seconds, position and total finishers for a confirmed enrolment.</summary>
 [HttpPost("api/enrolments/{id:int}/result"),Authorize(Roles="Organiser")] public async Task<IActionResult> Create(int id,ResultRequest req){var entry=await db.Enrolments.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.EnrolmentId==id);if(entry==null)return NotFound();if(entry.Event.OrganiserId!=User.CurrentId())return Forbid();if(entry.Status!="Confirmed")return BadRequest("Only confirmed enrolments can receive results.");if(entry.Event.EventDate>DateTime.UtcNow)return BadRequest("Results cannot be recorded before the event.");var error=Helpers.ResultError(req.FinishTimeSeconds,req.FinishPosition,req.TotalFinishers);if(error!=null)return BadRequest(error);if(await db.Results.AnyAsync(x=>x.EnrolmentId==id))return Conflict("Result already exists.");var result=new RaceResult{EnrolmentId=id,FinishTimeSeconds=req.FinishTimeSeconds,FinishPosition=req.FinishPosition,TotalFinishers=req.TotalFinishers};db.Results.Add(result);await db.SaveChangesAsync();return StatusCode(201,new{result.ResultId,result.EnrolmentId,result.FinishTimeSeconds,result.FinishPosition,result.TotalFinishers});}
 /// <summary>Correct a result for an event you organise.</summary>
 [HttpPut("api/results/{id:int}"),Authorize(Roles="Organiser")] public async Task<IActionResult> Update(int id,ResultRequest req){var result=await db.Results.Include(x=>x.Enrolment).ThenInclude(x=>x.Event).FirstOrDefaultAsync(x=>x.ResultId==id);if(result==null)return NotFound();if(result.Enrolment.Event.OrganiserId!=User.CurrentId())return Forbid();var error=Helpers.ResultError(req.FinishTimeSeconds,req.FinishPosition,req.TotalFinishers);if(error!=null)return BadRequest(error);result.FinishTimeSeconds=req.FinishTimeSeconds;result.FinishPosition=req.FinishPosition;result.TotalFinishers=req.TotalFinishers;await db.SaveChangesAsync();return Ok(new{result.ResultId,result.EnrolmentId,result.FinishTimeSeconds,result.FinishPosition,result.TotalFinishers});}
 /// <summary>List the signed-in Participant's personal race results.</summary>
 [HttpGet("api/participants/me/results"),Authorize(Roles="Participant")] public async Task<IActionResult> Mine(){var id=User.CurrentId();return Ok(await db.Results.Where(x=>x.Enrolment.ParticipantId==id).Select(x=>new{x.ResultId,x.FinishTimeSeconds,x.FinishPosition,x.TotalFinishers,EventName=x.Enrolment.Event.Name,x.Enrolment.Event.EventDate,CategoryName=x.Enrolment.Category.Name}).ToListAsync());}
 /// <summary>Public leaderboard for a completed event.</summary>
 [HttpGet("api/events/{eventId:int}/results"),AllowAnonymous] public async Task<IActionResult> EventResults(int eventId){var ev=await db.Events.FindAsync(eventId);if(ev==null)return NotFound();if(ev.EventDate>DateTime.UtcNow)return Ok(Array.Empty<object>());return Ok(await db.Results.Where(x=>x.Enrolment.EventId==eventId).OrderBy(x=>x.FinishPosition).Select(x=>new{x.ResultId,x.FinishPosition,x.FinishTimeSeconds,x.TotalFinishers,ParticipantName=x.Enrolment.Participant.FullName,CategoryName=x.Enrolment.Category.Name}).ToListAsync());}
}
