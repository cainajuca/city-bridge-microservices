namespace ApiTwo.Api._3_Domain.Entities;
public class Bridge
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public Guid CityId { get; set; } // FK
}
