using System.Diagnostics.CodeAnalysis;
using BeMyGuest.Api.Common;
using BeMyGuest.Application.Events.Commands.CreateEvent;
using BeMyGuest.Application.Events.Commands.JoinEvent;
using BeMyGuest.Application.Events.Queries.GetAllCurrentUserEvents;
using BeMyGuest.Application.Events.Queries.GetEvent;
using BeMyGuest.Contracts.Events.Create;
using BeMyGuest.Contracts.Events.Get;
using BeMyGuest.Contracts.Events.GetAllForCurrentUser;
using BeMyGuest.Contracts.Events.Join;
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
            evt => Ok(_mapper.Map<GetEventResponse>(evt)),
            _ => NotFound());
    }

    [HttpGet("")]
    public async Task<ActionResult<GetAllCurrentUserEventsResponse>> GetAllCurrentUserEvents()
    {
        var command = new GetAllCurrentUserEventsQuery();

        var result = await _sender.Send(command);

        return result.Match<ActionResult<GetAllCurrentUserEventsResponse>>(
            events => Ok(new GetAllCurrentUserEventsResponse(events.Select(ev => _mapper.Map<GetEventResponse>(ev)))));
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

    [HttpPost("{eventId}/join")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public async Task<ActionResult> JoinEvent([FromRoute] JoinEventRequest request)
    {
        var query = request.Adapt<JoinEventCommand>();

        var result = await _sender.Send(query);

        return result.Match<ActionResult>(
            success => Ok(),
            notFound => NotFound(),
            tooManyGuests => BadRequest(),
            guestAlreadyJoined => BadRequest(),
            error => InternalServerError());
    }
}