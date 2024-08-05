using Domain.Common;

namespace Domain.Entities;
public class Country : Entity
{
    public string Code { get; set; }
    public string Name { get; set; }

    public readonly List<Sale> _sales;
    public IReadOnlyCollection<Sale> Sales => _sales;
}

