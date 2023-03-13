using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class PhotoController : BaseApiController
{
    private readonly IPhotoService _photoService;
    private readonly IMapper _mapper;

    public PhotoController(IPhotoService photoService, IMapper mapper)
    {
        _photoService = photoService;
        _mapper = mapper;
    }

    [HttpGet("get-photo/{photoId}")]
    public async Task<ActionResult<PhotoDto>> GetPhotoById(int photoId)
    {
        var photo = _photoService.GetPhotoById(photoId);

        if (photo == null) return NotFound();

        var result = _mapper.Map<PhotoDto>(photo.Result);

        return Ok(result);
    }
}