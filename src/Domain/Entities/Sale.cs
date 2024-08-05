using Domain.Common;

namespace Domain.Entities;
public class Sale : Entity
{
    public int CountryId { get; set; }
    public int CustomerId { get; set; }
    public string StoreCustomerCode { get; set; }
    public string CustomerItemCode { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }

    public Country Country { get; set; }
    public Customer Customer { get; set; }
}

