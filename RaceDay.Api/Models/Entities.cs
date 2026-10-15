// This file defines the main data models used in the RaceDay application.
// Each class represents a database table and contains properties that
// store information about users, organisers, participants, race events,
// categories, enrolments and race results. Data annotations specify
// primary keys and maximum field lengths, while navigation properties
// define relationships between the models. Default values are provided
// for dates and enrolment status, and nullable properties allow optional
// information such as contact details, profile pictures and results.
using System.ComponentModel.DataAnnotations;
namespace RaceDay.Api.Models;
public class User { public int UserId {get;set;} [MaxLength(150)] public string Email {get;set;}=""; [MaxLength(255)] public string PasswordHash {get;set;}=""; [MaxLength(20)] public string Role {get;set;}=""; public DateTime CreatedDate {get;set;}=DateTime.UtcNow; public Organiser? Organiser {get;set;} public Participant? Participant {get;set;} }
public class Organiser { [Key] public int UserId {get;set;} public User User {get;set;}=null!; [MaxLength(150)] public string FullName {get;set;}=""; [MaxLength(20)] public string? ContactNumber {get;set;} [MaxLength(150)] public string? OrganisationName {get;set;} public List<RaceEvent> Events {get;set;}=new(); }
public class Participant { [Key] public int UserId {get;set;} public User User {get;set;}=null!; [MaxLength(150)] public string FullName {get;set;}=""; [MaxLength(20)] public string? ContactNumber {get;set;} public DateTime? DateOfBirth {get;set;} [MaxLength(500)] public string? ProfilePictureUrl {get;set;} public List<Enrolment> Enrolments {get;set;}=new(); }
public class RaceEvent { [Key] public int EventId {get;set;} public int OrganiserId {get;set;} public Organiser Organiser {get;set;}=null!; [MaxLength(150)] public string Name {get;set;}=""; public string? Description {get;set;} public DateTime EventDate {get;set;} [MaxLength(150)] public string Location {get;set;}=""; public decimal DistanceKm {get;set;} [MaxLength(10)] public string EventType {get;set;}=""; [MaxLength(500)] public string? BannerImageUrl {get;set;} public List<Category> Categories {get;set;}=new(); public List<Enrolment> Enrolments {get;set;}=new(); }
public class Category { [Key] public int CategoryId {get;set;} public int EventId {get;set;} public RaceEvent Event {get;set;}=null!; [MaxLength(100)] public string Name {get;set;}=""; public int? MinAge {get;set;} public int? MaxAge {get;set;} public decimal? DistanceKm {get;set;} public List<Enrolment> Enrolments {get;set;}=new(); }
public class Enrolment { [Key] public int EnrolmentId {get;set;} public int ParticipantId {get;set;} public Participant Participant {get;set;}=null!; public int EventId {get;set;} public RaceEvent Event {get;set;}=null!; public int CategoryId {get;set;} public Category Category {get;set;}=null!; public DateTime EnrolmentDate {get;set;}=DateTime.UtcNow; [MaxLength(20)] public string Status {get;set;}="Pending"; public RaceResult? Result {get;set;} }
public class RaceResult { [Key] public int ResultId {get;set;} public int EnrolmentId {get;set;} public Enrolment Enrolment {get;set;}=null!; public int FinishTimeSeconds {get;set;} public int FinishPosition {get;set;} public int TotalFinishers {get;set;} public DateTime CapturedDate {get;set;}=DateTime.UtcNow; }
