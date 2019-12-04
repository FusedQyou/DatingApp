using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repoUser = await _repo.GetUser(currentUserId);

            // Set user parameters
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender)) { userParams.Gender = repoUser.Gender == "male" ? "female" : "male"; }

            var users = await _repo.GetUsers(userParams);
            var userList = _mapper.Map<IEnumerable<ListUser>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalItemCount, users.TotalPageCount); 
            return Ok(userList);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var detailedUser = _mapper.Map<DetailedUser>(user);
            return Ok(detailedUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUser updateUser)
        {
            // Verify the user that requests the update is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != tokenId) {
                return Unauthorized();
            }

            var repoUser = await _repo.GetUser(id);
            _mapper.Map(updateUser, repoUser);

            if (await _repo.SaveAll()) {
                return NoContent();
            }

            // Something has gone wrong
            throw new Exception($"User update with id \"{id}\" failed.");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            // Verify the user that requests the update is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != tokenId) {
                return Unauthorized();
            }

            // Check if we already liked this user
            var like = await _repo.GetLike(id, recipientId);
            if (like != null) {
                return BadRequest("You already liked this user.");
            }

            // Does the recipient exist
            if (await _repo.GetUser(recipientId) == null) {
                return NotFound("Recipient does not exist.");
            }

            like = new Like {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if (await _repo.SaveAll()) {
                return NoContent();
            }

            // Something has gone wrong
            throw new Exception("Failed to like user.");
        }
    }
}
