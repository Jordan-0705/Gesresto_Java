using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudSettings = configuration.GetSection("CloudinarySettings");
        Account account = new Account(
            cloudSettings["CloudName"],
            cloudSettings["ApiKey"],
            cloudSettings["ApiSecret"]
        );

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string?> UploadImageAsync(IFormFile file)
{
    if (file == null || file.Length == 0)
        return null;

    using var stream = file.OpenReadStream();

    var uploadParams = new ImageUploadParams
    {
        File = new FileDescription(file.FileName, stream)
    };

    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

    return uploadResult?.SecureUrl?.ToString();
}

}
