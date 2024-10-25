
namespace first_api_backend.Models;

public class Company {
  public long Id { get; set; }
  public string? Name { get; set; }
  public int OrganizationNumber { get; set; }
  public string? ContactPerson { get; set; }
  public string? PhoneNumber { get; set; }
}