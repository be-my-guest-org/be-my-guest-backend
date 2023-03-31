using BeMyGuest.Application.Events.Commands.CreateEvent;
using BeMyGuest.Application.Events.Queries.GetEvent;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.Utils;
using BeMyGuest.Contracts.Events.Create;
using BeMyGuest.Contracts.Events.Get;
using BeMyGuest.Domain.Events;
using BeMyGuest.Infrastructure.Persistence.Events;
using Mapster;

namespace BeMyGuest.Api.Mapping;

public class EventMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateEventRequest, CreateEventCommand>();

        config.NewConfig<Event, EventSnapshot>()
            .Map(dest => dest.HostId, src => src.HostId.ToString().PrependKeyIdentifier(KeyIdentifiers.User))
            .Map(dest => dest.EventId, src => src.Id.ToString().PrependKeyIdentifier(KeyIdentifiers.Event));

        config.NewConfig<Event, CreateEventResponse>();

        config.NewConfig<GetEventRequest, GetEventQuery>();

        config.NewConfig<EventSnapshot, Event>()
            .ConstructUsing(src =>
                Event.Create(
                    Guid.Parse(src.EventId.RemoveKeyIdentifier()),
                    src.Title,
                    src.Description,
                    src.When,
                    src.Where,
                    src.MaxParticipants,
                    Guid.Parse(src.HostId.RemoveKeyIdentifier()),
                    src.CreatedAt,
                    src.UpdatedAt));
    }
}