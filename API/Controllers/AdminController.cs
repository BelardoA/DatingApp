using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPhotoService _photoService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(
        UserManager<AppUser> userManager,
        IPhotoService photoService,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _photoService = photoService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUserWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                UserName = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{userName}")]
    public async Task<ActionResult> EditRoles(string userName, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least 1 role.");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var results = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!results.Succeeded) return BadRequest("Failed to add to roles.");

        var result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult<IEnumerable<PhotoDto>> GetPhotosForModeration()
    {
        var query = _photoService.GetUnapprovedPhotos();

        if (query == null) return NotFound();

        List<PhotoDto> photos = new List<PhotoDto>();
        // add photos to list of PhotoDtos
        foreach (Photo photo in query.Result)
        {
            photos.Add(_mapper.Map<PhotoDto>(photo));
        }

        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = _photoService.GetPhotoById(photoId).Result;

        if (photo == null) return NotFound();

        if (photo.IsApproved) return BadRequest("This photo has already been approved.");

        photo.IsApproved = true;

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem setting main photo.");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var photo = _photoService.GetPhotoById(photoId).Result;

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot a user's main photo.");

        var user = await _unitOfWork.UserRepository.GetUserByIdAsync(photo.AppUserId);

        if (user == null) return NotFound();

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the photo.");
    }
}