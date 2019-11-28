using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var detailedUser = _mapper.Map<IEnumerable<ListUser>>(users);
            return Ok(detailedUser);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var detailedUser = _mapper.Map<DetailedUser>(user);
            return Ok(detailedUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUser updateUser)
        {
            // Verify the user that requests the update is the same as the updated user
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
    }
}
