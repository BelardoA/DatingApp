using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;

    public MessagesController(IUserRepository userRepository, IMessagesRepository messagesRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _messagesRepository = messagesRepository;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var userName = User.GetUserName();

        if (userName == createMessageDto.RecipientUserName.ToLower())
        {
            return BadRequest("You cannot send messages to yourself.");
        }

        var sender = await _userRepository.GetUserByUserNameAsync(userName);

        var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUserName = sender.UserName,
            Content = createMessageDto.Content
        };

        _messagesRepository.AddMessage(message);

        if (await _messagesRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message.");
    }
}