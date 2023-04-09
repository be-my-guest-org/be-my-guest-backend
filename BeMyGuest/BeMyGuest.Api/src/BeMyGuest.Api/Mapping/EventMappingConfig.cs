using BeMyGuest.Application.Events.Commands.CreateEvent;
using BeMyGuest.Application.Events.Queries.GetEvent;
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

        config.NewConfig<EventSnapshot, Event>()
            .ConstructUsing(src =>
                Event.Create(
                    Guid.Parse(src.EventId.RemoveKeyIdentifiers()),
                    src.Title,
                    src.Description,
                    src.When,
                    src.Where,
                    src.MaxParticipants,
                    Guid.Parse(src.HostId.RemoveKeyIdentifiers()),
                    Status.From(src.Status),
                    src.Guests,
                    src.CreatedAt,
                    src.UpdatedAt));

        config.NewConfig<Event, GetEventResponse>()
            .Map(dest => dest.Status, src => src.Status.Value);
    }

    private static void RegisterCreate(TypeAdapterConfig config)
    {
        config.NewConfig<CreateEventRequest, CreateEventCommand>();

        config.NewConfig<Event, EventSnapshot>()
            .Map(dest => dest.HostId, src => src.HostId.ToString().PrependKeyIdentifiers(KeyIdentifiers.User))
            .Map(dest => dest.EventId, src => src.Id.ToString().PrependKeyIdentifiers(KeyIdentifiers.Event, KeyIdentifiers.Host))
            .Map(dest => dest.Status, src => src.Status.Value);

        config.NewConfig<Event, CreateEventResponse>()
            .Map(dest => dest.Status, src => src.Status.Value);
    }
}