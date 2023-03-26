﻿using BeMyGuest.Api.Common;
using BeMyGuest.Application.Users.Queries.GetUser;
using BeMyGuest.Contracts.Users;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BeMyGuest.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : AbstractController
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

    [HttpGet("")]
    public async Task<ActionResult<GetUserResponse>> GetUser()
    {
        var query = new GetUserQuery();

        var result = await _sender.Send(query);

        return result.Match<ActionResult<GetUserResponse>>(
            user => Ok(_mapper.Map<GetUserResponse>(user)),
            _ => NotFound());
    }
}