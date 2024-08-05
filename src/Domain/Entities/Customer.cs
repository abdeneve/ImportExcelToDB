using Domain.Common;

namespace Domain.Entities;
public class Customer : Entity
{
    public string Region { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }


    public readonly List<Sale> _sales;
    public IReadOnlyCollection<Sale> Sales => _sales;
}

