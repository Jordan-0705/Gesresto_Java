using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface ICloudinaryService
{
    Task<string?> UploadImageAsync(IFormFile file);
}
