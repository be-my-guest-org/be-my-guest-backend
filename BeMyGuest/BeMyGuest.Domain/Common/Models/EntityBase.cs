using CSharpFunctionalExtensions;

namespace BeMyGuest.Domain.Common.Models;

public abstract class EntityBase<TId> : Entity<TId>
{
    protected EntityBase(
        TId id,
        DateTime createdAt,
        DateTime? updatedAt = null)
        : base(id)
    {
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; protected set; }
}