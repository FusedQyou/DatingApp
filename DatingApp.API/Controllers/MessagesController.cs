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
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class messagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public messagesController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            // Verify the user that requests the request is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != tokenId) {
                return Unauthorized();
            }

            var repoMessage = await _repo.GetMessage(id);
            if (repoMessage == null) {
                return NotFound();
            }

            return Ok(repoMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            // Verify the user that requests the creation is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != tokenId) {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var repoMessages = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<ReturnedMessage>>(repoMessages);

            Response.AddPagination(repoMessages.CurrentPage, repoMessages.PageSize, repoMessages.TotalItemCount, repoMessages.TotalPageCount);
            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            // Verify the user that requests the creation is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != tokenId) {
                return Unauthorized();
            }

            var repoMessage = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<ReturnedMessage>>(repoMessage);
            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, CreateMessage createMessage)
        {
            // Verify the user that requests the creation is the same as the user received from the token.
            var tokenId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userId != tokenId) {
                return Unauthorized();
            }

            createMessage.SenderId = userId;
            var recipient = await _repo.GetUser(createMessage.RecipientId);

            if (recipient == null) {
                return BadRequest("Could not find receiver.");
            }

            var message = _mapper.Map<Message>(createMessage);
            _repo.Add(message);

            if (await _repo.SaveAll()) {
                var returnedMessage = _mapper.Map<CreateMessage>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, returnedMessage);
            }

            throw new Exception("Failed to create message.");
        }
    }
}
