using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly DataContext _context;
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config, DataContext context)
    {
        _context = context;
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "da-net7"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }

    public async Task<Photo> GetPhotoById(int photoId)
    {
        return await _context.Photos.Where(x => x.Id == photoId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Photo>> GetUnapprovedPhotos()
    {
        return await _context.Photos.Where(x => x.IsApproved == false).ToListAsync();
    }
}