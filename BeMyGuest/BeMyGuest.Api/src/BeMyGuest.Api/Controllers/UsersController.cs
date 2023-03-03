using BeMyGuest.Application.Users.Queries.GetUser;
using BeMyGuest.Contracts.Users;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BeMyGuest.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public UsersController(ILogger<UsersController> logger, ISender sender, IMapper mapper)
    {
        _logger = logger;
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("echo")]
    public async Task<IActionResult> GetUser(GetUserRequest request)
    {
        var query = _mapper.Map<GetUserQuery>(request);

        await _sender.Send(query);

        return Ok();
    }
}