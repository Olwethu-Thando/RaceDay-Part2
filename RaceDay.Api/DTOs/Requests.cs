// This file defines the Data Transfer Objects (DTOs) used to transfer
// data between API requests and the RaceDay application. It includes
// request models for user registration, login, profile updates, event
// creation, category creation, enrolment, status updates and race results.
// Data annotation attributes are used to validate input, such as required
// fields, email format and maximum or minimum character lengths.
using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.DTOs;
public record RegisterRequest([Required,MaxLength(150)] string FullName,[Required,EmailAddress] string Email,[Required,MinLength(8)] string Password,[Required] string Role,string? ContactNumber,string? OrganisationName,DateTime? DateOfBirth);
public record LoginRequest([Required,EmailAddress] string Email,[Required] string Password);
public record ProfileRequest([Required,MaxLength(150)] string FullName,string? ContactNumber,string? OrganisationName,DateTime? DateOfBirth);
public record EventRequest([Required,MaxLength(150)] string Name,string? Description,DateTime EventDate,[Required,MaxLength(150)] string Location,decimal DistanceKm,[Required] string EventType);
public record CategoryRequest([Required,MaxLength(100)] string Name,int? MinAge,int? MaxAge,decimal? DistanceKm);
public record EnrolRequest(int CategoryId);
public record StatusRequest(string Status);
public record ResultRequest(int FinishTimeSeconds,int FinishPosition,int TotalFinishers);
