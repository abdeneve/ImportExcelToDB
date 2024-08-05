namespace Domain.Common;
public abstract class Entity
{
    public int Id { get; set; }

    public bool IsUnnasigned()
    {
        return Id == default;
    }
}
