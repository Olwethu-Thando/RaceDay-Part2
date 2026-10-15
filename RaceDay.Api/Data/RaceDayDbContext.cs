using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
namespace RaceDay.Api.Data;


// This class defines the RaceDay database context and manages the
// relationship between the application's entities and the SQL Server
// database. It defines database tables for Users, Organisers,
// Participants, RaceEvents, Categories, Enrolments and RaceResults.
// It also configures primary relationships, foreign keys, unique
// constraints, validation rules, decimal precision and delete behaviours
// to maintain data accuracy and integrity throughout the application.

public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options):DbContext(options) {
 public DbSet<User> Users=>Set<User>(); public DbSet<Organiser> Organisers=>Set<Organiser>(); public DbSet<Participant> Participants=>Set<Participant>(); public DbSet<RaceEvent> Events=>Set<RaceEvent>(); public DbSet<Category> Categories=>Set<Category>(); public DbSet<Enrolment> Enrolments=>Set<Enrolment>(); public DbSet<RaceResult> Results=>Set<RaceResult>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<User>().HasIndex(x=>x.Email).IsUnique();
  b.Entity<User>().ToTable(t=>t.HasCheckConstraint("CK_Users_Role","[Role] IN ('Organiser','Participant')"));
  b.Entity<Organiser>().HasOne(x=>x.User).WithOne(x=>x.Organiser).HasForeignKey<Organiser>(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
  b.Entity<Participant>().HasOne(x=>x.User).WithOne(x=>x.Participant).HasForeignKey<Participant>(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
  b.Entity<RaceEvent>().HasOne(x=>x.Organiser).WithMany(x=>x.Events).HasForeignKey(x=>x.OrganiserId).OnDelete(DeleteBehavior.NoAction);
  b.Entity<RaceEvent>().Property(x=>x.DistanceKm).HasPrecision(6,2);
  b.Entity<RaceEvent>().ToTable(t=>t.HasCheckConstraint("CK_Events_EventType","[EventType] IN ('Run','Walk','Cycle')"));
  b.Entity<Category>().HasOne(x=>x.Event).WithMany(x=>x.Categories).HasForeignKey(x=>x.EventId).OnDelete(DeleteBehavior.Cascade);
  b.Entity<Category>().Property(x=>x.DistanceKm).HasPrecision(6,2);
  b.Entity<Enrolment>().HasOne(x=>x.Participant).WithMany(x=>x.Enrolments).HasForeignKey(x=>x.ParticipantId).OnDelete(DeleteBehavior.NoAction);
  b.Entity<Enrolment>().HasOne(x=>x.Event).WithMany(x=>x.Enrolments).HasForeignKey(x=>x.EventId).OnDelete(DeleteBehavior.NoAction);
  b.Entity<Enrolment>().HasOne(x=>x.Category).WithMany(x=>x.Enrolments).HasForeignKey(x=>x.CategoryId).OnDelete(DeleteBehavior.NoAction);
  b.Entity<Enrolment>().HasIndex(x=>new{x.ParticipantId,x.EventId}).IsUnique();
  b.Entity<Enrolment>().ToTable(t=>t.HasCheckConstraint("CK_Enrolments_Status","[Status] IN ('Pending','Confirmed','Cancelled')"));
  b.Entity<RaceResult>().HasOne(x=>x.Enrolment).WithOne(x=>x.Result).HasForeignKey<RaceResult>(x=>x.EnrolmentId).OnDelete(DeleteBehavior.Cascade);
  b.Entity<RaceResult>().HasIndex(x=>x.EnrolmentId).IsUnique();
 }
}
