namespace ApiTwo.Api._3_Domain.Dtos;
public class BridgeDto
{
    public string Name { get; set; } = null!;
    public Guid CityId { get; set; }
}

public class BridgeWithIdDto : BridgeDto
{
    public Guid Id { get; set; }
}