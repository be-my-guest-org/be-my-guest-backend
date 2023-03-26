using BeMyGuest.Application.Events.Commands.CreateEvent;
using BeMyGuest.Common.Identifiers;
using BeMyGuest.Common.Utils;
using BeMyGuest.Contracts.Events;
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
            .Map(dest => dest.HostId, src => src.HostId.PrependKeyIdentifier(KeyIdentifiers.User))
            .Map(dest => dest.EventId, src => src.Id.ToString().PrependKeyIdentifier(KeyIdentifiers.Event));

        config.NewConfig<Event, CreateEventResponse>();
    }
}