
// This file contains helper methods and services used by the RaceDay API.
// The Helpers class retrieves the logged-in user's ID and validates race
// events, categories and race results before they are processed. The
// IImageStorage interface defines the image upload functionality, while
// AzureImageStorage uploads images to Azure Blob Storage and returns the
// image URL. The ImageRules class checks that uploaded images meet the
// required file size, file extension and content type restrictions.
using System.Security.Claims;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace RaceDay.Api.Services;
public static class Helpers { public static int CurrentId(this ClaimsPrincipal user)=>int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!); public static string? EventError(string name,string location,decimal distance,string type,DateTime date) => string.IsNullOrWhiteSpace(name)||string.IsNullOrWhiteSpace(location)||distance<=0||distance>9999.99m||!new[]{"Run","Walk","Cycle"}.Contains(type)||date==default ? "Provide a name, location, positive distance, valid date and eventType (Run, Walk or Cycle)." : null; public static string? CategoryError(string name,int? min,int? max,decimal? distance)=>string.IsNullOrWhiteSpace(name)||min<0||max<0||(min.HasValue&&max.HasValue&&min>max)||(distance.HasValue&&(distance<=0||distance>9999.99m)) ? "Invalid category name, age range or distance." : null; public static string? ResultError(int time,int position,int total)=>time<=0||position<=0||total<=0||position>total?"Finish time and positions must be positive; position cannot exceed total finishers.":null; }
public interface IImageStorage { Task<string> UploadAsync(IFormFile file,string container,CancellationToken ct); }
public class AzureImageStorage(IConfiguration config):IImageStorage { public async Task<string> UploadAsync(IFormFile file,string container,CancellationToken ct) { var connection=config["AzureStorage:ConnectionString"]; if(string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("AzureStorage:ConnectionString has not been configured."); var client=new BlobContainerClient(connection,container); await client.CreateIfNotExistsAsync(PublicAccessType.None,cancellationToken:ct); var ext=Path.GetExtension(file.FileName).ToLowerInvariant(); var blob=client.GetBlobClient($"{Guid.NewGuid():N}{ext}"); await using var stream=file.OpenReadStream(); await blob.UploadAsync(stream,new BlobUploadOptions {HttpHeaders=new BlobHttpHeaders{ContentType=file.ContentType}},ct); return blob.Uri.ToString(); } }
public static class ImageRules { public static bool Valid(IFormFile? file)=>file is not null && file.Length>0 && file.Length<=5*1024*1024 && new[]{".jpg",".jpeg",".png",".webp"}.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()) && new[]{"image/jpeg","image/png","image/webp"}.Contains(file.ContentType.ToLowerInvariant()); }
