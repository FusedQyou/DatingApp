using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var repoPhoto = await _repo.GetPhoto(id);
            var returnPhoto = _mapper.Map<ReturnPhoto>(repoPhoto);
            return Ok(returnPhoto);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoCreation photoCreation)
        {
            // Verify the user that requests the update is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != tokenId) {
                return Unauthorized();
            }

            var repoUser = await _repo.GetUser(userId);
            var file = photoCreation.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length == 0) {
                throw new Exception("File is empty");
            }

            // Start reading the file's content
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),

                    // This crops the user's face
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            // Set the resulting data to our photo
            photoCreation.Url = uploadResult.Uri.ToString();
            photoCreation.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoCreation);

            // Does the user have a main photo set?
            if (!repoUser.Photos.Any(u => u.AsMainPhoto)) {
                photo.AsMainPhoto = true;
            }

            repoUser.Photos.Add(photo);

            if (await _repo.SaveAll()) {
                var returnPhoto = _mapper.Map<ReturnPhoto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, returnPhoto);
            }

            return BadRequest("Could not save photo.");
        }
    }
}