using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RaceDay.Api.Data;
using RaceDay.Api.Services;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<RaceDayDbContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("RaceDayDb")));
builder.Services.AddScoped<IImageStorage,AzureImageStorage>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o=>{o.Cookie.Name="RaceDay.Session";o.Cookie.HttpOnly=true;o.Cookie.SecurePolicy=CookieSecurePolicy.SameAsRequest;o.Cookie.SameSite=SameSiteMode.Lax;o.ExpireTimeSpan=TimeSpan.FromHours(8);o.SlidingExpiration=true;o.Events.OnRedirectToLogin=c=>{c.Response.StatusCode=401;return Task.CompletedTask;};o.Events.OnRedirectToAccessDenied=c=>{c.Response.StatusCode=403;return Task.CompletedTask;};});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o=>{o.SwaggerDoc("v1",new OpenApiInfo{Title="RaceDay API",Version="v1",Description="PROG6212 Part 2. Register and log in using Swagger; your browser retains the authentication cookie."});o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,"RaceDay.Api.xml"),true);});
var app=builder.Build();
app.UseSwagger();app.UseSwaggerUI();app.UseHttpsRedirection();app.UseAuthentication();app.UseAuthorization();app.MapControllers();app.Run();
public partial class Program {}
