using BeMyGuest.Api.Common;
using BeMyGuest.Application.Events.Commands.CreateEvent;
using BeMyGuest.Application.Events.Queries.GetEvent;
using BeMyGuest.Contracts.Events.Create;
using BeMyGuest.Contracts.Events.Get;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BeMyGuest.Api.Controllers;

[ApiController]
[Route("api/v1/events")]
public class EventsController : AbstractController
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public EventsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("{eventId}")]
    public async Task<ActionResult<GetEventResponse>> GetEvent([FromRoute] GetEventRequest request)
    {
        var query = request.Adapt<GetEventQuery>();

        var result = await _sender.Send(query);

        return result.Match<ActionResult<GetEventResponse>>(
            user => Ok(_mapper.Map<GetEventResponse>(user)),
            _ => NotFound());
    }

    [HttpPost("")]
    public async Task<ActionResult<CreateEventResponse>> CreateEvent(CreateEventRequest request)
    {
        var command = request.Adapt<CreateEventCommand>();

        var result = await _sender.Send(command);

        return result.Match<ActionResult<CreateEventResponse>>(
            user => Ok(_mapper.Map<CreateEventResponse>(user)),
            _ => InternalServerError());
    }
}