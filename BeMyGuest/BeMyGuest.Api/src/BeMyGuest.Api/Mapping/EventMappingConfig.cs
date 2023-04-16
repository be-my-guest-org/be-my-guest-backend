using BeMyGuest.Application.Events.Commands.CreateEvent;
using BeMyGuest.Application.Events.Queries.GetEvent;
using BeMyGuest.Common.Common;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.Utils;
using BeMyGuest.Contracts.Events.Create;
using BeMyGuest.Contracts.Events.Get;
using BeMyGuest.Domain.Events;
using BeMyGuest.Domain.Events.ValueObjects;
using BeMyGuest.Infrastructure.Persistence.Events;
using Mapster;

namespace BeMyGuest.Api.Mapping;

public class EventMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        RegisterCreate(config);

        RegisterGet(config);
    }

    private static void RegisterGet(TypeAdapterConfig config)
    {
        config.NewConfig<GetEventRequest, GetEventQuery>();

        config.NewConfig<(EventDataSnapshot, IEnumerable<EventParticipantSnapshot>), Event>()
            .ConstructUsing(src => CreateEvent(src));

        config.NewConfig<Event, GetEventResponse>()
            .Map(dest => dest.Status, src => src.Status.Value);
    }

    private static void RegisterCreate(TypeAdapterConfig config)
    {
        config.NewConfig<CreateEventRequest, CreateEventCommand>();

        config.NewConfig<Event, EventDataSnapshot>().Map<, string>(dest => dest.HostId, src => src.HostId.ToString().PrependKeyIdentifiers(KeyIdentifiers.User))
            .Map(dest => dest.EventId, src => src.Id.ToString().PrependKeyIdentifiers(KeyIdentifiers.Event, KeyIdentifiers.Host))
            .Map(dest => dest.Status, src => src.Status.Value);

        config.NewConfig<Event, CreateEventResponse>()
            .Map(dest => dest.Status, src => src.Status.Value);
    }

    private static Event CreateEvent((EventDataSnapshot, IEnumerable<EventParticipantSnapshot>) data)
    {
        (EventDataSnapshot eventData, IEnumerable<EventParticipantSnapshot> participants) = data;
        var participantsList = participants.ToList();

        return Event.Create(
            Guid.Parse(eventData.EventId.RemoveKeyIdentifiers()),
            eventData.Title,
            eventData.Description,
            eventData.When,
            eventData.Where,
            eventData.MaxParticipants,
            Guid.Parse(participantsList.Single(p => p.Role == ParticipantRoles.Host).UserId.RemoveKeyIdentifiers()),
            Status.From(eventData.Status),
            participantsList.Where(p => p.Role == ParticipantRoles.Guest).Select(p => Guid.Parse(p.UserId.RemoveKeyIdentifiers())).ToList(),
            eventData.CreatedAt,
            eventData.UpdatedAt);
    }
}