namespace HrManager.Domain.ValueObjects;
public class Address : ValueObject
{
    public Address(string region, string street, string house, string apartment, string fullAddress)
    {
        Region = region;
        Street = street;
        House = house;
        Apartment = apartment;
        FullAddress = fullAddress;
    }

    public string Region { get; private set; }

    public string Street { get; private set; }

    public string House { get; private set; }

    public string Apartment { get; private set; }

    public string FullAddress { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Region;
        yield return Street;
        yield return House;
        yield return Apartment;
        yield return FullAddress;
    }
}
