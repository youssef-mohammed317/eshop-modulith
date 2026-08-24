namespace Shared.DDD;

public interface IEntity<TId> : IEntity
{
    public TId Id { get; set; }
}
public interface IEntity
{
    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
